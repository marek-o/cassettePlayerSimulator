﻿using System;
using System.Collections.Generic;
using Utils;

namespace cassettePlayerSimulator
{
    class SoundMixer
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

            public float GetLengthSeconds()
            {
                return (float)(wavFile.dataLengthBytes / 2) / wavFile.sampleRate / wavFile.channels;
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

        private SoundWrapper player;
        private SoundWrapper recorder;
        private List<Sample> samples = new List<Sample>();
        private Sample recordingSample;

        private object playingLocker = new object();

        public SoundMixer(ushort bitsPerSample, ushort channels, uint sampleRate, uint bufferLengthBytes)
        {
            player = new SoundWrapper(SoundWrapper.Mode.Play, bitsPerSample, channels, sampleRate, bufferLengthBytes);
            player.NewDataRequested += Player_NewDataRequested;
            
            recorder = new SoundWrapper(SoundWrapper.Mode.Record, bitsPerSample, channels, sampleRate, bufferLengthBytes);
            recorder.NewDataPresent += Recorder_NewDataPresent;
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
            player.Start(0);
        }

        public void Stop()
        {
            player.Stop();
        }

        public void StartRecording()
        {
            recorder.Start(0);
        }

        public void StopRecording()
        {
            recorder.Stop();
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

        private void Recorder_NewDataPresent(object sender, SoundWrapper.NewDataEventArgs e)
        {
            if (recordingSample != null)
            {
                recordingSample.RecordFromBuffer(e.data);
            }
        }
    }
}
