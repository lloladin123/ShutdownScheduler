using System;
using System.Linq;
using System.Windows.Forms;

namespace ShutdownScheduler
{
    public partial class MainForm : Form
    {
        private readonly TaskManager _taskManager;

        public MainForm(TaskManager taskManager)
        {
            _taskManager = taskManager;
            InitializeComponent();
            RefreshTaskList();
        }

        private ListBox taskList;
        private Button btnShutdown;
        private Button btnRestart;
        private Button btnDaily;
        private Button btnWeekly;
        private Button btnOneTime;
        private Button btnDelete;
        private Button btnDeleteAll;
        private Button btnRefresh;

        private void InitializeComponent()
        {
            Text = "Shutdown Scheduler";
            Width = 500;
            Height = 400;
            StartPosition = FormStartPosition.CenterScreen;

            taskList = new ListBox { Left = 20, Top = 20, Width = 440, Height = 200 };
            btnShutdown = new Button { Text = "Shutdown Now", Left = 20, Top = 230, Width = 130 };
            btnRestart = new Button { Text = "Restart Now", Left = 160, Top = 230, Width = 130 };
            btnDaily = new Button { Text = "Add Daily", Left = 300, Top = 230, Width = 160 };
            btnWeekly = new Button { Text = "Add Weekly", Left = 20, Top = 270, Width = 130 };
            btnOneTime = new Button { Text = "Add One-Time", Left = 160, Top = 270, Width = 130 };
            btnDelete = new Button { Text = "Delete Selected", Left = 300, Top = 270, Width = 160 };
            btnDeleteAll = new Button { Text = "Delete All", Left = 20, Top = 310, Width = 130 };
            btnRefresh = new Button { Text = "Refresh", Left = 160, Top = 310, Width = 130 };

            // Bind events
            btnShutdown.Click += (s, e) => _taskManager.ShutdownNow();
            btnRestart.Click += (s, e) => _taskManager.RestartNow();
            btnDaily.Click += (s, e) =>
            {
                string time = Prompt("Enter time (HH:mm):");
                if (!string.IsNullOrEmpty(time)) _taskManager.ScheduleDaily(time);
                RefreshTaskList();
            };
            btnWeekly.Click += (s, e) =>
            {
                string day = Prompt("Enter day (MON-SUN):");
                string time = Prompt("Enter time (HH:mm):");
                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(time))
                    _taskManager.ScheduleWeekly(day, time);
                RefreshTaskList();
            };
            btnOneTime.Click += (s, e) =>
            {
                string date = Prompt("Enter date (yyyy-MM-dd):");
                string time = Prompt("Enter time (HH:mm):");
                if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(time))
                    _taskManager.ScheduleOneTime(date, time);
                RefreshTaskList();
            };
            btnDelete.Click += (s, e) =>
            {
                if (taskList.SelectedItem is string selected)
                {
                    string name = selected.Split(' ')[0]; // task name
                    // Use TaskManager delete logic
                    _taskManager.DeleteTaskByName(name);
                    RefreshTaskList();
                }
            };
            btnDeleteAll.Click += (s, e) =>
            {
                _taskManager.DeleteAllTasks(useUiConfirm: true);
                RefreshTaskList();
            };

            btnRefresh.Click += (s, e) => RefreshTaskList();

            Controls.Add(taskList);
            Controls.Add(btnShutdown);
            Controls.Add(btnRestart);
            Controls.Add(btnDaily);
            Controls.Add(btnWeekly);
            Controls.Add(btnOneTime);
            Controls.Add(btnDelete);
            Controls.Add(btnDeleteAll);
            Controls.Add(btnRefresh);
        }

        private void RefreshTaskList()
        {
            taskList.Items.Clear();
            foreach (var t in _taskManager.GetAllTasks())
            {
                string extra = t.Type switch
                {
                    ScheduleType.Daily => $"at {t.Time}",
                    ScheduleType.Weekly => $"{t.Day} at {t.Time}",
                    ScheduleType.OneTime => $"{t.Date} at {t.Time}",
                    _ => ""
                };
                taskList.Items.Add($"{t.Name} ({t.Type}, {extra})");
            }
        }

        private string Prompt(string text)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(text, "Input", "");
        }
    }
}
