using System;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    public partial class ProgressForm : Form, IProgress<float>
    {
        private bool enableClosing = false;

        public ProgressForm(string header)
        {
            InitializeComponent();

            label1.Text = header;
            label2.Text = "0%";
        }

        public void Report(float progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<float>(Report), progress);
                return;
            }

            progressBar1.Value = (int)(progress * progressBar1.Maximum);
            label2.Text = string.Format("{0:P0}", progress);
        }

        public void Finish()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Finish));
                return;
            }

            enableClosing = true;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!enableClosing)
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }
    }
}
