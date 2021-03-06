using System;
using System.Drawing;
using System.IO;
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

        private string TapeListFile => Path.Combine(TapesDirectory, "tapes.xml");

        public void PerformImport()
        {
            string inputFileFullPath;
            string outputFilePath = Path.Combine(TapesDirectory, LoadedTapeSide.FilePath);
            float tapeImportPosition;

            using (var dialog = new ImportForm())
            {
                dialog.TapeLengthSeconds = LoadedTapeSide.Parent.Length;
                dialog.TapeImportPosition = LoadedTapeSide.Position;

                var result = dialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }

                inputFileFullPath = dialog.FileName;
                tapeImportPosition = dialog.TapeImportPosition;
            }

            Directory.CreateDirectory(TapesDirectory);

            ProgressForm progressForm = new ProgressForm(string.Format("Importing {0}...", Path.GetFileName(inputFileFullPath)));
            var thread = new Thread(() =>
            {
                Import(inputFileFullPath, outputFilePath, tapeImportPosition, progressForm);
                progressForm.Finish();
            });

            thread.Start();
            progressForm.ShowDialog();
        }

        private void Import(string inputFilePath, string outputFilePath, float tapeImportPosition, IProgress<float> progress = null)
        {
            byte[] buffer = new byte[1024 * 128 * 2];

            Utils.WAVFile outputWavFile = Utils.WAVFile.Load(outputFilePath);

            try
            {
                long byteImportPosition = outputWavFile.dataOffsetBytes
                    +
                    (long)(Common.WaveFormat.SampleRate * tapeImportPosition)
                    * (Common.WaveFormat.BitsPerSample / 8)
                    * Common.WaveFormat.Channels;

                outputWavFile.stream.Seek(byteImportPosition, SeekOrigin.Begin);

                using (var reader = new NAudio.Wave.AudioFileReader(inputFilePath))
                using (var tempConverter = new NAudio.Wave.Wave32To16Stream(reader))
                using (var converter = new NAudio.Wave.WaveFormatConversionStream(Common.WaveFormat, tempConverter))
                {
                    int readCount = 0;

                    int stepCount = 100;
                    int step = 0;
                    int stepSize = (int)(reader.Length / stepCount);
                    int nextStep = 0;

                    do
                    {
                        readCount = converter.Read(buffer, 0, buffer.Length);
                        outputWavFile.stream.Write(buffer, 0, readCount);

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
            finally
            {
                outputWavFile.Close();
            }
        }

        public event Action LoadedTapeSideChanged = () => { };

        private TapeSide loadedTapeSide;

        public TapeSide LoadedTapeSide
        {
            get
            {
                return loadedTapeSide;
            }
            set
            {
                if (loadedTapeSide != value && (loadedTapeSide == null || value == null))
                {
                    if (value != null)
                    {
                        var path = Path.Combine(TapesDirectory, value.FilePath);

                        if (!File.Exists(path))
                        {
                            MessageBox.Show("File not found: " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    loadedTapeSide = value;
                    LoadedTapeSideChanged();
                }
            }
        }

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem toolStripMenuItemLoadA;
        private ToolStripMenuItem toolStripMenuItemLoadB;
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

                    SaveList();
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

            Directory.CreateDirectory(TapesDirectory);

            var pathA = Path.Combine(TapesDirectory, filenameA);
            var pathB = Path.Combine(TapesDirectory, filenameB);

            int seconds = (int)sideLengthSeconds;
            
            byte[] buffer = new byte[Common.WaveFormat.AverageBytesPerSecond * seconds / 100];

            using (var writerA = new NAudio.Wave.WaveFileWriter(pathA, Common.WaveFormat))
            using (var writerB = new NAudio.Wave.WaveFileWriter(pathB, Common.WaveFormat))
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
            tape.SideA.FilePath = filenameA;

            tape.SideB.Label = labelSideB;
            tape.SideB.FilePath = filenameB;

            tape.Length = sideLengthSeconds;
            tape.Color = color;

            return tape;
        }

        public void SaveList()
        {
            listOfTapes.Save(TapeListFile);
        }

        public void EjectTape()
        {
            LoadedTapeSide = null;

            listBox.Invalidate();

            SaveList();
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0 && e.Index < listOfTapes.Tapes.Count)
            {
                var tape = listOfTapes.Tapes[e.Index];

                bool isSelected = (e.State & DrawItemState.Selected) != 0;

                RenderListItemSide(tape.SideA, e.Graphics, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height / 2), "Side A", isSelected);
                RenderListItemSide(tape.SideB, e.Graphics, new Rectangle(e.Bounds.X, e.Bounds.Y + e.Bounds.Height / 2 - 1, e.Bounds.Width, e.Bounds.Height / 2), "Side B", isSelected);

                e.Graphics.DrawRectangle(Pens.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height - 2);
            }
        }

        private void RenderListItemSide(TapeSide side, Graphics g, Rectangle bounds, string prefix, bool isSelected)
        {
            bool isLoaded = side == loadedTapeSide;

            var backBrush = isSelected ? SystemBrushes.Highlight : SystemBrushes.Window;
            g.FillRectangle(backBrush, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1));

            int rectSize = bounds.Height - 1;

            var textColor = isSelected ? SystemColors.HighlightText : SystemColors.ControlText;

            using (SolidBrush b = new SolidBrush(side.Parent.Color))
            {
                var rect = new Rectangle(bounds.X, bounds.Y, rectSize, rectSize);
                g.FillRectangle(b, rect);

                if (isLoaded)
                {
                    rect.Inflate(-rectSize / 4, -rectSize / 4);
                    g.FillEllipse(Brushes.Black, rect);
                }
            }

            TextRenderer.DrawText(g, string.Format("{0}: {1} ({2})", prefix, side.Label, Common.FormatTime((int)side.Parent.Length)),
                SystemFonts.DefaultFont,
                new Rectangle(bounds.X + rectSize, bounds.Y, bounds.Width - rectSize, bounds.Height),
                textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            if (side != null && e.Button == MouseButtons.Right)
            {
                rightClickedTapeSide = side;

                bool isSideA = (side == side.Parent.SideA);

                toolStripMenuItemLoadA.Font = isSideA ? boldFont : normalFont;
                toolStripMenuItemLoadB.Font = !isSideA ? boldFont : normalFont;

                toolStripMenuItemLoadA.Enabled = (LoadedTapeSide == null);
                toolStripMenuItemLoadB.Enabled = (LoadedTapeSide == null);
                toolStripMenuItemDelete.Enabled = (LoadedTapeSide == null);

                contextMenuStrip.Show(listBox.PointToScreen(e.Location));
            }
        }

        private TapeSide rightClickedTapeSide = null;

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            LoadedTapeSide = side;
        }

        private TapeSide GetClickedItem(MouseEventArgs e)
        {
            var i = listBox.IndexFromPoint(e.Location);

            if (i >= 0 && i < listOfTapes.Tapes.Count)
            {
                var rect = listBox.GetItemRectangle(i);
                bool upperHalf = e.Location.Y < (rect.Top + rect.Height / 2);

                var t = listOfTapes.Tapes[i];
                var side = upperHalf ? t.SideA : t.SideB;

                return side;
            }

            return null;
        }

        private void ToolStripMenuItemLoadA_Click(object sender, EventArgs e)
        {
            LoadedTapeSide = rightClickedTapeSide.Parent.SideA;
        }

        private void ToolStripMenuItemLoadB_Click(object sender, EventArgs e)
        {
            LoadedTapeSide = rightClickedTapeSide.Parent.SideB;
        }

        private void ToolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            if (rightClickedTapeSide != null)
            {
                using (var form = new EditTapeForm(true))
                {
                    var tape = rightClickedTapeSide.Parent;

                    form.LabelSideA = tape.SideA.Label;
                    form.LabelSideB = tape.SideB.Label;
                    form.Color = tape.Color;

                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        tape.SideA.Label = form.LabelSideA;
                        tape.SideB.Label = form.LabelSideB;
                        tape.Color = form.Color;

                        SaveList();
                    }
                }
            }
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            if (rightClickedTapeSide != null)
            {
                var tape = rightClickedTapeSide.Parent;

                var result = MessageBox.Show(
                    string.Format("This will remove both tape sides: \"{0}\" and \"{1}\". Are you sure?",
                    tape.SideA.Label, tape.SideB.Label),
                    "Deleting tape", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    var file1 = Path.Combine(TapesDirectory, tape.SideA.FilePath);
                    var file2 = Path.Combine(TapesDirectory, tape.SideB.FilePath);

                    if (File.Exists(file1))
                    {
                        File.Delete(file1);
                    }

                    if (File.Exists(file2))
                    {
                        File.Delete(file2);
                    }

                    listOfTapes.Tapes.Remove(tape);
                    listBox.Items.Remove(tape);

                    SaveList();
                }
            }
        }

        private Font normalFont;
        private Font boldFont;

        public TapeManager(ListBox listBox)
        {
            this.listBox = listBox;

            this.contextMenuStrip = new ContextMenuStrip();
            this.toolStripMenuItemLoadA = new ToolStripMenuItem();
            this.toolStripMenuItemLoadB = new ToolStripMenuItem();
            this.toolStripMenuItemEdit = new ToolStripMenuItem();
            this.toolStripMenuItemDelete = new ToolStripMenuItem();

            normalFont = new Font(contextMenuStrip.Font, FontStyle.Regular);
            boldFont = new Font(contextMenuStrip.Font, FontStyle.Bold);

            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            this.toolStripMenuItemLoadA,
            this.toolStripMenuItemLoadB,
            this.toolStripMenuItemEdit,
            this.toolStripMenuItemDelete});
            this.toolStripMenuItemLoadA.Text = "Load Side A";
            this.toolStripMenuItemLoadA.Click += ToolStripMenuItemLoadA_Click;
            this.toolStripMenuItemLoadB.Text = "Load Side B";
            this.toolStripMenuItemLoadB.Click += ToolStripMenuItemLoadB_Click;
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
