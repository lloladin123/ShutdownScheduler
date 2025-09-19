using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Timers;

namespace ShutdownScheduler
{
    public partial class ShutdownPopup : Form
    {
        private readonly SchedulerService _scheduler;
        private int _secondsRemaining;
        private readonly bool _isRestart;
        private readonly System.Timers.Timer _timer;

        private Label lblMessage;
        private Button btnCancel;

        // 🔹 Import Win32 APIs
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        // Flags for FlashWindowEx
        private const uint FLASHW_ALL = 3;
        private const uint FLASHW_TIMERNOFG = 12;

        public ShutdownPopup(SchedulerService scheduler, int seconds, bool isRestart = false)
        {
            _scheduler = scheduler;
            _secondsRemaining = seconds;
            _isRestart = isRestart;

            InitializeComponent();

            this.TopMost = true;

            this.Shown += (s, e) =>
            {
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Activate();
                SetForegroundWindow(this.Handle);
                ForceAttention();
            };

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            UpdateMessage();
        }

        private void InitializeComponent()
        {
            this.Text = _isRestart ? "Restart Warning" : "Shutdown Warning";
            this.Width = 400;
            this.Height = 250;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            // Message
            lblMessage = new Label
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            layout.Controls.Add(lblMessage, 0, 0);

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel Shutdown",
                Dock = DockStyle.Fill,
                Height = 40
            };
            btnCancel.Click += BtnCancel_Click;
            layout.Controls.Add(btnCancel, 0, 1);

            this.Controls.Add(layout);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _secondsRemaining--;
            if (_secondsRemaining <= 0)
            {
                _timer.Stop();
                this.Invoke((Action)(() =>
                {
                    this.Close();
                    if (_isRestart)
                        _scheduler.RunShutdown("/r /f /t 0");
                    else
                        _scheduler.RunShutdown("/s /f /t 0");
                }));
            }
            else
            {
                this.Invoke((Action)(UpdateMessage));
            }
        }

        private void UpdateMessage()
        {
            lblMessage.Text = _isRestart
                ? $"⚠️ System will restart in {_secondsRemaining} seconds."
                : $"⚠️ System will shut down in {_secondsRemaining} seconds.";
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            _scheduler.RunShutdown("/a"); // abort shutdown
            MessageBox.Show(
                _isRestart ? "Restart cancelled." : "Shutdown cancelled.",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            this.Close();
        }

        private void ForceAttention()
        {
            this.TopMost = false;
            this.TopMost = true;

            FLASHWINFO fw = new FLASHWINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(FLASHWINFO)),
                hwnd = this.Handle,
                dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG,
                uCount = uint.MaxValue,
                dwTimeout = 0
            };
            FlashWindowEx(ref fw);
        }
    }
}
