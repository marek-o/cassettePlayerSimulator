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
    class TapeManager
    {
        private TapeList listOfTapes = new TapeList();

        //to be extracted
        public string TapesDirectory_tapeManager =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cassette Player Simulator");

        //to be extracted
        private string TapeFile => Path.Combine(TapesDirectory_tapeManager, "tape.wav");

        //to be extracted
        private string TapeListFile => Path.Combine(TapesDirectory_tapeManager, "tapes.xml");

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

        //to be extracted
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
        public event Action LoadedTapeChanged_tapeManager = () => { };

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
                    listBox_tapeManager.Items.Add(tape);
                }
            }
        }

        //to be extracted
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

        //to be extracted
        public void SaveList_tapeManager()
        {
            listOfTapes.Save(TapeListFile);
        }

        //to be extracted
        public void EjectTape_tapeManager()
        {
            LoadedTape_tapeManager = null;

            listBox_tapeManager.Invalidate();
        }

        //to be extracted
        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tape = listOfTapes.Tapes[e.Index];

            bool isSelected = (e.State & DrawItemState.Selected) != 0;

            RenderListItemSide(tape.SideA, e.Graphics, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height / 2), "Side A", isSelected);
            RenderListItemSide(tape.SideB, e.Graphics, new Rectangle(e.Bounds.X, e.Bounds.Y + e.Bounds.Height / 2, e.Bounds.Width, e.Bounds.Height / 2), "Side B", isSelected);

            e.Graphics.DrawRectangle(Pens.Black, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height - 2);
        }

        //to be extracted
        private void RenderListItemSide(TapeSide side, Graphics g, Rectangle bounds, string prefix, bool isSelected)
        {
            bool isLoaded = side == loadedTape_tapeManager;

            var backBrush = isLoaded ? Brushes.LimeGreen :
                isSelected ? SystemBrushes.Highlight : SystemBrushes.Window;
            g.FillRectangle(backBrush, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1));

            TextRenderer.DrawText(g, string.Format("{3}: {0} ({1}) ({2})", side.Label, side.FilePath, side.Length, prefix), SystemFonts.DefaultFont,
                bounds, Color.Black, TextFormatFlags.Left);
        }

        //to be extracted
        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            if (side != null && e.Button == MouseButtons.Right)
            {
                rightClickedTape = side;
                contextMenuStrip1.Show(listBox_tapeManager.PointToScreen(e.Location));
            }
        }

        //to be extracted
        TapeSide rightClickedTape = null;

        //to be extracted
        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TapeSide side = GetClickedItem(e);

            LoadedTape_tapeManager = side;
        }

        //to be extracted
        private TapeSide GetClickedItem(MouseEventArgs e)
        {
            var i = listBox_tapeManager.IndexFromPoint(e.Location);
            var rect = listBox_tapeManager.GetItemRectangle(i);
            bool upperHalf = e.Location.Y < (rect.Top + rect.Height / 2);

            if (i >= 0 && i < listOfTapes.Tapes.Count)
            {
                var t = listOfTapes.Tapes[i];
                var side = upperHalf ? t.SideA : t.SideB;

                return side;
            }

            return null;
        }

        //to be extracted
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

        public TapeManager(ListBox listBox)
        {
            TapeManager_constructor(listBox);
        }
    }
}
