using System;
using System.Windows.Forms;
using ShutdownScheduler.Core;

namespace ShutdownScheduler
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var config = AppConfig.Load();
            var scheduler = new SchedulerService();
            var taskManager = new TaskManager(scheduler, config);

            Application.Run(new Forms.MainForm(taskManager, config));
        }
    }
}
