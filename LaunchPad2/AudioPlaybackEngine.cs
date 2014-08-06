using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaunchPad2
{
    class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;
        private readonly WaveFormat waveFormat;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            outputDevice = new WaveOutEvent();
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
            mixer = new MixingSampleProvider(waveFormat);
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public CachedSoundSampleProvider PlaySound(CachedSound sound)
        {
            var provider = new CachedSoundSampleProvider(sound);
            AddMixerInput(provider);
            return provider;
        }

        private ISampleProvider AddMixerInput(ISampleProvider input)
        {
            //var stream = new WaveFormatConversionStream(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2), input);
            var resampler = new WdlResamplingSampleProvider(input, 44100);
            var provider = ConvertToRightChannelCount(resampler);
            mixer.AddMixerInput(provider);
            return provider;
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }
}
