using IntelOrca.Launchpad;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchPad2
{
    public class LaunchpadManager
    {
        LaunchpadDevice device;
        LaunchpadDeviceControl controls;

        [JsonProperty]
        LaunchpadButtonHandler[] handlers = new LaunchpadButtonHandler[64];

        AudioPlaybackEngine engine = new AudioPlaybackEngine();

        ToolMode toolButtonMode = ToolMode.StopAll;
        ToolMode sideButtonMode = ToolMode.StopAll;

        public event EventHandler LaunchpadConnected;
        public event EventHandler LaunchpadDisconnected;

        enum ToolMode
        {
            StopAll,
            PlayAll
        }

        public LaunchpadManager(LaunchpadDeviceControl controls)
        {
            this.controls = controls;

            // Setup Top toolbar

            // Setup Side toolbar

            // Setup main grid

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var handler = new LaunchpadButtonHandler(this, x, y);
                    var i = (y * 8) + x;
                    handlers[i] = handler;
                }
            }

            LoadConfig();

            UpdateToolButtons();
        }

        public void Start()
        {
            // init device
            try
            {
                this.device = new LaunchpadDevice();

                if (LaunchpadConnected != null)
                {
                    LaunchpadConnected(this, null);
                }
            }
            catch (IntelOrca.Launchpad.LaunchpadException ex)
            {
                if (LaunchpadDisconnected != null)
                {
                    LaunchpadDisconnected(this, null);
                }
            }

            controls.Manager = this;

            handlers.ToList().ForEach(x => x.Init());

            if (device != null)
            {
                device.ButtonPressed += device_ButtonPressed;
            }
        }

        void device_ButtonPressed(object sender, ButtonPressEventArgs e)
        {
            switch (e.Type)
            {
                case ButtonType.Grid:
                    handlers[(e.Y * 8) + e.X].TriggerPress();
                    break;
                case ButtonType.Side:
                    if (sideButtonMode == ToolMode.StopAll)
                    {
                        this.StopRow((int)e.SidebarButton);
                    }
                    else
                    {
                        this.PlayRow((int)e.SidebarButton);
                    }
                    break;
                case ButtonType.Toolbar:
                    if (sideButtonMode == ToolMode.PlayAll)
                    {
                        this.StopColumn((int)e.ToolbarButton);
                    }
                    else
                    {
                        this.PlayRow((int)e.ToolbarButton);
                    }
                    break;
            }
        }

        public void Stop()
        {
            handlers.ToList().ForEach(x => x.Stop());

        }



        internal LaunchpadButtonControl getControl(int x, int y)
        {
            return controls[x, y];
        }

        internal LaunchpadButton getButton(int x, int y)
        {
            if (device == null) return null;
            return device[x, y];
        }

        internal LaunchpadButtonHandler getHandler(int x, int y)
        {
            return handlers[y*8+x];
        }

        public void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(this.handlers);
            var path = @"D:\launchpad.json";
            File.WriteAllText(path, json);
        }

        public void StopRow(int y)
        {
            for (int x = 0; x < 8; x++)
            {
                var handler = getHandler(x, y);
                if (handler != null)
                {
                    handler.Stop();
                }
            }
        }

        public void StopColumn(int x)
        {
            for (int y = 0; y < 8; y++)
            {
                var handler = getHandler(x, y);
                if (handler != null)
                {
                    handler.Stop();
                }
            }
        }


        public void PlayRow(int y)
        {
            for (int x = 0; x < 8; x++)
            {
                var handler = getHandler(x, y);
                if (handler != null)
                {
                    handler.Play();
                }
            }
        }

        public void PlayColumn(int x)
        {
            for (int y = 0; y < 8; y++)
            {
                var handler = getHandler(x, y);
                if (handler != null)
                {
                    handler.Play();
                }
            }
        }

        public void LoadConfig()
        {
            var definition = new[] { new { Loop = false, Path = @"", X = -1, Y = -1 } };
            var path = @"D:\launchpad.json";
            if (!File.Exists(path))
            {
                return;
            }
            var json = File.ReadAllText(path);
            //this.handlers = JsonConvert.DeserializeObject<LaunchpadButtonHandler[]>(json);
            try
            {
                var data = JsonConvert.DeserializeAnonymousType(json, definition);

                foreach (var config in data)
                {

                    var handler = this.getHandler(config.X, config.Y);
                    handler.Load(config.Path);
                    handler.Loop = config.Loop;

                }
            }
            catch (Newtonsoft.Json.JsonSerializationException e)
            {
                Console.WriteLine(@"Could not deserialize JSON");
                Console.WriteLine(e);
                return;
            }
           
        }

        public void UpdateToolButtons()
        {
            bool[] activeRows = new bool[8];
            bool[] activeColumns = new bool[8];
            for(var x=0; x<8;x++) 
            {
                for (var y = 0; y < 8; y++)
                {
                    var handler = getHandler(x, y);
                    if (handler != null)
                    {
                        if (handler.IsPlaying)
                        {
                            activeRows[y] = true;
                            activeColumns[x] = true;
                        }
                    }
                }
            }

            for (var i = 0; i < 8; i++)
            {
                if (toolButtonMode == ToolMode.StopAll)
                {
                    SetToolbarBrightness(i, activeColumns[i] ? ButtonBrightness.Full : ButtonBrightness.Low, ButtonBrightness.Off);
                }
                else
                {
                    SetToolbarBrightness(i, ButtonBrightness.Off, activeColumns[i] ? ButtonBrightness.Full : ButtonBrightness.Medium);
                }

                if (sideButtonMode == ToolMode.StopAll)
                {
                    SetSidebarBrightness(i, activeRows[i] ? ButtonBrightness.Full : ButtonBrightness.Low, ButtonBrightness.Off);
                }
                else
                {
                    SetSidebarBrightness(i, ButtonBrightness.Off, activeColumns[i] ? ButtonBrightness.Full : ButtonBrightness.Medium);
                    //SetSidebarBrightness(i, activeRows[i] ? ButtonBrightness.Full : ButtonBrightness.Low, ButtonBrightness.Off);
                }
            }
        }

        public void SetToolbarBrightness(int index, ButtonBrightness red, ButtonBrightness green)
        {
            this.controls.getToolbarButton(index).SetBrightness(red, green);
            if (this.device != null)
            {
                this.device.GetButton((ToolbarButton)index).SetBrightness(red, green);
            }
        }
        public void SetSidebarBrightness(int index, ButtonBrightness red, ButtonBrightness green)
        {
            this.controls.getSidebarButton(index).SetBrightness(red, green);
            if (this.device != null)
            {
                this.device.GetButton((SideButton)index).SetBrightness(red, green);
            }
        }
    }
}
