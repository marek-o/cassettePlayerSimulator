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
        public int dataLengthBytes = 0;

        public int dataOffsetBytes = 0;

        public string filename;

        private Stream stream;
        private BinaryReader reader;
        public BinaryWriter writer;

        private short[] sampleCache = new short[0];
        private long sampleCacheOffset = 0;

        //to be removed
        public void OpenForWriting()
        {
            if (writer == null)
            {
                writer = new BinaryWriter(stream);
            }
        }

        //to be removed
        public void CloseForWriting()
        {
        }

        public void WriteSamples(short[] buffer, int position)
        {
            if (writer == null)
            {
                return;
            }

            byte[] bytes = new byte[buffer.Length * 2];
            Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);

            writer.Seek(dataOffsetBytes + position * 2, SeekOrigin.Begin);
            writer.Write(bytes, 0, bytes.Length);

            //FIXME clear read cache
        }

        public short ReadSample(int position)
        {
            if (!(position >= sampleCacheOffset && position < sampleCacheOffset + sampleCache.Length))
            {
                reader.BaseStream.Seek(dataOffsetBytes + position * 2, SeekOrigin.Begin);

                const int sampleCacheSize = 1024;
                byte[] tmp = new byte[sampleCacheSize * 2];
                sampleCache = new short[sampleCacheSize];
                reader.Read(tmp, 0, tmp.Length);

                Buffer.BlockCopy(tmp, 0, sampleCache, 0, tmp.Length);

                sampleCacheOffset = position;
            }

            return sampleCache[position - sampleCacheOffset];
        }

        private static WAVFile Load(string filename, Stream stream)
        {
            var wav = new WAVFile();
            wav.filename = filename;
            wav.stream = stream;

            BinaryReader reader = new BinaryReader(stream);
            wav.reader = reader;

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
            wav.dataLengthBytes = reader.ReadInt32();

            wav.dataOffsetBytes = (int)reader.BaseStream.Position;

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

            return wav;
        }

        public static WAVFile Load(System.IO.UnmanagedMemoryStream stream)
        {
            return Load("", stream as Stream);
        }

        public static WAVFile Load(string filename)
        {
            var file = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
            return Load(filename, file);
        }

        public void Close()
        {
            writer?.Close();
            writer = null;

            reader?.Close();
            reader = null;

            stream?.Close();
            stream = null;
        }
    }
}
