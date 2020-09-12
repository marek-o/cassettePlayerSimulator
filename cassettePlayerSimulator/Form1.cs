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
        SoundWrapper player;
        WAVFile sound;

        public Form1()
        {
            InitializeComponent();

            player = new SoundWrapper(SoundWrapper.Mode.Play, 16, 1, 44100, 8*1024);
            player.NewDataRequested += Player_NewDataRequested;

            sound = WAVFile.Load("../../sounds/stopDown.wav");

            player.Start(0);

        }

        int pos = 0;

        private void Player_NewDataRequested(object sender, SoundWrapper.NewDataEventArgs e)
        {
            for (int i = 0; i < e.data.Length; ++i)
            {
                e.data[i] = sound.data[pos];

                ++pos;
                if (pos >= sound.data.Length)
                {
                    pos = sound.data.Length - 1;
                }
            }
        }
    }
}
