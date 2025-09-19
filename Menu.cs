using System;
using System.Linq;
using System.Windows.Forms;

namespace ShutdownScheduler
{
    public class Menu
    {
        private readonly TaskManager _taskManager;

        public Menu(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Shutdown Scheduler ===");
                Console.WriteLine("1. Shutdown now");
                Console.WriteLine("2. Restart now");
                Console.WriteLine("3. Schedule daily shutdown");
                Console.WriteLine("4. Schedule weekly shutdown");
                Console.WriteLine("5. Schedule one-time shutdown");
                Console.WriteLine("6. View scheduled shutdowns");
                Console.WriteLine("7. Delete a specific shutdown");
                Console.WriteLine("8. Cancel ALL shutdowns");
                Console.WriteLine("9. Exit");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        _taskManager.ShutdownNow();
                        Pause();
                        break;

                    case "2":
                        _taskManager.RestartNow();
                        Pause();
                        break;

                    case "3":
                        var dailyTime = PromptTime("Pick a time for daily shutdown");
                        if (!string.IsNullOrEmpty(dailyTime))
                            _taskManager.ScheduleDaily(dailyTime);
                        Pause();
                        break;

                    case "4":
                        var weeklyDay = PromptDay("Pick a day for weekly shutdown");
                        var weeklyTime = PromptTime("Pick a time for weekly shutdown");
                        if (!string.IsNullOrEmpty(weeklyDay) && !string.IsNullOrEmpty(weeklyTime))
                            _taskManager.ScheduleWeekly(weeklyDay, weeklyTime);
                        Pause();
                        break;

                    case "5":
                        var oneDate = PromptDate("Pick a date for one-time shutdown");
                        var oneTime = PromptTime("Pick a time for one-time shutdown");
                        if (!string.IsNullOrEmpty(oneDate) && !string.IsNullOrEmpty(oneTime))
                            _taskManager.ScheduleOneTime(oneDate, oneTime);
                        Pause();
                        break;

                    case "6":
                        _taskManager.ViewTasks();
                        Pause();
                        break;

                    case "7":
                        _taskManager.DeleteTaskInteractive();
                        Pause();
                        break;

                    case "8":
                        _taskManager.DeleteAllTasks();
                        Pause();
                        break;

                    case "9":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        private void Pause()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // 🔹 Time picker
        private string PromptTime(string title)
        {
            using var picker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Now
            };

            using var form = new Form
            {
                Width = 250,
                Height = 120,
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };

            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 80, Width = 80, Top = 50 };
            form.Controls.Add(picker);
            form.Controls.Add(ok);
            form.AcceptButton = ok;

            return form.ShowDialog() == DialogResult.OK ? picker.Value.ToString("HH:mm") : null;
        }

        // 🔹 Date picker
        private string PromptDate(string title)
        {
            using var picker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.Date
            };

            using var form = new Form
            {
                Width = 250,
                Height = 120,
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };

            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 80, Width = 80, Top = 50 };
            form.Controls.Add(picker);
            form.Controls.Add(ok);
            form.AcceptButton = ok;

            return form.ShowDialog() == DialogResult.OK ? picker.Value.ToString("yyyy-MM-dd") : null;
        }

        // 🔹 Day picker
        private string PromptDay(string title)
        {
            var combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = 20,
                Top = 10,
                Width = 150
            };
            combo.Items.AddRange(new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" });
            combo.SelectedIndex = 0;

            using var form = new Form
            {
                Width = 250,
                Height = 120,
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };

            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 80, Width = 80, Top = 50 };
            form.Controls.Add(combo);
            form.Controls.Add(ok);
            form.AcceptButton = ok;

            return form.ShowDialog() == DialogResult.OK ? combo.SelectedItem.ToString() : null;
        }
    }
}
