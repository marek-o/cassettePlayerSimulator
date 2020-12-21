using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Utils;

namespace cassettePlayerSimulator
{
    public partial class SimulatorForm : Form
    {
        private TapeList listOfTapes = new TapeList();
        private TapeSide loadedTape = null;
        private TapeManager tapeManager;

        SoundMixer mixer;
        SoundMixer.Sample stopDown, stopUp, playDown, playUp, rewDown, rewNoise, rewUp, ffDown, ffNoise, ffUp, recordDown, recordUp,
            pauseDown, pauseUp, unpauseDown, unpauseUp, ejectDown, ejectUp, cassetteClose, cassetteInsert;
        SoundMixer.Sample music;

        public enum PlayerState
        {
            OPEN, STOPPED, PLAYING, RECORDING, FF, REWIND
        }

        private PlayerState State = PlayerState.OPEN;

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDebug.Text = string.Format("{0}\r\npaused:{1}\r\n{2:F2}", State.ToString(), isPaused.ToString(), music != null ? music.GetCurrentPositionSeconds() : 0.0f);
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
                var timeOffset = cassetteControl1.AngularToLinear(State == PlayerState.FF, pos, elapsed * 360 * 5);
                timeOffset *= (State == PlayerState.REWIND) ? -1 : 1;
                music.SetCurrentPositionSeconds(pos + timeOffset);
            }

            cassetteControl1.HeadEngaged = State == PlayerState.PLAYING || State == PlayerState.RECORDING;
            cassetteControl1.AnimateSpools(music.GetCurrentPositionSeconds());

            counter1.SetPosition(-cassetteControl1.GetSpoolAngle(false, music.GetCurrentPositionSeconds()) / 360 / 2);

            if ((State == PlayerState.FF
                || State == PlayerState.REWIND
                || (State == PlayerState.RECORDING && !isPaused)
                || (State == PlayerState.PLAYING && !isPaused))
                &&
                (music.position == 0.0
                || music.position == music.LastSafePosition()))
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

        private bool isPaused = false; //is pause button engaged, this is different from playback pause

        //to be extracted
        public string TapesDirectory_tapeManager =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cassette Player Simulator");

        private string TapeFile => Path.Combine(TapesDirectory_tapeManager, "tape.wav");

        private string TapeListFile => Path.Combine(TapesDirectory_tapeManager, "tapes.xml");

        private void buttonImport_Click(object sender, EventArgs e)
        {
            PerformImport_tapeManager();
        }
        
        //to be extracted
        public void PerformImport_tapeManager()
        {
            string inputFileFullPath;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Audio files (*.mp3;*.wma;*.wav)|*.mp3;*.wma;*.wav|All files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }

