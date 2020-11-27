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
    public partial class CreateTapeForm : Form
    {
        public CreateTapeForm()
        {
            InitializeComponent();

            colorDialog1.Color = panelColor.BackColor = Color.Red;
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            panelColor.BackColor = colorDialog1.Color;
        }

        public string LabelSideA => textBoxLabelA.Text;
        public string LabelSideB => textBoxLabelB.Text;

        public Color Color => colorDialog1.Color;

        public float SideLengthSeconds
        {
            get
            {
                if (radioButtonC30.Checked) return 15 * 60;
                if (radioButtonC60.Checked) return 30 * 60;
                if (radioButtonC90.Checked) return 45 * 60;
                return 15 * 60;
            }
        }
    }
}
