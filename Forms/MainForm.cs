using System;
using System.Linq;
using System.Windows.Forms;
using ShutdownScheduler.Core;

namespace ShutdownScheduler.Forms
{
    public partial class MainForm : Form
    {
        private readonly TaskManager _taskManager;
        private readonly AppConfig _config;

        private ListBox taskList;
        private Label lblRole;

        // Buttons
        private Button btnShutdown;
        private Button btnRestart;
        private Button btnDaily;
        private Button btnWeekly;
        private Button btnOneTime;
        private Button btnDelete;
        private Button btnDeleteAll;
        private Button btnRefresh;
        private Button btnRole;
        private Button btnWol;

        public MainForm(TaskManager taskManager, AppConfig config)
        {
            _taskManager = taskManager;
            _config = config;

            InitializeComponent();
            RefreshTaskList();
        }

        private void InitializeComponent()
        {
            this.Text = "Shutdown Scheduler";
            this.Width = 700;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            // === Main layout ===
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 250)); // Fixed height for task list
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Buttons area
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Footer fixed height

            // === Task list ===
            taskList = new ListBox
            {
                Dock = DockStyle.Fill,
                Height = 250
            };

            // === Button grid (3x3) ===
            var buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                Padding = new Padding(10)
            };

            for (int i = 0; i < 3; i++)
                buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            for (int i = 0; i < 3; i++)
                buttonLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33));

            btnShutdown = new Button { Text = "Shutdown Now", Dock = DockStyle.Fill };
            btnRestart = new Button { Text = "Restart Now", Dock = DockStyle.Fill };
            btnDaily = new Button { Text = "Add Daily", Dock = DockStyle.Fill };
            btnWeekly = new Button { Text = "Add Weekly", Dock = DockStyle.Fill };
            btnOneTime = new Button { Text = "Add One-Time", Dock = DockStyle.Fill };
            btnDelete = new Button { Text = "Delete Selected", Dock = DockStyle.Fill };
            btnDeleteAll = new Button { Text = "Delete All", Dock = DockStyle.Fill };
            btnRefresh = new Button { Text = "Refresh", Dock = DockStyle.Fill };

            buttonLayout.Controls.Add(btnShutdown, 0, 0);
            buttonLayout.Controls.Add(btnRestart, 1, 0);
            buttonLayout.Controls.Add(btnDaily, 2, 0);

            buttonLayout.Controls.Add(btnWeekly, 0, 1);
            buttonLayout.Controls.Add(btnOneTime, 1, 1);
            buttonLayout.Controls.Add(btnDelete, 2, 1);

            buttonLayout.Controls.Add(btnDeleteAll, 0, 2);
            buttonLayout.Controls.Add(btnRefresh, 1, 2);

            // === Footer: Role label + Change Role + Wake Clients ===
            var footerPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(5)
            };
            footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Role label
            footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Change Role
            footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); // Wake Clients

            lblRole = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = $"Role: {_config.Role}"
            };

            btnRole = new Button
            {
                Text = "Change Role",
                Dock = DockStyle.Fill,
                Height = 35
            };

            btnWol = new Button
            {
                Text = "Wake Clients",
                Dock = DockStyle.Fill,
                Height = 35
            };
            btnWol.Click += (s, e) =>
            {
                // This will later use ClientMacs from config
                MessageBox.Show("Wake-on-LAN packets would be sent here.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            footerPanel.Controls.Add(lblRole, 0, 0);
            footerPanel.Controls.Add(btnRole, 1, 0);
            footerPanel.Controls.Add(btnWol, 2, 0);

            // === Add everything to main layout ===
            mainLayout.Controls.Add(taskList, 0, 0);
            mainLayout.Controls.Add(buttonLayout, 0, 1);
            mainLayout.Controls.Add(footerPanel, 0, 2);

            this.Controls.Add(mainLayout);

            // === Event bindings ===
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
                    string name = selected.Split(' ')[0];
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

            btnRole.Click += (s, e) =>
            {
                using (var roleForm = new RoleSelectorForm(_config))
                {
                    if (roleForm.ShowDialog() == DialogResult.OK)
                    {
                        lblRole.Text = $"Role: {_config.Role}";
                        MessageBox.Show("Role updated. Please restart the app.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
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
