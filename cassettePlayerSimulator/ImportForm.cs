﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static cassettePlayerSimulator.Translations;

namespace cassettePlayerSimulator
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        public string FileName => textBoxFilename.Text;

        public float TapeLengthSeconds { get; set; }

        public float TapeImportPosition { get; set; }


        private int fileLengthSeconds;

        private void ImportForm_Load(object sender, EventArgs e)
        {
            UpdateLabels();

            trackBarPosition.Maximum = (int)TapeLengthSeconds;
            trackBarPosition.Value = (int)TapeImportPosition;
        }

        private void UpdateLabels()
        {
            labelTapeLength.Text = Common.FormatTime((int)TapeLengthSeconds);
            labelTapePosition.Text = Common.FormatTime((int)TapeImportPosition);
            labelLength.Text = Common.FormatTime(fileLengthSeconds);

            positionSelectorControl.TotalLengthSeconds = TapeLengthSeconds;
            positionSelectorControl.SelectionPosition = TapeImportPosition;
            positionSelectorControl.SelectionLength = fileLengthSeconds;
            positionSelectorControl.Invalidate();
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.FileName = textBoxFilename.Text;
                dialog.Filter = string.Format("{0} (*.mp3;*.wma;*.wav)|*.mp3;*.wma;*.wav|{1} (*.*)|*.*", _("Audio files"), _("All files"));
                var result = dialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }

                textBoxFilename.Text = dialog.FileName;
            }

            try
            {
                using (var reader = new NAudio.Wave.AudioFileReader(FileName))
                {
                    fileLengthSeconds = (int)reader.TotalTime.TotalSeconds;
                }

                buttonImport.Enabled = true;
            }
            catch (Exception ex)
            {
                fileLengthSeconds = 0;
                textBoxFilename.Text = "";
                buttonImport.Enabled = false;
                MessageBox.Show(ex.Message, _("Cannot open file"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateLabels();
        }

        private void trackBarPosition_Scroll(object sender, EventArgs e)
        {
            TapeImportPosition = trackBarPosition.Value;
            UpdateLabels();
        }
    }
}
