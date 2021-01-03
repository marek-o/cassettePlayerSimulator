using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    class TapeManager
    {
        private TapeList listOfTapes = new TapeList();

        public string TapesDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cassette Player Simulator");

        private string TapeFile => Path.Combine(TapesDirectory, "tape.wav");

        private string TapeListFile => Path.Combine(TapesDirectory, "tapes.xml");

        public void PerformImport()
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

            Directory.CreateDirectory(TapesDirectory);

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

        public event Action LoadedTapeChanged = () => { };

        private TapeSide loadedTape;

        public TapeSide LoadedTape
        {
            get
            {
                return loadedTape;
            }
            set
            {
                if (loadedTape != value)
                {
                    loadedTape = value;
                    LoadedTapeChanged();
                }
            }
        }

        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItemLoad;
        private ToolStripMenuItem toolStripMenuItemEdit;
        private ToolStripMenuItem toolStripMenuItemDelete;

        private ListBox listBox;

        public void CreateTape()
        {
            using (EditTapeForm form = new EditTapeForm(false))
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

            var pathA = Path.Combine(TapesDirectory, filenameA);
            var pathB = Path.Combine(TapesDirectory, filenameB);

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

        public void SaveList()
        {
            listOfTapes.Save(TapeListFile);
        }

        public void EjectTape()
        {
            LoadedTape = null;

            listBox.Invalidate();
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

            TextRenderer.DrawText(g, string.Format("{3}: {0} ({1}) ({2})", side.Label, side.FilePath, side.Length, prefix), SystemFonts.DefaultFont,
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

        private TapeSide rightClickedTape = null;

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            LoadedTape = side;
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

        private void ToolStripMenuItemLoad_Click(object sender, EventArgs e)
        {
            LoadedTape = rightClickedTape;
        }

        private void ToolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            if (rightClickedTape != null)
            {
                using (var form = new EditTapeForm(true))
                {
                    //FIXME
                    var tape = listOfTapes.Tapes.First(t => t.SideA == rightClickedTape || t.SideB == rightClickedTape);

                    form.LabelSideA = tape.SideA.Label;
                    form.LabelSideB = tape.SideB.Label;
                    form.Color = tape.Color;

                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        tape.SideA.Label = form.LabelSideA;
                        tape.SideB.Label = form.LabelSideB;
                        tape.Color = form.Color;
                    }
                }

            }
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
        }

        public TapeManager(ListBox listBox)
        {
            this.listBox = listBox;

            this.contextMenuStrip1 = new ContextMenuStrip();
            this.toolStripMenuItemLoad = new ToolStripMenuItem();
            this.toolStripMenuItemEdit = new ToolStripMenuItem();
            this.toolStripMenuItemDelete = new ToolStripMenuItem();

            this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            this.toolStripMenuItemLoad,
            this.toolStripMenuItemEdit,
            this.toolStripMenuItemDelete});
            this.toolStripMenuItemLoad.Text = "Load";
            this.toolStripMenuItemLoad.Font = new Font(contextMenuStrip1.Font, FontStyle.Bold);
            this.toolStripMenuItemLoad.Click += ToolStripMenuItemLoad_Click;
            this.toolStripMenuItemEdit.Text = "Edit";
            this.toolStripMenuItemEdit.Click += ToolStripMenuItemEdit_Click;
            this.toolStripMenuItemDelete.Text = "Delete";
            this.toolStripMenuItemDelete.Click += ToolStripMenuItemDelete_Click;

            this.listBox.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBox.ItemHeight = 50;
            this.listBox.DrawItem += listBox_DrawItem;
            this.listBox.MouseDoubleClick += listBox_MouseDoubleClick;
            this.listBox.MouseDown += listBox_MouseDown;

            listOfTapes = TapeList.Load(TapeListFile);
            this.listBox.Items.AddRange(listOfTapes.Tapes.ToArray());
        }
    }
}
