using NAudio.Wave;
using System;
using System.Collections.Generic;
using Utils;

namespace cassettePlayerSimulator
{
    class SoundMixer : IWaveProvider
    {
        public class Sample
        {
            public WAVFile wavFile;
            private double position;
            private bool isPlaying;
            private bool isRecording;
            private bool isLooped;
            private bool autoRewind;
            private float volume;
            private float speed;

            private float destinationSpeed;
            private int rampSampleCounter;
            private float rampSlope;
            private float leadInOutLength;

            private object locker = new object();

            public Sample(WAVFile wavFile, bool isPlaying, bool isLooped, bool autoRewind, float volume, float speed)
            {
                this.wavFile = wavFile;
                position = 0;

                this.isPlaying = isPlaying;
                this.isLooped = isLooped;
                this.autoRewind = autoRewind;
                this.volume = volume;
                this.speed = speed;
            }

            public void SetVolume(float vol)
            {
                lock (locker)
                {
                    volume = vol;
                }
            }

            public void SetLeadInOutLengthSeconds(float val)
            {
                lock (locker)
                {
                    leadInOutLength = val * wavFile.format.SampleRate * wavFile.format.Channels;
                }
            }

            private short Clamp(double sample)
            {
                if (sample > short.MaxValue)
                {
                    sample = short.MaxValue;
                }
                else if (sample < short.MinValue)
                {
                    sample = short.MinValue;
                }

                return (short)sample;
            }

            private int LastSafePosition()
            {
                return (wavFile.dataLengthBytes / 2) - 4;
            }

            public bool IsAtBeginningOrEnd()
            {
                return position == 0 || position == LastSafePosition();
            }

            public float GetCurrentPositionSeconds()
            {
                return (float)(position / wavFile.format.SampleRate / wavFile.format.Channels);
            }

            public void SetCurrentPositionSeconds(float seconds)
            {
                lock (locker)
                {
                    position = (int)(seconds * wavFile.format.SampleRate * wavFile.format.Channels);

                    position = Math.Max(0, Math.Min(LastSafePosition(), position));
                }
            }

            public float GetLengthSeconds()
            {
                return (float)(wavFile.dataLengthBytes / 2) / wavFile.format.SampleRate / wavFile.format.Channels;
            }

            public void UpdatePlayback(bool isPlaying)
            {
                lock (locker)
                {
                    this.isPlaying = isPlaying;
                }
            }

            public void UpdateRecording(bool isRecording)
            {
                lock (locker)
                {
                    if (this.isRecording != isRecording)
                    {
                        this.isRecording = isRecording;
                    }
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

            internal void PlayIntoBuffer(short[] buffer)
            {
                lock (locker)
                {
                    for (int i = 0; i < buffer.Length; i += 2)
                    {
                        if (isPlaying)
                        {
                            if (rampSampleCounter > 0)
                            {
                                speed += rampSlope * 2;
                                rampSampleCounter -= 2;

                                if (rampSampleCounter <= 0)
                                {
                                    speed = destinationSpeed;
                                }
                            }

                            int intPos = (int)position;
                            int intPosL = intPos & ~0x1;
                            int intPosR = intPos | 0x1;
                            var ratio = position - intPosL; //in range 0-2 because of interleaved samples

                            if (position >= leadInOutLength
                                && position < LastSafePosition() - leadInOutLength)
                            {
                                //left channel
                                var samp1L = wavFile.ReadSample(intPosL);
                                var samp2L = wavFile.ReadSample(intPosL + 2);
                                var interpolatedSampleL = samp1L * (2 - ratio) + samp2L * ratio;
                                buffer[i] += Clamp(interpolatedSampleL * volume * speed);

                                //right channel
                                var samp1R = wavFile.ReadSample(intPosR);
                                var samp2R = wavFile.ReadSample(intPosR + 2);
                                var interpolatedSampleR = samp1R * (2 - ratio) + samp2R * ratio;
                                buffer[i + 1] += Clamp(interpolatedSampleR * volume * speed);
                            }

                            position += speed * 2;

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

            internal void RecordFromBuffer(short[] buffer)
            {
                lock (locker)
                {
                    if (isRecording)
                    {
                        if (position + buffer.Length < LastSafePosition())
                        {
                            wavFile.WriteSamples(buffer, (int)position);

                            position += buffer.Length;
                        }
                        else
                        {
                            position = LastSafePosition();
                        }
                    }
                }
            }
        }

        private WaveOut player;
        private WaveIn recorder;

        private List<Sample> samples = new List<Sample>();
        private Sample recordingSample;

        private object playingLocker = new object();

        public WaveFormat WaveFormat => Common.WaveFormat;

        public SoundMixer()
        {
            player = new WaveOut();
            player.DesiredLatency = 40;
            player.NumberOfBuffers = 3;
            player.Init(this);
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

        public void SetRecordingSample(Sample sample)
        {
            recordingSample = sample;
        }

        public void Start()
        {
            player.Play();
        }

        public void Stop()
        {
            player.Stop();
        }

        public void StartRecording()
        {
            if (recorder == null)
            {
                //sometimes it doesn't record when reusing same object
                recorder = new WaveIn();
                recorder.WaveFormat = Common.WaveFormat;
                recorder.DataAvailable += Recorder_NewDataPresent;
                recorder.BufferMilliseconds = 40;
                recorder.NumberOfBuffers = 3;
                recorder.StartRecording();
            }
        }

        public void StopRecording()
        {
            if (recorder != null)
            {
                recorder.StopRecording();
                recorder.Dispose();
                recorder = null;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            lock (playingLocker)
            {
                short[] shortBuf = new short[count / 2];

                foreach (var sample in samples)
                {
                    sample.PlayIntoBuffer(shortBuf);
                }

                Buffer.BlockCopy(shortBuf, 0, buffer, offset, count);
            }
            return count;
        }

        private void Recorder_NewDataPresent(object sender, WaveInEventArgs e)
        {
            if (recordingSample != null)
            {
                short[] shortBuf = new short[e.BytesRecorded / 2];
                Buffer.BlockCopy(e.Buffer, 0, shortBuf, 0, e.BytesRecorded);
                recordingSample.RecordFromBuffer(shortBuf);
            }
        }
    }
}