                inputFileFullPath = dialog.FileName;
            }

            Directory.CreateDirectory(TapesDirectory_tapeManager);

            if (File.Exists(TapeFile))
            {
                var res = MessageBox.Show("Are you sure to overwrite existing tape file?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res != DialogResult.OK)
                {
                    return;
                }
            }

            ProgressForm progressForm = new ProgressForm(string.Format("Importing {0}...", Path.GetFileName(inputFileFullPath)));
            var thread = new Thread(() =>
            {
                Import(inputFileFullPath, progressForm);
                progressForm.Finish();
            });

            thread.Start();
            progressForm.ShowDialog();
        }

        private void Import(string inputFilePath, IProgress<float> progress = null)
        {
            float[] buffer = new float[1024 * 128];

            using (var reader = new NAudio.Wave.AudioFileReader(inputFilePath))
            using (var writer = new NAudio.Wave.WaveFileWriter(TapeFile, new NAudio.Wave.WaveFormat(44100, 16, 2)))
            {
                int readCount = 0;

                int stepCount = 100;
                int step = 0;
                int stepSize = (int)(reader.Length / stepCount);
                int nextStep = 0;

                do
                {
                    readCount = reader.Read(buffer, 0, buffer.Length);
                    writer.WriteSamples(buffer, 0, readCount);

                    if (progress != null && reader.Position >= nextStep)
                    {
                        nextStep += stepSize;
                        progress.Report(step / (float)stepCount);
                        step++;
                    }
                }
                while (readCount > 0);
            }
        }

        //to be extracted
        public event Action LoadedTapeChanged_tapeManager = () => {};

        //to be extracted
        private TapeSide loadedTape_tapeManager;

        //to be extracted
        public TapeSide LoadedTape_tapeManager
        {
            get
            {
                return loadedTape_tapeManager;
            }
            set
            {
                if (loadedTape_tapeManager != value)
                {
                    loadedTape_tapeManager = value;
                    LoadedTapeChanged_tapeManager();
                }
            }
        }

        private void LoadTape(TapeSide tapeSide)
        {
            string path = Path.Combine(TapesDirectory_tapeManager, tapeSide.FilePath);

            WAVFile wav = null;
            ProgressForm progressForm = new ProgressForm("Loading tape...");

            var thread = new Thread(() =>
            {
                wav = WAVFile.Load(path, progressForm);
                progressForm.Finish();
            });

            thread.Start();
            progressForm.ShowDialog();

            if (music != null && loadedTape != null)
            {
                loadedTape.Position = music.GetCurrentPositionSeconds();
            }

            mixer.RemoveSample(music);
            music = new SoundMixer.Sample(wav, false, false, false, 0.2f, 1.0f);
            mixer.AddSample(music);
            mixer.SetRecordingSample(music);

            loadedTape = tapeSide;
            music.SetCurrentPositionSeconds(tapeSide.Position);

            State = PlayerState.STOPPED;
            cassetteControl1.CassetteInserted = true;
            cassetteControl1.SetTapeDuration(music.GetLengthSeconds());
            cassetteButtons.Enabled = true;

            counter1.IgnoreNextSetPosition();

            cassetteClose.UpdatePlayback(true);
        }

        public SimulatorForm()
        {
            InitializeComponent();

            cassetteButtons.Enabled = false;

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

            mixer.Start();

            tapeManager = new TapeManager(listBox);
            TapeManager_constructor(listBox);

            LoadedTapeChanged_tapeManager += TapeManager_LoadedTapeChanged;
        }

        private void TapeManager_LoadedTapeChanged()
        {
            if (LoadedTape_tapeManager != loadedTape)
            {
                if (LoadedTape_tapeManager != null)
                {
                    LoadTape(LoadedTape_tapeManager);
                }
            }
        }

        //to be extracted
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChangeLabel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemChangeColor;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;

        //to be extracted
        private ListBox listBox_tapeManager;

        //to be extracted
        public void TapeManager_constructor(ListBox listBox)
        {
            listBox_tapeManager = listBox;

            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.toolStripMenuItemChangeLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemChangeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();

            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemChangeLabel,
            this.toolStripMenuItemChangeColor,
            this.toolStripMenuItemDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
            // 
            // toolStripMenuItemChangeLabel
            // 
            this.toolStripMenuItemChangeLabel.Name = "toolStripMenuItemChangeLabel";
            this.toolStripMenuItemChangeLabel.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemChangeLabel.Text = "Change label";
            this.toolStripMenuItemChangeLabel.Click += new System.EventHandler(this.toolStripMenuItemChangeLabel_Click);
            // 
            // toolStripMenuItemChangeColor
            // 
            this.toolStripMenuItemChangeColor.Name = "toolStripMenuItemChangeColor";
            this.toolStripMenuItemChangeColor.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemChangeColor.Text = "Change color";
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            this.toolStripMenuItemDelete.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemDelete.Text = "Delete";

            this.listBox_tapeManager.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox_tapeManager.ItemHeight = 50;
            this.listBox_tapeManager.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
            this.listBox_tapeManager.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
            this.listBox_tapeManager.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDown);

            listOfTapes = TapeList.Load(TapeListFile);
            listBox_tapeManager.Items.AddRange(listOfTapes.Tapes.ToArray());
        }

        private void buttonCreateTape_Click(object sender, EventArgs e)
        {
            CreateTape_tapeManager();
        }

        //to be extracted
        public void CreateTape_tapeManager()
        {
            using (CreateTapeForm form = new CreateTapeForm(false))
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Tape tape = null;

                    ProgressForm progressForm = new ProgressForm(string.Format("Creating tape..."));
                    var thread = new Thread(() =>
                    {
                        tape = CreateTape(form.SideLengthSeconds, form.LabelSideA, form.LabelSideB, form.Color, progressForm);
                        progressForm.Finish();
                    });

                    thread.Start();
                    progressForm.ShowDialog();

                    listOfTapes.Tapes.Add(tape);
                    listBox.Items.Add(tape);
                }
            }
        }

        private Tape CreateTape(float sideLengthSeconds, string labelSideA, string labelSideB, Color color, IProgress<float> progress = null)
        {
            string filenameA, filenameB;

            do
            {
                var guid = Guid.NewGuid().ToString().Substring(24, 12);
                filenameA = guid + "_A.wav";
                filenameB = guid + "_B.wav";
            }
            while (File.Exists(filenameA) || File.Exists(filenameB));

            var pathA = Path.Combine(TapesDirectory_tapeManager, filenameA);
            var pathB = Path.Combine(TapesDirectory_tapeManager, filenameB);

            int seconds = (int)sideLengthSeconds;

            byte[] buffer = new byte[44100 * 2 * 2 * (seconds / 100)];

            using (var writerA = new NAudio.Wave.WaveFileWriter(pathA, new NAudio.Wave.WaveFormat(44100, 16, 2)))
            using (var writerB = new NAudio.Wave.WaveFileWriter(pathB, new NAudio.Wave.WaveFormat(44100, 16, 2)))
            {
                for (int i = 0; i < 100; ++i)
                {
                    writerA.Write(buffer, 0, buffer.Length);
                    writerB.Write(buffer, 0, buffer.Length);

                    if (progress != null)
                    {
                        progress.Report((float)i / 100);
                    }
                }
            }

            Tape tape = new Tape();

            tape.SideA.Label = labelSideA;
            tape.SideA.Length = sideLengthSeconds;
            tape.SideA.FilePath = filenameA;

            tape.SideB.Label = labelSideB;
            tape.SideB.Length = sideLengthSeconds;
            tape.SideB.FilePath = filenameB;

            return tape;
        }

        private void buttonSaveList_Click(object sender, EventArgs e)
        {
            if (music != null && loadedTape != null)
            {
                var pos = music.GetCurrentPositionSeconds();

                loadedTape.Position = pos;
            }

            SaveList_tapeManager();
        }

        //to be extracted
        public void SaveList_tapeManager()
        {
            listOfTapes.Save(TapeListFile);
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
                SetRecording(false);
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

        private void SetRecording(bool isRecording)
        {
            if (!isPaused)
            {
                music.UpdateRecording(isRecording);
            }
        }

        private void RecButton_MouseDown(CancelEventArgs e)
        {
            if (State != PlayerState.PLAYING && State != PlayerState.RECORDING)
            {
                DisengageButtons();

                recordDown.UpdatePlayback(true);
                SetRecording(true);
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
                cassetteControl1.CassetteInserted = false;
                loadedTape.Position = music.GetCurrentPositionSeconds();
                loadedTape = null;

                EjectTape_tapeManager();

                ejectDown.UpdatePlayback(true);
            }
        }

        public void EjectTape_tapeManager()
        {
            LoadedTape_tapeManager = null;

            listBox.Invalidate();
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
            if (!isPaused)
            {
                pauseDown.UpdatePlayback(true);

                if (State == PlayerState.PLAYING)
                {
                    SetPlayback(false, true);
                }
                else if (State == PlayerState.RECORDING)
                {
                    SetRecording(false);
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
                else if (State == PlayerState.RECORDING)
                {
                    SetRecording(true);
                }
            }
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tape = listOfTapes.Tapes[e.Index];

            bool isSelected = (e.State & DrawItemState.Selected) != 0;

            RenderListItemSide(tape.SideA, e.Graphics, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height / 2), "Side A", isSelected);
            RenderListItemSide(tape.SideB, e.Graphics, new Rectangle(e.Bounds.X, e.Bounds.Y + e.Bounds.Height / 2, e.Bounds.Width, e.Bounds.Height / 2), "Side B", isSelected);

            e.Graphics.DrawRectangle(Pens.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height - 2);
        }

        private void RenderListItemSide(TapeSide side, Graphics g, Rectangle bounds, string prefix, bool isSelected)
        {
            bool isLoaded = side == loadedTape;

            var backBrush = isLoaded ? Brushes.LimeGreen :
                isSelected ? SystemBrushes.Highlight : SystemBrushes.Window;
            g.FillRectangle(backBrush, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1));

            TextRenderer.DrawText(g, string.Format("{3}: {0} ({1}) ({2})", side.Label, side.FilePath, side.Length, prefix), Font,
                bounds, Color.Black, TextFormatFlags.Left);
        }

        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            if (side != null && e.Button == MouseButtons.Right)
            {
                rightClickedTape = side;
                contextMenuStrip1.Show(listBox.PointToScreen(e.Location));
            }
        }

        TapeSide rightClickedTape = null;

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            if (side != null)
            {
                LoadTape(side);
            }
        }

        private TapeSide GetClickedItem(MouseEventArgs e)
        {
            var i = listBox.IndexFromPoint(e.Location);
            var rect = listBox.GetItemRectangle(i);
            bool upperHalf = e.Location.Y < (rect.Top + rect.Height / 2);

            if (i >= 0 && i < listOfTapes.Tapes.Count)
            {
                var t = listOfTapes.Tapes[i];
                var side = upperHalf ? t.SideA : t.SideB;

                return side;
            }

            return null;
        }

        private void toolStripMenuItemChangeLabel_Click(object sender, EventArgs e)
        {
            if (rightClickedTape != null)
            {
                MessageBox.Show(string.Format("changing label of {0} {1}", rightClickedTape.FilePath, rightClickedTape.Length));

                using (var form = new CreateTapeForm(true))
                {
                    //FIXME
                    var tape = listOfTapes.Tapes.First(t => t.SideA == rightClickedTape || t.SideB == rightClickedTape);

                    form.LabelSideA = tape.SideA.Label;
                    form.LabelSideB = tape.SideB.Label;

                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        tape.SideA.Label = form.LabelSideA;
                        tape.SideB.Label = form.LabelSideB;
                    }
                }
                
            }
        }
    }
}
