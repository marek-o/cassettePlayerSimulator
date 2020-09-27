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
            private bool isPlaying;
            private bool isLooped;
            private float volume;
            private float speed;

            private float destinationSpeed;
            private int rampSampleCounter;
            private float rampSlope;

            public object locker = new object();

            public Sample(WAVFile wavFile, bool isPlaying, bool isLooped, float volume, float speed)
            {
                this.wavFile = wavFile;
                position = 0;

                UpdatePlayback(isPlaying, isLooped, volume, speed);
            }

            private short Clamp(double sample)
            {
                if (sample > 32767.0)
                {
                    sample = 32767.0;
                }
                else if (sample < -32768.0)
                {
                    sample = -32768.0;
                }

                return (short)sample;
            }

            public float GetCurrentPositionSeconds()
            {
                return (float)(position / wavFile.sampleRate / wavFile.channels);
            }

            public void UpdatePlayback(bool isPlaying, bool isLooped, float volume, float speed)
            {
                lock (locker)
                {
                    this.isPlaying = isPlaying;
                    this.isLooped = isLooped;
                    this.volume = volume;
                    this.speed = speed;
                }
            }

            public void UpdatePlayback(bool isPlaying)
            {
                lock (locker)
                {
                    this.isPlaying = isPlaying;
                }
            }

            public void RampSpeed(float startingSpeed, float destinationSpeed, int sampleCount)
            {
                lock (locker)
                {
                    this.speed = startingSpeed;
                    this.destinationSpeed = destinationSpeed;
                    rampSampleCounter = sampleCount;
                    rampSlope = (destinationSpeed - startingSpeed) / sampleCount;
                }
            }

            public void PlayIntoBuffer(short[] buffer)
            {
                lock (locker)
                {
                    for (int i = 0; i < buffer.Length; ++i)
                    {
                        if (isPlaying)
                        {
                            if (rampSampleCounter > 0)
                            {
                                speed += rampSlope;
                                --rampSampleCounter;

                                if (rampSampleCounter == 0)
                                {
                                    speed = destinationSpeed;
                                }
                            }

                            var samp1 = wavFile.data[(int)position];
                            var samp2 = wavFile.data[(int)position + 2]; //next sample from this channel
                            var ratio = position - Math.Floor(position);
                            var interpolatedSample = samp1 * (1 - ratio) + samp2 * ratio;

                            buffer[i] += Clamp(interpolatedSample * volume);
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
                sample.UpdatePlayback(false);

                lock (sample.locker) //FIXME another lock for playing
                {
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
