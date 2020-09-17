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
    public partial class CassetteButton : UserControl
    {
        public CassetteButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
        }

        private int frame = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var brush = ((frame % 2) == 0) ? Brushes.Red : Brushes.Yellow;
            e.Graphics.FillRectangle(brush, new RectangleF(0, 0, Width, Height));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            frame++;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            frame++;
            Invalidate();
        }
    }
}
