using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Utils;

namespace cassettePlayerSimulator
{
    public partial class SimulatorForm : Form
    {
        private TapeSide loadedTapeSide = null;
        private TapeManager tapeManager;

        private SoundMixer mixer;
        private SoundMixer.Sample stopDown, stopUp, playDown, playUp, rewDown, rewNoise, rewUp, ffDown, ffNoise, ffUp, recordDown, recordUp,
            pauseDown, pauseUp, unpauseDown, unpauseUp, ejectDown, ejectUp, cassetteClose, cassetteInsert;
        private List<SoundMixer.Sample> effectSamples = new List<SoundMixer.Sample>();
        private SoundMixer.Sample music;

        private float effectsMaxVolume = 2.0f;

        private enum PlayerState
        {
            OPEN, STOPPED, PLAYING, RECORDING, FF, REWIND
        }

        private PlayerState State = PlayerState.OPEN;

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDebug.Text = string.Format("{0}\r\npaused:{1}\r\n{2:F2}\r\nscale {3:F3}\r\nspeed {4:F3}",
                State.ToString(),
                isPauseFullyPressed.ToString(),
                music != null ? music.GetCurrentPositionSeconds() : 0.0f,
                cassetteControl.scaler.ScalingFactor,
                music != null ? music.GetSpeed() : 0.0f);
        }

        private Stopwatch rewindStopwatch = new Stopwatch();
        private Stopwatch autoStopStopwatch = new Stopwatch();

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            if (music == null)
            {
                return;
            }

            if (State == PlayerState.FF || State == PlayerState.REWIND)
            {
                var elapsed = (float)rewindStopwatch.Elapsed.TotalSeconds;
                rewindStopwatch.Restart();

                var pos = music.GetCurrentPositionSeconds();
                var timeOffset = cassetteControl.AngularToLinear(State == PlayerState.FF, pos, elapsed * 360 * 5);
                timeOffset *= (State == PlayerState.REWIND) ? -1 : 1;
                music.SetCurrentPositionSeconds(pos + timeOffset);
            }

            cassetteControl.HeadEngaged = State == PlayerState.PLAYING || State == PlayerState.RECORDING;
            cassetteControl.RollerEngaged = (State == PlayerState.PLAYING || State == PlayerState.RECORDING) && !isTapePaused;
            cassetteControl.AnimateSpools(music.GetCurrentPositionSeconds());

            counter.SetPosition(-cassetteControl.GetSpoolAngleDegrees(false, music.GetCurrentPositionSeconds()) / 360 / 2);

