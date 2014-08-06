using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchPad2
{
    class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public bool IsStopped { get; private set; }

        public CachedSound(string audioFileName)
        {
            IsStopped = false;

            using (var audioFileReader = new AudioFileReader(audioFileName))
            {
                // TODO: could add resampling in here if required
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }

        public event EventHandler FinishedPlayback;

        public void TriggerEnd()
        {
            if (FinishedPlayback != null)
                FinishedPlayback(this, null);
        }

        public bool Loop { get; set; }

        public void Stop()
        {
            if (IsPlaying)
            {
                this.IsStopped = true;
            }
        }

        public bool IsPlaying
        {
            get;
            set;
        }
    }
}
