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

        public Form1()
        {
            InitializeComponent();

            mixer = new SoundMixer(16, 1, 44100, 8*1024);

            samp1 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopDown), true, false, 1.0f, 1.0f);
            samp2 = new SoundMixer.Sample(WAVFile.Load(Properties.Resources.stopUp), true, true, 1.0f, 1.0f);

            mixer.AddSample(samp1);
            mixer.AddSample(samp2);
            mixer.Start();
        }
    }
}
