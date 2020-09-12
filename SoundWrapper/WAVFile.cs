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
        int audioFormat = 0;
        int channels = 0;
        int sampleRate = 0;
        int blockAlign = 0;
        int bitsPerSample = 0;
        int dataLength = 0;

        public short[] data;

        public static WAVFile Load(string filename)
        {
            var wav = new WAVFile();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
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
                var magic3 = reader.ReadBytes(4);
                wav.dataLength = reader.ReadInt32();

                if (   Encoding.ASCII.GetString(magic1) != "RIFF"
                    || Encoding.ASCII.GetString(magic2) != "WAVEfmt "
                    || Encoding.ASCII.GetString(magic3) != "data"
                    || chunkSize != reader.BaseStream.Length - 8
                    || subchunkSize != 16
                    || wav.dataLength != reader.BaseStream.Length - 44
                    )
                {
                    throw new ArgumentException("Invalid WAV file");
                }

                if (   wav.audioFormat != 1
                    || wav.channels < 1
                    || wav.channels > 2
                    || wav.bitsPerSample != 16)
                {
                    throw new NotImplementedException("Unsupported WAV format");
                }

                wav.data = new short[wav.dataLength / 2];

                for (int i = 0; i < wav.data.Length; ++i)
                {
                    wav.data[i] = reader.ReadInt16();
                }
            }

            return wav;
        }
    }
}
