using IntelOrca.Launchpad;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace LaunchPad2
{
    public class LaunchpadButtonHandler : IDisposable
    {
        LaunchpadManager manager;
        LaunchpadButtonControl control;
        LaunchpadButton button;
        int x, y;

        readonly bool[] red = {
                                    true, true, true, true,false,false,false,false,
                                    true,false,false,true,false, true, true,false,
                                    true,false,false,true,false, true, true,false,
                                    true, true, true, true,false,false,false,false,
                                    false,false,false,false,true,true,true,true,
                                    false,true,true,false,true,false,false,true,
                                    false,true,true,false,true,false,false,true,
                                    false,false,false,false,true,true,true,true,
                                 };

        public event EventHandler PathChanged;

        CachedSound sound;
        CachedSoundSampleProvider soundProvider;
        string path;
        bool loop;

        public LaunchpadButtonHandler(LaunchpadManager manager, int x, int y)
        {
            this.manager = manager;
            this.control = manager.getControl(x, y);
            this.button = manager.getButton(x, y);

            this.x = x;
            this.y = y;
        }

        public void Init()
        {
            ResetBrightness();
            this.control = manager.getControl(x, y);
            this.button = manager.getButton(x, y);
        }

        public void Load(string path)
        {
            SetBrightness(ButtonBrightness.Medium, ButtonBrightness.Off);

            if (!String.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                sound = new CachedSound(path);
                sound.FinishedPlayback += sound_FinishedPlayback;
            }
            else
            {
                sound = null;
            }

            this.path = path;

            if (this.PathChanged != null)
            {
                this.PathChanged(this, null);
            }

            manager.SaveConfig();

            ResetBrightness();
        }

        void sound_FinishedPlayback(object sender, EventArgs e)
        {
            ResetBrightness();
            manager.UpdateToolButtons();
        }

        void player_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SetBrightness(ButtonBrightness.Full, ButtonBrightness.Full);
        }

        public void Stop()
        {
            //SetBrightness(ButtonBrightness.Off, ButtonBrightness.Off);
            if (soundProvider != null)
            {
                soundProvider.Stop();
            }
            ResetBrightness();
            manager.UpdateToolButtons();

        }

        public void SetBrightness(ButtonBrightness red, ButtonBrightness green)
        {
            if (button != null) button.SetBrightness(red, green);
            if (control != null) control.SetBrightness(red, green);
        }

        internal void TriggerPress()
        {
            if (HasFile)
            {
                Stop();
                Play();
            }
        }

        public void Play()
        {
            SetBrightness(ButtonBrightness.Off, ButtonBrightness.Full);

            this.soundProvider = AudioPlaybackEngine.Instance.PlaySound(sound);

            manager.UpdateToolButtons();

        }

        public void ResetBrightness()
        {
            if (HasFile)
            {
                if (IsPlaying)
                {
                    SetBrightness(ButtonBrightness.Off, ButtonBrightness.Full);
                }
                else
                {
                    int i = this.Y * 8 + this.X;
                    SetBrightness(ButtonBrightness.Full, red[i] ? ButtonBrightness.Full : ButtonBrightness.Low);
                }
            }
            else
            {
                SetBrightness(ButtonBrightness.Off, ButtonBrightness.Off);
            }

        }

        public bool Loop
        {
            get { return loop; }
            set { 
                loop = value; 
                ResetBrightness();

                if (sound != null)
                {
                    sound.Loop = value;
                }
            }
        }

        [JsonIgnore]
        public bool HasFile
        {
            get
            {
                return !String.IsNullOrEmpty(path) && System.IO.File.Exists(path);
            }
        }

        [JsonIgnore]
        public bool IsPlaying
        {
            get
            {
                if (soundProvider != null)
                {
                    return soundProvider.IsPlaying;
                }
                else
                {
                    return false;
                }
            }
        }

        public string Path
        {
            get { return path; }
        }

        public void Clear()
        {
            path = null;
            Loop = false;
            ResetBrightness();
        }

        public int X
        {
            get { return this.x; }
        }

        public int Y
        {
            get { return this.y; }
        }

        public void Dispose()
        {
            this.button.TurnOffLight();
        }
    }
}
