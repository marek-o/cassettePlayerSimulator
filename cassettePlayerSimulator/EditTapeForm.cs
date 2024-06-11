using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static cassettePlayerSimulator.Translations;

namespace cassettePlayerSimulator
{
    public partial class EditTapeForm : Form
    {
        public EditTapeForm(bool editMode)
        {
            InitializeComponent();

            if (editMode)
            {
                Text = _("Editing tape");
                buttonCreate.Text = _("Save");
                groupBox.Enabled = false;
            }

            colorDialog.Color = panelColor.BackColor = GetRandomSaturatedColor();
        }

        private Color GetRandomSaturatedColor()
        {
            var random = new Random();
            var r = random.Next(256);
            var g = random.Next(256);
            var b = random.Next(256);
            var min = new int[] { r, g, b }.Min();
            var max = new int[] { r, g, b }.Max();
            if (min == max)
            {
                max++;
            }
            r = (r - min) * 255 / (max - min);
            g = (g - min) * 255 / (max - min);
            b = (b - min) * 255 / (max - min);
            return Color.FromArgb(r, g, b);
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            panelColor.BackColor = colorDialog.Color;
        }

        public string LabelSideA
        {
            get
            {
                return textBoxLabelA.Text;
            }
            set
            {
                textBoxLabelA.Text = value;
            }
        }

        public string LabelSideB
        {
            get
            {
                return textBoxLabelB.Text;
            }
            set
            {
                textBoxLabelB.Text = value;
            }
        }

        public Color Color
        {
            get
            {
                return colorDialog.Color;
            }
            set
            {
                colorDialog.Color = value;
                panelColor.BackColor = value;
            }
        }

        public float SideLengthSeconds
        {
            get
            {
                if (radioButtonC10.Checked) return 5 * 60;
                if (radioButtonC30.Checked) return 15 * 60;
                if (radioButtonC60.Checked) return 30 * 60;
                if (radioButtonC90.Checked) return 45 * 60;
                return 15 * 60;
            }
        }
    }
}
