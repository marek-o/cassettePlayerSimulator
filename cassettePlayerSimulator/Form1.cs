using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        SoundMixer.Sample samp1, samp2;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lock (samp1.locker)
            {
                samp1.isPlaying = true;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            lock (samp2.locker)
            {
                samp2.isPlaying = true;
            }
        }

        public Form1()
        {
            InitializeComponent();

            mixer = new SoundMixer(16, 1, 44100, 4*1024);

            samp1 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopDown), false, false, 1.0f, 1.0f);
            samp2 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopUp), false, false, 1.0f, 1.0f);

            mixer.AddSample(samp1);
            mixer.AddSample(samp2);
            mixer.Start();
        }
    }
}
