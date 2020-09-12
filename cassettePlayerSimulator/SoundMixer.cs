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
            public int position;
            public bool isPlaying;
            public bool isLooped;
            public float volume;
            public float speed;

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
                for (int i = 0; i < buffer.Length; ++i, ++position)
                {
                    if (isPlaying)
                    {
                        if (position >= wavFile.data.Length)
                        {
                            position = 0;

                            if (!isLooped)
                            {
                                isPlaying = false;
                            }
                        }

                        buffer[i] += wavFile.data[position];
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
