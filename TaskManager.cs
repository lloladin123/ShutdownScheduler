using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ShutdownScheduler.Core;

namespace ShutdownScheduler
{
    public class TaskManager
    {
        private readonly SchedulerService _scheduler;
        private readonly ConfigStore _store;
        private const string TaskPrefix = "ShutdownScheduler_";
        private const int DefaultCountdown = 60; // seconds

        public TaskManager(SchedulerService scheduler, AppConfig config)
        {
            _scheduler = scheduler;

            // 🔹 Master = use local JSON
            // 🔹 Client = use shared JSON (UNC path)
            string path = config.Role == NodeRole.Master
                ? "schedules.json"
                : config.SharedPath;

            _store = new ConfigStore(path);
        }

        public System.Collections.Generic.List<ScheduleItem> GetAllTasks()
        {
            return _store.Data.Items.ToList();
        }

        // 🔹 Export tasks to a shared path (for master)
        public void ExportTasks(string path)
        {
            try
            {
                File.Copy("schedules.json", path, overwrite: true);
                Console.WriteLine($"✅ Tasks exported to {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Export failed: {ex.Message}");
            }
        }

        // 🔹 Import tasks from a shared path (for client)
        public void ImportTasks(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("⚠️ No shared tasks file found.");
                    return;
                }

