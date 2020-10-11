using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace cassettePlayerSimulator
{
    public partial class SimulatorForm : Form
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDebug.Text = string.Format("{0} paused:{1} {2:F2}", State.ToString(), isPaused.ToString(), music.GetCurrentPositionSeconds());
        }

        private Stopwatch rewindStopwatch = new Stopwatch();

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            if (State == PlayerState.FF || State == PlayerState.REWIND)
            {
                var elapsed = (float)rewindStopwatch.Elapsed.TotalSeconds;
                rewindStopwatch.Restart();

                var pos = music.GetCurrentPositionSeconds();
                var timeOffset = cassetteControl1.AngularToLinear(State == PlayerState.FF, pos, elapsed * 360 * 5);
                timeOffset *= (State == PlayerState.REWIND) ? -1 : 1;
                music.SetCurrentPositionSeconds(pos + timeOffset);
            }

            cassetteControl1.AnimateSpools(music.GetCurrentPositionSeconds());

            counter1.Position = -cassetteControl1.GetSpoolAngle(false, music.GetCurrentPositionSeconds()) / 360 / 2;
            counter1.Invalidate();
        }

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
            music = new SoundMixer.Sample(WAVFile.Load(TapeFile), false, false, false, 0.2f, 1.0f);
            mixer.AddSample(music);
        }

        private void buttonLoadTape_Click(object sender, EventArgs e)
        {
            LoadTape();
        }

        public SimulatorForm()
        {
            InitializeComponent();

#if DEBUG
            timer1.Enabled = true;
#else
            labelDebug.Visible = false;
#endif

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

            float vol = 2.0f;
            float speed = 0.5f;

            stopDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopDown), false, false, true, vol, speed);
            stopUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopUp), false, false, true, vol, speed);

            playDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.playDown), false, false, true, vol, speed);
            playUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.playUp), false, false, true, vol, speed);

            rewDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.rewDown), false, false, true, vol, speed);
            rewUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.rewUp), false, false, true, vol, speed);
            rewNoise = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.rewNoise), false, true, true, vol, speed);

            ffDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ffDown), false, false, true, vol, speed);
            ffUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ffUp), false, false, true, vol, speed);
            ffNoise = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ffNoise), false, true, true, vol, speed);

            recordDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.recordDown), false, false, true, vol, speed);
            recordUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.recordUp), false, false, true, vol, speed);

            pauseDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.pauseDown), false, false, true, vol, speed);
            pauseUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.pauseUp), false, false, true, vol, speed);

            unpauseDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.unpauseDown), false, false, true, vol, speed);
            unpauseUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.unpauseUp), false, false, true, vol, speed);

            ejectDown = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ejectDown), false, false, true, vol, speed);
            ejectUp = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.ejectUp), false, false, true, vol, speed);

            cassetteClose = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.casetteClose), false, false, true, vol, speed);
            cassetteInsert = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.casetteInsert), false, false, true, vol, speed);

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

        private void DisengageButtons()
        {
            if (State == PlayerState.PLAYING)
            {
                stopDown.UpdatePlayback(true);
                SetPlayback(false, false);

                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.UP;
            }
            else if (State == PlayerState.RECORDING)
            {
                stopDown.UpdatePlayback(true);

                cassetteButtons.RecButton.ButtonState = CassetteButtons.Button.State.UP;
                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.UP;
            }
            else if (State == PlayerState.REWIND)
            {
                stopDown.UpdatePlayback(true);
                rewNoise.UpdatePlayback(false);

                cassetteButtons.RewButton.ButtonState = CassetteButtons.Button.State.UP;
            }
            else if (State == PlayerState.FF)
            {
                stopDown.UpdatePlayback(true);
                ffNoise.UpdatePlayback(false);
                rewindStopwatch.Stop();

                cassetteButtons.FfButton.ButtonState = CassetteButtons.Button.State.UP;
            }
        }

        private void SetPlayback(bool setPlaying, bool slowChange)
        {
            int sampleCount = slowChange ? 44100 * 2 / 5 : 44100 * 2 / 20;

            if (!isPaused)
            {
                if (setPlaying)
                {
                    music.RampSpeed(0.0f, 1.0f, sampleCount);
                    music.UpdatePlayback(true);
                }
                else
                {
                    music.RampSpeed(1.0f, 0.0f, sampleCount);
                }
            }
        }

        private void RecButton_MouseDown(CancelEventArgs e)
        {
            if (State != PlayerState.PLAYING && State != PlayerState.RECORDING)
            {
                DisengageButtons();

                recordDown.UpdatePlayback(true);

                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.DOWN;

                State = PlayerState.RECORDING;
            }
            else if (State == PlayerState.PLAYING)
            {
                e.Cancel = true;
            }
        }

        private void RecButton_MouseUp()
        {
            recordUp.UpdatePlayback(true);
        }

        private void PlayButton_MouseDown(CancelEventArgs e)
        {
            if (State != PlayerState.PLAYING && State != PlayerState.RECORDING)
            {
                DisengageButtons();

                playDown.UpdatePlayback(true);

                SetPlayback(true, false);

                State = PlayerState.PLAYING;
            }
        }

        private void PlayButton_MouseUp()
        {
            playUp.UpdatePlayback(true);
        }

        private void RewButton_MouseDown(CancelEventArgs e)
        {
            if (State != PlayerState.REWIND)
            {
                DisengageButtons();

                rewDown.UpdatePlayback(true);
                rewNoise.UpdatePlayback(true);

                State = PlayerState.REWIND;
            }
        }

        private void RewButton_MouseUp()
        {
            rewUp.UpdatePlayback(true);
        }

        private void FfButton_MouseDown(CancelEventArgs e)
        {
            if (State != PlayerState.FF)
            {
                DisengageButtons();

                ffDown.UpdatePlayback(true);
                ffNoise.UpdatePlayback(true);
                rewindStopwatch.Restart();

                State = PlayerState.FF;
            }
        }

        private void FfButton_MouseUp()
        {
            ffUp.UpdatePlayback(true);
        }

        private void StopEjectButton_MouseDown(CancelEventArgs e)
        {
            DisengageButtons();

            if (State != PlayerState.OPEN)
            {
                State = PlayerState.STOPPED;
            }
        }

        private void StopEjectButton_MouseUp()
        {
            stopUp.UpdatePlayback(true);
        }

        private void PauseButton_MouseDown(CancelEventArgs e)
        {
            if (!isPaused)
            {
                pauseDown.UpdatePlayback(true);

                if (State == PlayerState.PLAYING)
                {
                    SetPlayback(false, true);
                }
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

                if (State == PlayerState.PLAYING)
                {
                    SetPlayback(true, true);
                }
            }
        }
    }
}
