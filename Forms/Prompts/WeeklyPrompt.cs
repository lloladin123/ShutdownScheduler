using System;
using System.Windows.Forms;

namespace ShutdownScheduler.Forms.Prompts
{
    public class WeeklyPrompt : Form
    {
        private ComboBox dayCombo;
        private DateTimePicker timePicker;
        private Button okBtn;
        private Button cancelBtn;

        public string SelectedDay => dayCombo.SelectedItem?.ToString() ?? "";
        public string SelectedTime => timePicker.Value.ToString("HH:mm");

        public WeeklyPrompt()
        {
            this.Text = "Pick Weekly Shutdown";
            this.Width = 350;
            this.Height = 200;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblDay = new Label { Text = "Day:", Left = 20, Top = 20, AutoSize = true };
            Label lblTime = new Label { Text = "Time (HH:mm):", Left = 20, Top = 60, AutoSize = true };

            dayCombo = new ComboBox
            {
                Left = 120,
                Top = 18,
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            dayCombo.Items.AddRange(new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" });

            timePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Left = 120,
                Top = 58,
                Width = 180
            };

            okBtn = new Button { Text = "OK", Left = 120, Top = 100, Width = 80 };
            cancelBtn = new Button { Text = "Cancel", Left = 220, Top = 100, Width = 80 };

            okBtn.Click += OkBtn_Click;
            cancelBtn.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblDay);
            this.Controls.Add(lblTime);
            this.Controls.Add(dayCombo);
            this.Controls.Add(timePicker);
            this.Controls.Add(okBtn);
            this.Controls.Add(cancelBtn);
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedDay))
            {
                MessageBox.Show("❌ Please select a valid day.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectedTime))
            {
                MessageBox.Show("❌ Please select a valid time.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
