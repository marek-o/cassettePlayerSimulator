using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace cassettePlayerSimulator
{
    public partial class Form1 : Form
    {
        SoundMixer mixer;
        SoundMixer.Sample samp1, samp2, samp3, samp4, music;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            samp3.UpdatePlayback(true);
            music.UpdatePlayback(true);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            samp4.UpdatePlayback(true);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            samp1.UpdatePlayback(true);
            music.UpdatePlayback(false);
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            samp2.UpdatePlayback(true);
        }

        private string TapesDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cassette Player Simulator");

        private string TapeFile => Path.Combine(TapesDirectory, "tape.wav");

        private void buttonImport_Click(object sender, EventArgs e)
        {
            string inputFileFullPath;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Audio files (*.mp3;*.wma;*.wav)|*.mp3;*.wma;*.wav|All files (*.*)|*.*";
                dialog.ShowDialog();
                inputFileFullPath = dialog.FileName;
            }

            Directory.CreateDirectory(TapesDirectory);

            if (File.Exists(TapeFile))
            {
                var res = MessageBox.Show("Are you sure to overwrite existing tape file?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res != DialogResult.OK)
                {
                    return;
                }
            }

            float[] buffer = new float[1024*128];

            using (var reader = new NAudio.Wave.AudioFileReader(inputFileFullPath))
            using (var writer = new NAudio.Wave.WaveFileWriter(TapeFile, new NAudio.Wave.WaveFormat(44100, 16, 2)))
            {
                int readCount = 0;

                do
                {
                    readCount = reader.Read(buffer, 0, buffer.Length);
                    writer.WriteSamples(buffer, 0, readCount);
                }
                while (readCount > 0);
            }

            MessageBox.Show("Import finished");
        }

        private void buttonLoadTape_Click(object sender, EventArgs e)
        {
            mixer.RemoveSample(music);
            music = new SoundMixer.Sample(WAVFile.Load(TapeFile), true, false, 0.2f, 1.0f);
            mixer.AddSample(music);
        }

        public Form1()
        {
            InitializeComponent();

            mixer = new SoundMixer(16, 2, 44100, 8*1024);

            samp1 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopDown), false, false, 1.0f, 0.5f);
            samp2 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopUp), false, false, 1.0f, 0.5f);

            samp3 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.playDown), false, false, 1.0f, 0.5f);
            samp4 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.playUp), false, false, 1.0f, 0.5f);

            mixer.AddSample(samp1);
            mixer.AddSample(samp2);
            mixer.AddSample(samp3);
            mixer.AddSample(samp4);
            mixer.Start();
        }
    }
}
