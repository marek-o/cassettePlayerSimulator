using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        public string FileName => textBoxFilename.Text;

        private int fileLengthSeconds;

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.FileName = textBoxFilename.Text;
                dialog.Filter = "Audio files (*.mp3;*.wma;*.wav)|*.mp3;*.wma;*.wav|All files (*.*)|*.*";
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
                    labelLength.Text = Common.FormatTime(fileLengthSeconds);
                }
            }
            catch (Exception ex)
            {
                fileLengthSeconds = 0;
                labelLength.Text = Common.FormatTime(fileLengthSeconds);
                textBoxFilename.Text = "";
                MessageBox.Show(ex.Message, "Cannot open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
