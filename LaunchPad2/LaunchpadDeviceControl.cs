using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IntelOrca.Launchpad;

namespace LaunchPad2
{
    public partial class LaunchpadDeviceControl : UserControl
    {
        LaunchpadButtonControl[,] buttons = new LaunchpadButtonControl[8,8];
        LaunchpadButtonControl[] sideButtons = new LaunchpadButtonControl[8];
        LaunchpadButtonControl[] toolButtons = new LaunchpadButtonControl[8];

        public LaunchpadDeviceControl()
        {
            InitializeComponent();

            AddButtonControls();
        }

        public void AddButtonControls()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    LaunchpadButtonControl control = new LaunchpadButtonControl(x, y);
                    control.Location = new Point(x,y);
                    this.tableLayoutPanel1.Controls.Add(control, x, y+1);
                    this.buttons[x, y] = control;
                    
                }
            }

            for (int y = 0; y < 8; y++)
            {
                LaunchpadButtonControl control = new LaunchpadButtonControl(0, y, ButtonType.Side);
                control.Location = new Point(9, (y + 1));
                this.tableLayoutPanel1.Controls.Add(control, 9, (y + 1));
                this.sideButtons[y] = control;
            }

            for (int x = 0; x < 8; x++)
            {
                LaunchpadButtonControl control = new LaunchpadButtonControl(x, 0, ButtonType.Toolbar);
                control.Location = new Point(x, 0);
                this.tableLayoutPanel1.Controls.Add(control, x, 0);
                this.toolButtons[x] = control;
            }
        }

        public LaunchpadButtonControl this[int x, int y]
        {
            get {
                return buttons[x,y];
            }
        }

        public LaunchpadManager Manager
        {
            set
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        this[x, y].Manager = value;

                    }

                    this.toolButtons[x].Manager = value;
                    this.sideButtons[x].Manager = value;
                }
            }
        }

        public LaunchpadButtonControl getToolbarButton(int index)
        {
            return this.toolButtons[index];
        }

        public LaunchpadButtonControl getSidebarButton(int index)
        {
            return this.sideButtons[index];
        }
    }
}
