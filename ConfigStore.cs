using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ShutdownScheduler.Core
{
    public class ConfigStore
    {
        public ScheduleData Data { get; private set; } = new();
        private readonly string _path;

        public ConfigStore(string path)
        {
            _path = path;
            Load();
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_path))
                {
                    Data = new ScheduleData();
                    Save(); // create fresh file
                    return;
                }

                var json = File.ReadAllText(_path);
                Data = JsonSerializer.Deserialize<ScheduleData>(json) ?? new ScheduleData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to load {_path}: {ex.Message}");
                Data = new ScheduleData();
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save {_path}: {ex.Message}");
            }
        }

        // 🔹 Reload tasks from disk
        public void Reload()
        {
            Load();
        }
    }

    public class ScheduleData
    {
        public List<ScheduleItem> Items { get; set; } = new();
    }
}
