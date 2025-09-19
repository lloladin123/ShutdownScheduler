using System;
using System.Windows.Forms;

namespace ShutdownScheduler.Forms.Prompts
{
    public class DailyPrompt : Form
    {
        private DateTimePicker timePicker;
        private Button okBtn;
        private Button cancelBtn;

        public string SelectedTime => timePicker.Value.ToString("HH:mm");

        public DailyPrompt()
        {
            this.Text = "Pick Daily Shutdown Time";
            this.Width = 300;
            this.Height = 160;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lbl = new Label
            {
                Text = "Select time (HH:mm):",
                Left = 20,
                Top = 20,
                AutoSize = true
            };

            timePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Left = 150,
                Top = 18,
                Width = 100
            };

            okBtn = new Button { Text = "OK", Left = 150, Top = 60, Width = 70 };
            cancelBtn = new Button { Text = "Cancel", Left = 230, Top = 60, Width = 70 };

            okBtn.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(SelectedTime))
                {
                    MessageBox.Show("❌ Please select a valid time.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.DialogResult = DialogResult.OK;
            };

            cancelBtn.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lbl);
            this.Controls.Add(timePicker);
            this.Controls.Add(okBtn);
            this.Controls.Add(cancelBtn);
        }
    }
}
