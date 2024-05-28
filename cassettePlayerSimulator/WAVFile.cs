using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils
{
    public class WAVFile
    {
        public WaveFormat format;
        public int dataLengthBytes = 0;

        public int dataOffsetBytes = 0;

        public string filename;

        public Stream stream;

        public bool isValid = false;

        private short[] sampleCache = new short[0];
        private long sampleCacheOffset = 0;

        public void WriteSamples(short[] buffer, int position)
        {
            byte[] bytes = new byte[buffer.Length * 2];
            Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);

            stream.Seek(dataOffsetBytes + position * 2, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);

            sampleCache = new short[0];
            sampleCacheOffset = -1;
        }

        public short ReadSample(int position)
        {
            if (stream == null)
            {
                return 0;
            }

            if (!(position >= sampleCacheOffset && position < sampleCacheOffset + sampleCache.Length))
            {
                stream.Seek(dataOffsetBytes + position * 2, SeekOrigin.Begin);

                const int sampleCacheSize = 1024;
                byte[] tmp = new byte[sampleCacheSize * 2];
                sampleCache = new short[sampleCacheSize];

                //FIXME retry read when not all is read?
                stream.Read(tmp, 0, tmp.Length);

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

            WaveFileReader waveReader = null;

            try
            {
                int subchunkSize = 0;
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    stream.Seek(16, SeekOrigin.Begin);
                    subchunkSize = reader.ReadInt32();
                    stream.Seek(0, SeekOrigin.Begin);
                }

                waveReader = new WaveFileReader(stream);
                wav.format = waveReader.WaveFormat;
                wav.dataLengthBytes = (int)waveReader.Length;
                wav.dataOffsetBytes = 28 + subchunkSize;

                if (wav.format.Encoding != WaveFormatEncoding.Pcm
                    || wav.format.Channels < 1
                    || wav.format.Channels > 2
                    || wav.format.BitsPerSample != 16)
                {
                    throw new NotImplementedException("Unsupported WAV format");
                }

                wav.isValid = true;
            }
            catch (Exception)
            {
                wav.Close();
                throw;
            }
            finally
            {
                waveReader?.Dispose();
            }

            return wav;
        }

        public static WAVFile Load(System.IO.UnmanagedMemoryStream stream)
        {
            return Load("", stream as Stream);
        }

        public static WAVFile Load(string filename)
        {
            var file = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            return Load(filename, file);
        }

        public void Close()
        {
            stream?.Close();
            stream = null;
        }
    }
}
