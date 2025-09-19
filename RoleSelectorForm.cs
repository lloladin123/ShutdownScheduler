using System;
using System.Windows.Forms;
using ShutdownScheduler.Core;

namespace ShutdownScheduler.Forms
{
    public class RoleSelectorForm : Form
    {
        private ComboBox roleBox;
        private TextBox pathBox;
        private Button okBtn;
        private Button cancelBtn;

        private readonly AppConfig _config;

        public RoleSelectorForm(AppConfig config)
        {
            _config = config;
            this.Text = "Select Role";
            this.Width = 400;
            this.Height = 200;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            // Role
            layout.Controls.Add(new Label { Text = "Role:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 0);
            roleBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            roleBox.Items.AddRange(Enum.GetNames(typeof(NodeRole)));
            roleBox.SelectedItem = _config.Role.ToString();
            layout.Controls.Add(roleBox, 1, 0);

            // Shared path
            layout.Controls.Add(new Label { Text = "Shared Path:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 1);
            pathBox = new TextBox { Dock = DockStyle.Fill, Text = _config.SharedPath };
            layout.Controls.Add(pathBox, 1, 1);

            // Buttons
            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft };
            okBtn = new Button { Text = "Save", DialogResult = DialogResult.OK, Width = 80, Height = 35 };
            cancelBtn = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 80, Height = 35 };
            buttonPanel.Controls.Add(okBtn);
            buttonPanel.Controls.Add(cancelBtn);
            layout.Controls.Add(buttonPanel, 1, 2);

            this.Controls.Add(layout);

            okBtn.Click += (s, e) =>
            {
                _config.Role = Enum.Parse<NodeRole>(roleBox.SelectedItem.ToString()!);
                _config.SharedPath = pathBox.Text;
                _config.Save();
                this.Close();
            };
        }
    }
}
