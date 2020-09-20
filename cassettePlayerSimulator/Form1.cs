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
        SoundMixer.Sample stopDown, stopUp, playDown, playUp, music;

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

            cassetteButtons.RecButton.MouseDown += RecButton_MouseDown;
            cassetteButtons.RecButton.MouseUp += RecButton_MouseUp;
            cassetteButtons.PlayButton.MouseDown += PlayButton_MouseDown;
            cassetteButtons.PlayButton.MouseUp += PlayButton_MouseUp;
            cassetteButtons.RewButton.MouseDown += RewButton_MouseDown;
            cassetteButtons.RewButton.MouseUp += RewButton_MouseUp;
            cassetteButtons.FfButton.MouseDown += FfButton_MouseDown;
            cassetteButtons.FfButton.MouseUp += FfButton_MouseUp;
            cassetteButtons.StopEjectButton.MouseDown += StopEjectButton_MouseDown;
            cassetteButtons.StopEjectButton.MouseUp += StopEjectButton_MouseUp;
            cassetteButtons.PauseButton.MouseDown += PauseButton_MouseDown;
            cassetteButtons.PauseButton.MouseUp += PauseButton_MouseUp;

            mixer = new SoundMixer(16, 2, 44100, 8*1024);

            stopDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopDown), false, false, 1.0f, 0.5f);
            stopUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopUp), false, false, 1.0f, 0.5f);

            playDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.playDown), false, false, 1.0f, 0.5f);
            playUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.playUp), false, false, 1.0f, 0.5f);

            mixer.AddSample(stopDown);
            mixer.AddSample(stopUp);
            mixer.AddSample(playDown);
            mixer.AddSample(playUp);
            mixer.Start();
        }

        private void RecButton_MouseDown()
        {
        }

        private void RecButton_MouseUp()
        {
        }

        private void PlayButton_MouseDown()
        {
            playDown.UpdatePlayback(true);

            music.RampSpeed(0.5f, 1.0f, 44100 * 2 / 10);
            music.UpdatePlayback(true);
        }

        private void PlayButton_MouseUp()
        {
            playUp.UpdatePlayback(true);
        }

        private void RewButton_MouseDown()
        {
        }

        private void RewButton_MouseUp()
        {
        }

        private void FfButton_MouseDown()
        {
        }

        private void FfButton_MouseUp()
        {
        }

        private void StopEjectButton_MouseDown()
        {
            stopDown.UpdatePlayback(true);
            music.RampSpeed(1.0f, 0.0f, 44100 * 2 / 10);
        }

        private void StopEjectButton_MouseUp()
        {
            stopUp.UpdatePlayback(true);
        }

        private void PauseButton_MouseDown()
        {
        }

        private void PauseButton_MouseUp()
        {
        }
    }
}
