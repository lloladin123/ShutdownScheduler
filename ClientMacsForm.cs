using System;
using System.Linq;
using System.Windows.Forms;
using ShutdownScheduler.Core;

namespace ShutdownScheduler.Forms
{
    public class ClientMacsForm : Form
    {
        private ListBox listBox;
        private TextBox input;
        private Button addBtn;
        private Button removeBtn;
        private Button okBtn;
        private readonly AppConfig _config;

        public ClientMacsForm(AppConfig config)
        {
            _config = config;

            this.Text = "Client MAC Addresses";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(10)
            };

            listBox = new ListBox { Dock = DockStyle.Fill };
            listBox.Items.AddRange(_config.ClientMacs.ToArray());

            input = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Enter MAC (e.g. 00:11:22:33:44:55)" };

            addBtn = new Button { Text = "Add", Dock = DockStyle.Right, Width = 80 };
            removeBtn = new Button { Text = "Remove Selected", Dock = DockStyle.Right, Width = 120 };
            okBtn = new Button { Text = "OK", Dock = DockStyle.Bottom, DialogResult = DialogResult.OK };

            addBtn.Click += (s, e) =>
            {
                var mac = input.Text.Trim();
                if (!string.IsNullOrEmpty(mac) && !_config.ClientMacs.Contains(mac))
                {
                    _config.ClientMacs.Add(mac);
                    listBox.Items.Add(mac);
                    input.Clear();
                }
            };

            removeBtn.Click += (s, e) =>
            {
                if (listBox.SelectedItem is string selected)
                {
                    _config.ClientMacs.Remove(selected);
                    listBox.Items.Remove(selected);
                }
            };

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            buttonPanel.Controls.Add(addBtn);
            buttonPanel.Controls.Add(removeBtn);

            layout.Controls.Add(listBox, 0, 0);
            layout.Controls.Add(input, 0, 1);
            layout.Controls.Add(buttonPanel, 0, 2);
            layout.Controls.Add(okBtn, 0, 3);

            this.Controls.Add(layout);
        }
    }
}
