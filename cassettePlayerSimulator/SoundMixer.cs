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
            private bool autoRewind;
            private float volume;
            private float speed;

            private float destinationSpeed;
            private int rampSampleCounter;
            private float rampSlope;

            public object locker = new object();

            public Sample(WAVFile wavFile, bool isPlaying, bool isLooped, bool autoRewind, float volume, float speed)
            {
                this.wavFile = wavFile;
                position = 0;

                UpdatePlayback(isPlaying, isLooped, autoRewind, volume, speed);
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

            private int LastSafePosition()
            {
                return wavFile.data.Length - 3;
            }

            public float GetCurrentPositionSeconds()
            {
                return (float)(position / wavFile.sampleRate / wavFile.channels);
            }

            public void SetCurrentPositionSeconds(float seconds)
            {
                lock (locker)
                {
                    position = (int)(seconds * wavFile.sampleRate * wavFile.channels);

                    position = Math.Max(0, Math.Min(LastSafePosition(), position));
                }
            }

            public void UpdatePlayback(bool isPlaying, bool isLooped, bool autoRewind, float volume, float speed)
            {
                lock (locker)
                {
                    this.isPlaying = isPlaying;
                    this.isLooped = isLooped;
                    this.autoRewind = autoRewind;
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

                            buffer[i] += Clamp(interpolatedSample * volume * speed);
                            position += speed;

                            if (speed == 0.0f)
                            {
                                isPlaying = false;
                            }

                            if (position > LastSafePosition())
                            {
                                position = autoRewind ? 0 : LastSafePosition();

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

        private object playingLocker = new object();

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

                lock (playingLocker)
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
            lock (playingLocker)
            {
                foreach (var sample in samples)
                {
                    sample.PlayIntoBuffer(e.data);
                }
            }
        }
    }

}
