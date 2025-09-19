using System;
using System.Windows.Forms;

namespace ShutdownScheduler.Forms.Prompts
{
    public class WeeklyPrompt : Form
    {
        private ComboBox dayBox;
        private DateTimePicker timePicker;
        private Button okBtn;
        private Button cancelBtn;

        public string SelectedDay => dayBox.SelectedItem?.ToString() ?? "";
        public string SelectedTime => timePicker.Value.ToString("HH:mm");

        public WeeklyPrompt()
        {
            this.Text = "Schedule Weekly Shutdown";
            this.Width = 350;
            this.Height = 250;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 2, Padding = new Padding(10) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            // Day
            layout.Controls.Add(new Label { Text = "Day:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 0);
            dayBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            dayBox.Items.AddRange(new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" });
            dayBox.SelectedIndex = 0;
            layout.Controls.Add(dayBox, 1, 0);

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
                if (string.IsNullOrEmpty(SelectedDay))
                {
                    MessageBox.Show("⚠️ Please select a valid day.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(SelectedTime))
                {
                    MessageBox.Show("⚠️ Please select a valid time.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
