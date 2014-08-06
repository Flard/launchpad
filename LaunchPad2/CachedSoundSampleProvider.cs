using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchPad2
{
    class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;
        private bool stopped;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public long Position {
            get { return this.position; }
        }

        public bool IsPlaying
        {
            get { return !stopped && this.position < cachedSound.AudioData.Length; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            cachedSound.IsPlaying = true;
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;

            if (stopped)
            {
                samplesToCopy = 0;
            }

            if (samplesToCopy == 0)
            {
                cachedSound.IsPlaying = false;
                cachedSound.TriggerEnd();
            }
            return (int)samplesToCopy;
        }

        public void Stop()
        {
            this.stopped = true;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }
}
