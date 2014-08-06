namespace LaunchPad2
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.launchpadDeviceControl1 = new LaunchPad2.LaunchpadDeviceControl();
            this.SuspendLayout();
            // 
            // launchpadDeviceControl1
            // 
            this.launchpadDeviceControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.launchpadDeviceControl1.Location = new System.Drawing.Point(0, 0);
            this.launchpadDeviceControl1.Name = "launchpadDeviceControl1";
            this.launchpadDeviceControl1.Size = new System.Drawing.Size(950, 559);
            this.launchpadDeviceControl1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 559);
            this.Controls.Add(this.launchpadDeviceControl1);
            this.Name = "MainForm";
            this.Text = "LaunchPad";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private LaunchpadDeviceControl launchpadDeviceControl1;
    }
}

