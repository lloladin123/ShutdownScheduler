using System;
using System.Windows.Forms;

namespace ShutdownScheduler.Forms.Prompts
{
    public class OneTimePrompt : Form
    {
        private DateTimePicker datePicker;
        private DateTimePicker timePicker;
        private Button okBtn;
        private Button cancelBtn;

        public string SelectedDate => datePicker.Value.ToString("yyyy-MM-dd");
        public string SelectedTime => timePicker.Value.ToString("HH:mm");

        public OneTimePrompt()
        {
            this.Text = "Pick One-Time Shutdown";
            this.Width = 350;
            this.Height = 180;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblDate = new Label { Text = "Date:", Left = 20, Top = 20, AutoSize = true };
            Label lblTime = new Label { Text = "Time:", Left = 20, Top = 60, AutoSize = true };

            datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Left = 100,
                Top = 20,
                Width = 200
            };

            timePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Left = 100,
                Top = 60,
                Width = 200
            };

            okBtn = new Button { Text = "OK", Left = 100, Top = 100, Width = 80 };
            cancelBtn = new Button { Text = "Cancel", Left = 200, Top = 100, Width = 80 };

            okBtn.Click += OkBtn_Click;
            cancelBtn.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblDate);
            this.Controls.Add(lblTime);
            this.Controls.Add(datePicker);
            this.Controls.Add(timePicker);
            this.Controls.Add(okBtn);
            this.Controls.Add(cancelBtn);
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            DateTime chosen = datePicker.Value.Date + timePicker.Value.TimeOfDay;

            if (chosen <= DateTime.Now)
            {
                MessageBox.Show(
                    "❌ You cannot schedule a shutdown in the past.",
                    "Invalid Time",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
