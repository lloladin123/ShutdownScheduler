using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShutdownScheduler
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var scheduler = new SchedulerService();

            // 🔹 Save config in AppData so it’s persistent per-user
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "ShutdownScheduler");
            Directory.CreateDirectory(folder); // make sure folder exists
            string configPath = Path.Combine(folder, "schedules.json");

            var store = new ConfigStore(configPath);
            var taskManager = new TaskManager(scheduler, store);

            // 🔹 Check if launched by Task Scheduler
            if (args.Contains("--scheduled-shutdown"))
            {
                // Show popup timer only
                Application.Run(new ShutdownPopup(scheduler, 60, isRestart: false));
            }
            else
            {
                // Show full MainForm UI
                Application.Run(new Forms.MainForm(taskManager));
            }
        }
    }
}