            if ((State == PlayerState.FF
                || State == PlayerState.REWIND
                || (State == PlayerState.RECORDING && !isPauseFullyPressed)
                || (State == PlayerState.PLAYING && !isPauseFullyPressed))
                &&
                music.IsAtBeginningOrEnd())
            {
                if (!autoStopStopwatch.IsRunning)
                {
                    autoStopStopwatch.Restart();
                }
                else if (autoStopStopwatch.Elapsed.TotalSeconds >= 4.0)
                {
                    autoStopStopwatch.Stop();

                    DisengageButtons();
                    State = PlayerState.STOPPED;
                    cassetteButtons.Invalidate();
                }
            }
            else
            {
                autoStopStopwatch.Stop();
            }
        }

        private bool isPauseFullyPressed = false; //is pause button engaged, this is different from playback pause
        private bool isTapePaused = false;

        private void buttonImport_Click(object sender, EventArgs e)
        {
            loadedTapeSide.Position = music.GetCurrentPositionSeconds();
            tapeManager.PerformImport();
        }

        private void SimulatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mixer.Stop();
            StopEjectButton_MouseDown(new CancelEventArgs());
            StopEjectButton_MouseDown(new CancelEventArgs());
        }

        private void DoLayout()
        {
            Scaler scaler = new Scaler();

            float contentRatio = 416 / 457.0f;
            int contentMargin = scaler.S(20);

            int contentWidth = listBox.Left - contentMargin * 2;
            int contentHeight = (int)(contentWidth / contentRatio);
            int maxContentHeight = ClientRectangle.Height - contentMargin * 2;

            if (contentHeight > maxContentHeight)
            {
                contentHeight = maxContentHeight;
                contentWidth = (int)(contentHeight * contentRatio);
            }

            float cassetteRatio = 422 / 275.0f;
            cassetteControl.Location = new Point(contentMargin, contentMargin);
            cassetteControl.Size = new Size(contentWidth, (int)(contentWidth / cassetteRatio));

            float counterRatio = 141 / 57.0f;
            int counterWidth = contentWidth / 3;
            int counterMargin = (int)(contentWidth * 0.02f);
            counter.Location = new Point(contentMargin, cassetteControl.Bottom + counterMargin);
            counter.Size = new Size(counterWidth, (int)(counterWidth / counterRatio));
            counter.Invalidate();

            float buttonsRatio = 422 / 123.0f;
            cassetteButtons.Location = new Point(contentMargin, counter.Bottom);
            cassetteButtons.Size = new Size(contentWidth, (int)(contentWidth / buttonsRatio));
            cassetteButtons.Invalidate();
        }

        private void SimulatorForm_Resize(object sender, EventArgs e)
        {
            DoLayout();
        }

        private void LoadTapeSide(TapeSide tapeSide)
        {
            string path = Path.Combine(tapeManager.TapesDirectory, tapeSide.FilePath);

            if (music != null && loadedTapeSide != null)
            {
                loadedTapeSide.Position = music.GetCurrentPositionSeconds();
            }

            mixer.RemoveSample(music);
            music?.wavFile.Close();

            WAVFile wav = new WAVFile(); //default value in case of error

            try
            {
                wav = WAVFile.Load(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while loading tape", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            music = new SoundMixer.Sample(wav, false, false, false, 0.2f, 1.0f);
            mixer.AddSample(music);
            mixer.SetRecordingSample(music);

            loadedTapeSide = tapeSide;
            music.SetCurrentPositionSeconds(tapeSide.Position);
            music.SetLeadInOutLengthSeconds(5);
            UpdateMusicSpeedParameters();

            State = PlayerState.STOPPED;
            cassetteControl.LoadedTapeSide = tapeSide;
            cassetteButtons.Enabled = true;

            counter.IgnoreNextSetPosition();

            cassetteClose.UpdatePlayback(true);
        }

        public SimulatorForm()
        {
            InitializeComponent();
            DoLayout();

            cassetteButtons.Enabled = false;

#if DEBUG
            timerDebug.Enabled = true;
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

            mixer = new SoundMixer();

            float vol = effectsMaxVolume;
            float speed = 1.0f;

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

            effectSamples.Add(stopDown);
            effectSamples.Add(stopUp);
            effectSamples.Add(playDown);
            effectSamples.Add(playUp);
            effectSamples.Add(rewDown);
            effectSamples.Add(rewNoise);
            effectSamples.Add(rewUp);
            effectSamples.Add(ffDown);
            effectSamples.Add(ffNoise);
            effectSamples.Add(ffUp);
            effectSamples.Add(recordDown);
            effectSamples.Add(recordUp);
            effectSamples.Add(pauseDown);
            effectSamples.Add(pauseUp);
            effectSamples.Add(unpauseDown);
            effectSamples.Add(unpauseUp);
            effectSamples.Add(ejectDown);
            effectSamples.Add(ejectUp);
            effectSamples.Add(cassetteClose);
            effectSamples.Add(cassetteInsert);

            foreach (var samp in effectSamples)
            {
                mixer.AddSample(samp);
            }

            mixer.Start();

            tapeManager = new TapeManager(listBox);

            tapeManager.LoadedTapeSideChanged += TapeManager_LoadedTapeSideChanged;
        }

        private void TapeManager_LoadedTapeSideChanged()
        {
            if (tapeManager.LoadedTapeSide != loadedTapeSide)
            {
                if (tapeManager.LoadedTapeSide != null)
                {
                    LoadTapeSide(tapeManager.LoadedTapeSide);
                }
                else
                {
                    music?.wavFile.Close();
                    loadedTapeSide = null;
                }

                buttonImport.Enabled = tapeManager.LoadedTapeSide != null && music.wavFile.isValid;
            }
        }

        private void buttonCreateTape_Click(object sender, EventArgs e)
        {
            tapeManager.CreateTape();
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
            {
                about.ShowDialog(this);
            }
        }

        private void DisengageButtons()
        {
            if (State == PlayerState.PLAYING)
            {
                stopDown.UpdatePlayback(true);
                SetSoundPlayback(false, false);

                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.UP;
            }
            else if (State == PlayerState.RECORDING)
            {
                stopDown.UpdatePlayback(true);
                SetSoundRecording(false);
                mixer.StopRecording();

                cassetteButtons.RecButton.ButtonState = CassetteButtons.Button.State.UP;
                cassetteButtons.PlayButton.ButtonState = CassetteButtons.Button.State.UP;
            }
            else if (State == PlayerState.REWIND)
            {
                stopDown.UpdatePlayback(true);
                rewNoise.UpdatePlayback(false);
                rewindStopwatch.Stop();

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

        private void SetSoundPlayback(bool setPlaying, bool slowChange)
        {
            int sampleCount = slowChange ? music.wavFile.format.SampleRate * 2 / 5 : music.wavFile.format.SampleRate * 2 / 20;

            if (!isPauseFullyPressed)
            {
                if (setPlaying)
                {
                    music.RampSpeed(0.0f, 1.0f, sampleCount);
                    music.UpdatePlayback(true);
                    isTapePaused = false;
                }
                else
                {
                    music.RampSpeed(1.0f, 0.0f, sampleCount);
                    isTapePaused = true;
                }
            }
        }

        private void SetSoundRecording(bool isRecording)
        {
            if (!isPauseFullyPressed)
            {
                music.UpdateRecording(isRecording);
                isTapePaused = !isRecording;
            }
        }

        private void RecButton_MouseDown(CancelEventArgs e)
        {
            if (State != PlayerState.PLAYING && State != PlayerState.RECORDING)
            {
                DisengageButtons();

                recordDown.UpdatePlayback(true);
                SetSoundRecording(true);
                mixer.StartRecording();

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

                SetSoundPlayback(true, false);

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
                rewindStopwatch.Restart();

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
            if (State != PlayerState.STOPPED && State != PlayerState.OPEN)
            {
                DisengageButtons();

                State = PlayerState.STOPPED;
            }
            else if (State == PlayerState.STOPPED)
            {
                State = PlayerState.OPEN;
                cassetteControl.LoadedTapeSide = null;
                loadedTapeSide.Position = music.GetCurrentPositionSeconds();

                tapeManager.EjectTape();

                ejectDown.UpdatePlayback(true);
            }
        }

        private void StopEjectButton_MouseUp()
        {
            if (State == PlayerState.OPEN)
            {
                ejectUp.UpdatePlayback(true);
                cassetteButtons.Enabled = false;
            }
            else
            {
                stopUp.UpdatePlayback(true);
            }
        }

        private void PauseButton_MouseDown(CancelEventArgs e)
        {
            if (!isPauseFullyPressed)
            {
                pauseDown.UpdatePlayback(true);

                if (State == PlayerState.PLAYING)
                {
                    SetSoundPlayback(false, true);
                }
                else if (State == PlayerState.RECORDING)
                {
                    SetSoundRecording(false);
                }
            }
            else
            {
                unpauseDown.UpdatePlayback(true);
            }
        }

        private void PauseButton_MouseUp()
        {
            if (!isPauseFullyPressed)
            {
                pauseUp.UpdatePlayback(true);
                isPauseFullyPressed = true;
            }
            else
            {
                unpauseUp.UpdatePlayback(true);
                isPauseFullyPressed = false;

                if (State == PlayerState.PLAYING)
                {
                    SetSoundPlayback(true, true);
                }
                else if (State == PlayerState.RECORDING)
                {
                    SetSoundRecording(true);
                }
            }
        }

        private void trackBarEffectsVolume_Scroll(object sender, EventArgs e)
        {
            float vol = effectsMaxVolume * trackBarEffectsVolume.Value / trackBarEffectsVolume.Maximum;
            foreach (var samp in effectSamples)
            {
                samp.SetVolume(vol);
            }
        }

        private void UpdateMusicSpeedParameters()
        {
            music?.SetSpeedParameters(trackBarSpeed.Value / 100.0f,
                trackBarWow.Value * 0.1f / trackBarWow.Maximum,
                trackBarFlutter.Value * 0.1f / trackBarFlutter.Maximum);
        }

        private void trackBarSpeedParameters_Scroll(object sender, EventArgs e)
        {
            UpdateMusicSpeedParameters();
        }

        private void buttonResetSpeedParameters_Click(object sender, EventArgs e)
        {
            trackBarSpeed.Value = 100;
            trackBarWow.Value = 0;
            trackBarFlutter.Value = 0;
            UpdateMusicSpeedParameters();
        }
    }
}
