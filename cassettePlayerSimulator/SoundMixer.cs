using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace cassettePlayerSimulator
{
    class SoundMixer
    {
        public class Sample
        {
            public WAVFile wavFile;
            public double position;
            public bool isPlaying;
            public bool isLooped;
            public float volume;
            public float speed;

            public object locker = new object();

            public Sample(WAVFile wavFile, bool isPlaying, bool isLooped, float volume, float speed)
            {
                this.wavFile = wavFile;
                position = 0;
                this.isPlaying = isPlaying;
                this.isLooped = isLooped;
                this.volume = volume;
                this.speed = speed;
            }

            public void PlayIntoBuffer(short[] buffer)
            {
                lock (locker)
                {
                    for (int i = 0; i < buffer.Length; ++i)
                    {
                        if (isPlaying)
                        {
                            var samp1 = wavFile.data[(int)position];
                            var samp2 = wavFile.data[(int)position + 2]; //next sample from this channel
                            var ratio = position - Math.Floor(position);
                            var interpolatedSample = samp1 * (1 - ratio) + samp2 * ratio;

                            buffer[i] += (short)(interpolatedSample * volume);
                            position += speed;

                            if (position >= wavFile.data.Length - 2)
                            {
                                position = 0;

                                if (!isLooped)
                                {
                                    isPlaying = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        SoundWrapper player;
        List<Sample> samples = new List<Sample>();

        public SoundMixer(ushort bitsPerSample, ushort channels, uint sampleRate, uint bufferLengthBytes)
        {
            player = new SoundWrapper(SoundWrapper.Mode.Play, bitsPerSample, channels, sampleRate, bufferLengthBytes);
            player.NewDataRequested += Player_NewDataRequested;
        }

        public void AddSample(Sample sample)
        {
            samples.Add(sample);
        }

        public void RemoveSample(Sample sample)
        {
            if (samples.Contains(sample))
            {
                lock (sample.locker) //FIXME another lock for playing
                {
                    sample.isPlaying = false;
                    samples.Remove(sample);
                }
            }
        }

        public void Start()
        {
            player.Start(0);
        }

        private void Player_NewDataRequested(object sender, Utils.SoundWrapper.NewDataEventArgs e)
        {
            foreach (var sample in samples)
            {
                sample.PlayIntoBuffer(e.data);
            }
        }
    }

}
