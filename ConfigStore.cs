using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShutdownScheduler
{
    public class ConfigStore
    {
        private readonly string _filePath;

        public ScheduleData Data { get; private set; }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,    // forgiving JSON
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters = { new JsonStringEnumConverter() } // enums saved as strings
        };

        public ConfigStore(string filePath)
        {
            _filePath = filePath;
            Data = Load(); // load immediately
        }

        private ScheduleData Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new ScheduleData();

                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<ScheduleData>(json, JsonOptions) ?? new ScheduleData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to load config file: {_filePath}\n{ex.Message}");
                return new ScheduleData(); // fallback to empty config
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                string json = JsonSerializer.Serialize(Data, JsonOptions);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save config file: {_filePath}\n{ex.Message}");
            }
        }
    }
}
