using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class WAVFile
    {
        public int audioFormat = 0;
        public int channels = 0;
        public int sampleRate = 0;
        public int blockAlign = 0;
        public int bitsPerSample = 0;
        public int dataLength = 0;

        public short[] data;

        private static WAVFile Load(Stream stream, IProgress<float> progress = null)
        {
            var wav = new WAVFile();

            using (BinaryReader reader = new BinaryReader(stream))
            {
                if (reader.BaseStream.Length < 44)
                {
                    throw new ArgumentException("Invalid WAV file");
                }

                var magic1 = reader.ReadBytes(4);
                int chunkSize = reader.ReadInt32();
                var magic2 = reader.ReadBytes(8);
                int subchunkSize = reader.ReadInt32();
                wav.audioFormat = reader.ReadInt16();
                wav.channels = reader.ReadInt16();
                wav.sampleRate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                wav.blockAlign = reader.ReadInt16();
                wav.bitsPerSample = reader.ReadInt16();

                reader.BaseStream.Seek(20 + subchunkSize, SeekOrigin.Begin);

                var magic3 = reader.ReadBytes(4);
                wav.dataLength = reader.ReadInt32();

                if (Encoding.ASCII.GetString(magic1) != "RIFF"
                    || Encoding.ASCII.GetString(magic2) != "WAVEfmt "
                    || Encoding.ASCII.GetString(magic3) != "data"
                    || chunkSize != reader.BaseStream.Length - 8
                    )
                {
                    throw new ArgumentException("Invalid WAV file");
                }

                if (wav.audioFormat != 1
                    || wav.channels < 1
                    || wav.channels > 2
                    || wav.bitsPerSample != 16)
                {
                    throw new NotImplementedException("Unsupported WAV format");
                }

                wav.data = new short[wav.dataLength / 2];

                int totalBytes = wav.data.Length * 2;

                int step = 0;
                int stepCount = 100;
                int stepSize = totalBytes / stepCount;
                int nextStep = 0;

                int bufSize = 1024 * 128;
                byte[] buffer = new byte[bufSize];

                for (int i = 0; i < totalBytes;)
                {
                    int blockBytes = Math.Min(bufSize, totalBytes - i);
                    reader.BaseStream.Read(buffer, 0, blockBytes);
                    Buffer.BlockCopy(buffer, 0, wav.data, i, blockBytes);

                    if (progress != null && i >= nextStep)
                    {
                        nextStep += stepSize;
                        progress.Report(step / (float)stepCount);
                        step++;
                    }

                    i += blockBytes;
                }
            }

            return wav;
        }

        public static WAVFile Load(System.IO.UnmanagedMemoryStream stream)
        {
            return Load(stream as Stream);
        }

        public static WAVFile Load(string filename, IProgress<float> progress = null)
        {
            using (var file = File.OpenRead(filename))
            {
                return Load(file, progress);
            }
        }
    }
}
