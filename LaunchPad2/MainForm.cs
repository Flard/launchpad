﻿using IntelOrca.Launchpad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaunchPad2
{
    public partial class MainForm : Form
    {
        LaunchpadDevice device;
        LaunchpadManager manager;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // init manager
            manager = new LaunchpadManager(this.launchpadDeviceControl1);

            manager.LaunchpadConnected += manager_LaunchpadConnected;
            manager.LaunchpadDisconnected += manager_LaunchpadDisconnected;

            manager.Start();
        }

        void manager_LaunchpadDisconnected(object sender, EventArgs e)
        {
            tsbConnect.Checked = false;
        }

        void manager_LaunchpadConnected(object sender, EventArgs e)
        {
            tsbConnect.Checked = true;
        }

        private void tsbConnect_Click(object sender, EventArgs e)
        {
            manager.Start();
        }
    }
}
