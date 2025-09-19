using System;
using System.Linq;
using System.Windows.Forms;

namespace ShutdownScheduler.Forms
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
            this.Text = "Shutdown Scheduler";
            this.Width = 500;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterScreen;

            taskList = new ListBox { Left = 20, Top = 20, Width = 440, Height = 200 };
            btnShutdown = new Button { Text = "Shutdown Now", Left = 20, Top = 230, Width = 130 };
            btnRestart = new Button { Text = "Restart Now", Left = 160, Top = 230, Width = 130 };
            btnDaily = new Button { Text = "Add Daily", Left = 300, Top = 230, Width = 160 };
            btnWeekly = new Button { Text = "Add Weekly", Left = 20, Top = 270, Width = 130 };
            btnOneTime = new Button { Text = "Add One-Time", Left = 160, Top = 270, Width = 130 };
            btnDelete = new Button { Text = "Delete Selected", Left = 300, Top = 270, Width = 160 };
            btnDeleteAll = new Button { Text = "Delete All", Left = 20, Top = 310, Width = 130 };
            btnRefresh = new Button { Text = "Refresh", Left = 160, Top = 310, Width = 130 };

            // 🔹 Event bindings
            btnShutdown.Click += (s, e) => _taskManager.ShutdownNow();
            btnRestart.Click += (s, e) => _taskManager.RestartNow();

            btnDaily.Click += (s, e) =>
            {
                using (var picker = new Prompts.DailyPrompt())
                {
                    if (picker.ShowDialog() == DialogResult.OK)
                    {
                        _taskManager.ScheduleDaily(picker.SelectedTime);
                        RefreshTaskList();
                    }
                }
            };

            btnWeekly.Click += (s, e) =>
            {
                using (var picker = new Prompts.WeeklyPrompt())
                {
                    if (picker.ShowDialog() == DialogResult.OK)
                    {
                        _taskManager.ScheduleWeekly(picker.SelectedDay, picker.SelectedTime);
                        RefreshTaskList();
                    }
                }
            };

            btnOneTime.Click += (s, e) =>
            {
                using (var picker = new Prompts.OneTimePrompt())
                {
                    if (picker.ShowDialog() == DialogResult.OK)
                    {
                        _taskManager.ScheduleOneTime(picker.SelectedDate, picker.SelectedTime);
                        RefreshTaskList();
                    }
                }
            };

            btnDelete.Click += (s, e) =>
            {
                if (taskList.SelectedItem is string selected)
                {
                    string name = selected.Split(' ')[0]; // Task name only
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

            this.Controls.AddRange(new Control[]
            {
                taskList, btnShutdown, btnRestart,
                btnDaily, btnWeekly, btnOneTime,
                btnDelete, btnDeleteAll, btnRefresh
            });
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
    }
}
