using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    public partial class CassetteControl : UserControl
    {
        public CassetteControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var img = Properties.Resources.cassette;
            RectangleF destRect = new RectangleF(0, 0, img.Width * Width / img.Width, img.Height * Width / img.Width);

            if (destRect.Height > Height)
            {
                destRect = new RectangleF(0, 0, img.Width * Height / img.Height, img.Height * Height / img.Height);
            }

            e.Graphics.DrawImage(img, destRect, new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        }
    }
}