                File.Copy(path, "schedules.json", overwrite: true);
                _store.Reload(); // reloads tasks from updated file
                Console.WriteLine($"✅ Tasks imported from {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Import failed: {ex.Message}");
            }
        }

        // 🔹 Delete specific task
        public void DeleteTaskByName(string name)
        {
            var task = _store.Data.Items.FirstOrDefault(i => i.Name == name);
            if (task == null)
            {
                Console.WriteLine($"❌ No such task: {name}");
                return;
            }

            _scheduler.RunSchtasks($"/delete /tn \"{task.Name}\" /f");
            _store.Data.Items.Remove(task);
            _store.Save();
            Console.WriteLine($"✅ Deleted task: {task.Name}");
        }

        // 🔹 Manual shutdown with popup
        public void ShutdownNow() => ShowPopup(DefaultCountdown, isRestart: false);
        public void RestartNow() => ShowPopup(DefaultCountdown, isRestart: true);

        private void ShowPopup(int seconds, bool isRestart = false)
        {
            using (var popup = new ShutdownPopup(_scheduler, seconds, isRestart))
            {
                popup.ShowDialog(); // modal, blocks until closed
            }
        }

        // 🔹 Get path to our app (so Task Scheduler runs it)
        private string GetAppPath()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!;
        }

        // 🔹 Shift time back one minute (so popup runs early)
        private string ShiftTimeBackOneMinute(string time)
        {
            DateTime parsed = DateTime.ParseExact(time, "HH:mm", null);
            return parsed.AddMinutes(-1).ToString("HH:mm");
        }

        // 🔹 Shared helper for adding tasks
        private bool AddTask(string name, string schtasksArgs, ScheduleItem item)
        {
            if (_store.Data.Items.Any(i => i.Name == name))
            {
                Console.WriteLine($"❌ Task already exists: {name}");
                return false;
            }

            _scheduler.RunSchtasks(schtasksArgs);
            _store.Data.Items.Add(item);
            _store.Save();

            Console.WriteLine($"✅ Task added: {name}");
            return true;
        }

        // 🔹 Schedule daily
        public bool ScheduleDaily(string time)
        {
            string shiftedTime = ShiftTimeBackOneMinute(time);
            string name = $"{TaskPrefix}Daily_{time.Replace(":", "")}";
            string exePath = GetAppPath();

            string args =
                $"/create /sc daily /tn \"{name}\" " +
                $"/tr \"\\\"{exePath}\\\" --scheduled-shutdown\" " +
                $"/st {shiftedTime} /f";

            return AddTask(name, args, new ScheduleItem
            {
                Name = name,
                Type = ScheduleType.Daily,
                Time = time
            });
        }

        // 🔹 Schedule weekly
        public bool ScheduleWeekly(string day, string time)
        {
            string shiftedTime = ShiftTimeBackOneMinute(time);
            string name = $"{TaskPrefix}{day}_{time.Replace(":", "")}";
            string exePath = GetAppPath();

            string args =
                $"/create /sc weekly /d {day} /tn \"{name}\" " +
                $"/tr \"\\\"{exePath}\\\" --scheduled-shutdown\" " +
                $"/st {shiftedTime} /f";

            return AddTask(name, args, new ScheduleItem
            {
                Name = name,
                Type = ScheduleType.Weekly,
                Day = day,
                Time = time
            });
        }

        // 🔹 Schedule one-time
        public bool ScheduleOneTime(string date, string time)
        {
            DateTime target = DateTime.ParseExact($"{date} {time}", "yyyy-MM-dd HH:mm", null);
            if (target <= DateTime.Now)
            {
                Console.WriteLine("❌ That time has already passed.");
                return false;
            }

            DateTime shifted = target.AddMinutes(-1);

            string shiftedTime = shifted.ToString("HH:mm");
            string shiftedDate = shifted.ToString(
                CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);

            string name = $"{TaskPrefix}{date}_{time.Replace(":", "")}";
            string exePath = GetAppPath();

            string args =
                $"/create /sc once /tn \"{name}\" " +
                $"/tr \"\\\"{exePath}\\\" --scheduled-shutdown\" " +
                $"/st {shiftedTime} /sd {shiftedDate} /f";

            return AddTask(name, args, new ScheduleItem
            {
                Name = name,
                Type = ScheduleType.OneTime,
                Date = date,
                Time = time
            });
        }

        // 🔹 Console: view tasks
        public void ViewTasks()
        {
            if (_store.Data.Items.Count == 0)
            {
                Console.WriteLine("⚠️ No shutdown tasks found.");
                return;
            }

            Console.WriteLine("=== Scheduled Shutdown Tasks ===");
            foreach (var t in _store.Data.Items)
            {
                string extra = t.Type switch
                {
                    ScheduleType.Daily => $"at {t.Time}",
                    ScheduleType.Weekly => $"{t.Day} at {t.Time}",
                    ScheduleType.OneTime => $"{t.Date} at {t.Time}",
                    _ => ""
                };

                Console.WriteLine($"- {t.Name} ({t.Type}, {extra})");
            }
        }

        // 🔹 Console: delete interactive
        public void DeleteTaskInteractive()
        {
            if (_store.Data.Items.Count == 0)
            {
                Console.WriteLine("⚠️ No shutdown tasks found. Use option 3, 4, or 5 to create one.");
                return;
            }

            Console.WriteLine("=== Shutdown Tasks ===");
            for (int i = 0; i < _store.Data.Items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_store.Data.Items[i].Name}");
            }

            Console.Write("Enter the number OR partial task name to delete: ");
            var input = Console.ReadLine();

            string selectedTask = int.TryParse(input, out int choice) &&
                                  choice >= 1 && choice <= _store.Data.Items.Count
                ? _store.Data.Items[choice - 1].Name
                : _store.Data.Items.FirstOrDefault(
                    t => t.Name.Contains(input, StringComparison.OrdinalIgnoreCase))?.Name;

            if (selectedTask != null)
            {
                _scheduler.RunSchtasks($"/delete /tn \"{selectedTask}\" /f");
                _store.Data.Items.RemoveAll(i => i.Name == selectedTask);
                _store.Save();
                Console.WriteLine($"✅ Deleted task: {selectedTask}");
            }
            else
            {
                Console.WriteLine("❌ Invalid choice, no matching task found.");
            }
        }

        // 🔹 Delete all tasks
        public void DeleteAllTasks(bool useUiConfirm = false)
        {
            if (_store.Data.Items.Count == 0)
            {
                if (useUiConfirm)
                    MessageBox.Show("⚠️ No shutdown tasks found.", "Delete All",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    Console.WriteLine("⚠️ No shutdown tasks found.");
                return;
            }

            bool confirmed = false;

            if (useUiConfirm)
            {
                var result = MessageBox.Show(
                    "⚠️ Are you sure you want to delete ALL shutdown tasks?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                confirmed = result == DialogResult.Yes;
            }
            else
            {
                Console.Write("⚠️ Are you sure you want to delete ALL shutdown tasks? (y/n): ");
                var confirm = Console.ReadLine();
                confirmed = string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase);
            }

            if (!confirmed)
            {
                if (useUiConfirm)
                    MessageBox.Show("❌ Cancelled, no tasks deleted.", "Delete All",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    Console.WriteLine("❌ Cancelled, no tasks deleted.");
                return;
            }

            foreach (var task in _store.Data.Items.ToList())
            {
                _scheduler.RunSchtasks($"/delete /tn \"{task.Name}\" /f");
                Console.WriteLine($"✅ Deleted: {task.Name}");
            }

            _store.Data.Items.Clear();
            _store.Save();

            if (useUiConfirm)
                MessageBox.Show("✅ All shutdown tasks deleted.", "Delete All",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
