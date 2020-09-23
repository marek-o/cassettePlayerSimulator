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
        SoundMixer.Sample stopDown, stopUp, playDown, playUp, rewDown, rewNoise, rewUp, ffDown, ffNoise, ffUp, recordDown, recordUp,
            pauseDown, pauseUp, unpauseDown, unpauseUp, ejectDown, ejectUp, cassetteClose, cassetteInsert;
        SoundMixer.Sample music;

        public enum PlayerState
        {
            OPEN, STOPPED, PLAYING, RECORDING, FF, REWIND
        }

        private PlayerState State = PlayerState.STOPPED;
        private bool isPaused = false; //is pause button engaged, this is different from playback pause

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

        private void LoadTape()
        {
            mixer.RemoveSample(music);
            music = new SoundMixer.Sample(WAVFile.Load(TapeFile), false, false, 0.2f, 1.0f);
            mixer.AddSample(music);
        }

        private void buttonLoadTape_Click(object sender, EventArgs e)
        {
            LoadTape();
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

            rewDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.rewDown), false, false, 1.0f, 0.5f);
            rewUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.rewUp), false, false, 1.0f, 0.5f);
            rewNoise = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.rewNoise), false, true, 1.0f, 0.5f);

            ffDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ffDown), false, false, 1.0f, 0.5f);
            ffUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ffUp), false, false, 1.0f, 0.5f);
            ffNoise = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ffNoise), false, true, 1.0f, 0.5f);

            recordDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.recordDown), false, false, 1.0f, 0.5f);
            recordUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.recordUp), false, false, 1.0f, 0.5f);

            pauseDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.pauseDown), false, false, 1.0f, 0.5f);
            pauseUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.pauseUp), false, false, 1.0f, 0.5f);

            unpauseDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.unpauseDown), false, false, 1.0f, 0.5f);
            unpauseUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.unpauseUp), false, false, 1.0f, 0.5f);

            ejectDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ejectDown), false, false, 1.0f, 0.5f);
            ejectUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ejectUp), false, false, 1.0f, 0.5f);

            cassetteClose = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.casetteClose), false, false, 1.0f, 0.5f);
            cassetteInsert = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.casetteInsert), false, false, 1.0f, 0.5f);

            mixer.AddSample(stopDown);
            mixer.AddSample(stopUp);
            mixer.AddSample(playDown);
            mixer.AddSample(playUp);
            mixer.AddSample(rewDown);
            mixer.AddSample(rewNoise);
            mixer.AddSample(rewUp);
            mixer.AddSample(ffDown);
            mixer.AddSample(ffNoise);
            mixer.AddSample(ffUp);
            mixer.AddSample(recordDown);
            mixer.AddSample(recordUp);
            mixer.AddSample(pauseDown);
            mixer.AddSample(pauseUp);
            mixer.AddSample(unpauseDown);
            mixer.AddSample(unpauseUp);
            mixer.AddSample(ejectDown);
            mixer.AddSample(ejectUp);
            mixer.AddSample(cassetteClose);
            mixer.AddSample(cassetteInsert);

            LoadTape();

            mixer.Start();
        }

        private void RecButton_MouseDown()
        {
            if (State != PlayerState.PLAYING && State != PlayerState.RECORDING)
            {
                recordDown.UpdatePlayback(true);

                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.DOWN;

                State = PlayerState.RECORDING;
            }
        }

        private void RecButton_MouseUp()
        {
            recordUp.UpdatePlayback(true);
        }

        private void PlayButton_MouseDown()
        {
            if (State != PlayerState.PLAYING && State != PlayerState.RECORDING)
            {
                playDown.UpdatePlayback(true);

                music.RampSpeed(0.5f, 1.0f, 44100 * 2 / 10);
                music.UpdatePlayback(true);

                State = PlayerState.PLAYING;
            }
        }

        private void PlayButton_MouseUp()
        {
            playUp.UpdatePlayback(true);
        }

        private void RewButton_MouseDown()
        {
            if (State != PlayerState.REWIND)
            {
                rewDown.UpdatePlayback(true);
                rewNoise.UpdatePlayback(true);

                State = PlayerState.REWIND;
            }
        }

        private void RewButton_MouseUp()
        {
            rewUp.UpdatePlayback(true);
        }

        private void FfButton_MouseDown()
        {
            if (State != PlayerState.FF)
            {
                ffDown.UpdatePlayback(true);
                ffNoise.UpdatePlayback(true);

                State = PlayerState.FF;
            }
        }

        private void FfButton_MouseUp()
        {
            ffUp.UpdatePlayback(true);
        }

        private void StopEjectButton_MouseDown()
        {
            if (State == PlayerState.PLAYING)
            {
                stopDown.UpdatePlayback(true);
                music.RampSpeed(1.0f, 0.0f, 44100 * 2 / 10);

                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.UP;
                State = PlayerState.STOPPED;
            }
            else if (State == PlayerState.RECORDING)
            {
                stopDown.UpdatePlayback(true);

                cassetteButtons.RecButton.ButtonState = CassetteButtons.Button.State.UP;
                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.UP;
                State = PlayerState.STOPPED;
            }
            else if (State == PlayerState.REWIND)
            {
                stopDown.UpdatePlayback(true);
                rewNoise.UpdatePlayback(false);

                cassetteButtons.RewButton.ButtonState = CassetteButtons.Button.State.UP;
                State = PlayerState.STOPPED;
            }
            else if (State == PlayerState.FF)
            {
                stopDown.UpdatePlayback(true);
                ffNoise.UpdatePlayback(false);

                cassetteButtons.FfButton.ButtonState = CassetteButtons.Button.State.UP;
                State = PlayerState.STOPPED;
            }
        }

        private void StopEjectButton_MouseUp()
        {
            stopUp.UpdatePlayback(true);
        }

        private void PauseButton_MouseDown()
        {
            if (!isPaused)
            {
                pauseDown.UpdatePlayback(true);
            }
            else
            {
                unpauseDown.UpdatePlayback(true);
            }
        }

        private void PauseButton_MouseUp()
        {
            if (!isPaused)
            {
                pauseUp.UpdatePlayback(true);
                isPaused = true;
            }
            else
            {
                unpauseUp.UpdatePlayback(true);
                isPaused = false;
            }
        }
    }
}
