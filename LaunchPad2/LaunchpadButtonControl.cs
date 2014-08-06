using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using IntelOrca.Launchpad;

namespace LaunchPad2
{
    public partial class LaunchpadButtonControl : UserControl
    {
        int x, y;
        LaunchpadManager manager;

        public ButtonType ButtonType { get; internal set; }
        
        public LaunchpadButtonControl(int x, int y, ButtonType type)
        {
            InitializeComponent();

            this.x = x;
            this.y = y;

            this.ButtonType = type;
        }

        public LaunchpadButtonControl(int x, int y)
            :this(x,y,ButtonType.Grid)
        {
        }

        public LaunchpadManager Manager
        {
            get { return manager; }
            set
            {
                manager = value;

                if (this.ButtonType == ButtonType.Grid)
                {
                    handler = this.Manager.getHandler(x, y);
                    handler.PathChanged += Handler_PathChanged;
                    Handler_PathChanged(null, null);
                }
                
            }
        }

        void Handler_PathChanged(object sender, EventArgs e)
        {
            this.label1.Text = System.IO.Path.GetFileNameWithoutExtension(Handler.Path);
        }

        public void SetBrightness(ButtonBrightness red, ButtonBrightness green)
        {
            var color = Color.FromArgb(Brightness2Hex(red), Brightness2Hex(green), 0);
            this.BackColor = color;
        }

        private byte Brightness2Hex(ButtonBrightness brightness)
        {
            switch (brightness)
            {
                case ButtonBrightness.Off:
                default:
                    return 0;
                case ButtonBrightness.Low:
                    return 64;
                case ButtonBrightness.Medium:
                    return 160;
                case ButtonBrightness.Full:
                    return 255;
            }
        }

        private void LaunchpadButtonControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.ButtonType == ButtonType.Grid)
            {
                contextMenuStrip1.Show(this, e.Location);
            }
            else
            {
                if (this.ButtonType == ButtonType.Grid)
                {
                    this.Manager.getHandler(x, y).TriggerPress();
                }
                else if (this.ButtonType == ButtonType.Side)
                {
                    this.Manager.StopRow(y);
                }
                else if (this.ButtonType == ButtonType.Toolbar)
                {
                    this.Manager.StopColumn(y);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var handler = this.Manager.getHandler(x, y);
            this.loopToolStripMenuItem.Checked = handler.Loop && handler.HasFile;
            this.loopToolStripMenuItem.Enabled = handler.HasFile;
            this.clearToolStripMenuItem.Enabled = handler.HasFile;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Handler.SetBrightness(ButtonBrightness.Full, ButtonBrightness.Off);

            var win = new OpenFileDialog();
            var res = win.ShowDialog();

            if (res == DialogResult.OK)
            {
                var path = win.FileName;
                Handler.Load(path);
            }
            else
            {
                Handler.ResetBrightness();
            }
        }

        LaunchpadButtonHandler handler;
        public LaunchpadButtonHandler Handler
        {
            get
            {
                return handler;
            }
        }

        private void loopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.loopToolStripMenuItem.Checked = handler.Loop && handler.HasFile;
            this.Handler.Loop = !this.Handler.Loop;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Handler.Clear();
        }
    }
}
