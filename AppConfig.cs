using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ShutdownScheduler.Core
{
    public enum NodeRole
    {
        Master,
        Client
    }

    public class AppConfig
    {
        public NodeRole Role { get; set; } = NodeRole.Master;

        /// <summary>
        /// Network path to the shared schedules file.
        /// Only used when Role = Client.
        /// </summary>
        public string SharedPath { get; set; } =
            @"\\MASTERPC\ShutdownSchedulerShared\schedules.json";

        /// <summary>
        /// List of MAC addresses for client PCs (used when Role = Master).
        /// </summary>
        public List<string> ClientMacs { get; set; } = new();

        private static readonly string LocalPath = "appconfig.json";

        /// <summary>
        /// Loads the configuration from disk, or creates defaults if not found/invalid.
        /// </summary>
        public static AppConfig Load()
        {
            try
            {
                if (!File.Exists(LocalPath))
                    return CreateDefault();

                var json = File.ReadAllText(LocalPath);
                var config = JsonSerializer.Deserialize<AppConfig>(json);

                return config ?? CreateDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to load appconfig.json, using defaults. Error: {ex.Message}");
                return CreateDefault();
            }
        }

        /// <summary>
        /// Saves the configuration to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(
                    this,
                    new JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText(LocalPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save appconfig.json: {ex.Message}");
            }
        }

        private static AppConfig CreateDefault()
        {
            return new AppConfig
            {
                Role = NodeRole.Master,
                SharedPath = @"\\MASTERPC\ShutdownSchedulerShared\schedules.json",
                ClientMacs = new List<string>
                {
                    "00:11:22:33:44:55",
                    "AA:BB:CC:DD:EE:FF"
                }
            };
        }
    }
}
