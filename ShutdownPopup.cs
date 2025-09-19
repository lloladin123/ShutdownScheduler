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

            // ✅ Always keep on top
            this.TopMost = true;

            // ✅ When shown, force it forward and flash if blocked
            this.Shown += (s, e) =>
            {
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Activate();

                // 🚀 Try force focus
                SetForegroundWindow(this.Handle);

                // 🚨 If blocked, flash + jump
                ForceAttention();
            };

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            UpdateMessage();
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.btnCancel = new Button();

            this.SuspendLayout();

            // Label
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblMessage.Location = new System.Drawing.Point(20, 20);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(250, 25);

            // Cancel button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(20, 60);
            this.btnCancel.Size = new System.Drawing.Size(200, 40);
            this.btnCancel.Click += BtnCancel_Click;

            // Form
            this.ClientSize = new System.Drawing.Size(320, 120);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = _isRestart ? "Restart Warning" : "Shutdown Warning";

            this.ResumeLayout(false);
            this.PerformLayout();
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
                ? $"System will restart in {_secondsRemaining} seconds."
                : $"System will shut down in {_secondsRemaining} seconds.";
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

        // 🔹 Flash + TopMost toggle if focus is blocked
        private void ForceAttention()
        {
            // Toggle TopMost to push it above everything
            this.TopMost = false;
            this.TopMost = true;

            // Flash taskbar button until focused
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
