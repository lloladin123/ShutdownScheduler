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
            this.Text = "Schedule One-Time Shutdown";
            this.Width = 400;
            this.Height = 220;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 2, Padding = new Padding(10) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            // Date
            layout.Controls.Add(new Label { Text = "Date:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 0);
            datePicker = new DateTimePicker { Format = DateTimePickerFormat.Short, Dock = DockStyle.Fill };
            layout.Controls.Add(datePicker, 1, 0);

            // Time
            layout.Controls.Add(new Label { Text = "Time:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 1);
            timePicker = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Dock = DockStyle.Fill };
            layout.Controls.Add(timePicker, 1, 1);

            // Buttons
            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            okBtn = new Button { Text = "Add", Width = 80, Height = 40 };
            cancelBtn = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 80, Height = 40 };

            okBtn.Click += (s, e) =>
            {
                DateTime target = datePicker.Value.Date + timePicker.Value.TimeOfDay;
                if (target <= DateTime.Now)
                {
                    MessageBox.Show("⚠️ The selected date/time has already passed.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.DialogResult = DialogResult.OK;
            };

            buttonPanel.Controls.Add(okBtn);
            buttonPanel.Controls.Add(cancelBtn);
            layout.Controls.Add(buttonPanel, 1, 2);

            this.Controls.Add(layout);
        }
    }
}
