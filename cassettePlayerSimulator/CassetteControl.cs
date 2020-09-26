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

        private Brush tapeBrush = new SolidBrush(Color.FromArgb(128, 64, 0));
        private Brush spoolBrush = new SolidBrush(Color.FromArgb(240, 240, 240));

        private Brush backgroundBrush = new SolidBrush(Color.FromArgb(128, 128, 128));

        private float scale;

        public float angle = 0;

        internal float spoolInnerRadius => 68 * scale;
        internal float spoolMinRadius => 135 * scale;
        internal float spoolMaxRadius => 295 * scale;

        internal PointF centerLeft => new PointF(371 * scale, 377 * scale);
        internal PointF centerRight => new PointF(899 * scale, 377 * scale);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var img = Properties.Resources.cassette;
            scale = (float) Width / img.Width;

            if (img.Height * scale > Height)
            {
                scale = (float) Height / img.Height;
            }

            RectangleF destRect = new RectangleF(0, 0, img.Width * scale, img.Height * scale);

            DrawTapeSpool(e.Graphics, centerLeft, 295 * scale, angle);
            DrawTapeSpool(e.Graphics, centerRight, 135 * scale, 0);

            e.Graphics.DrawImage(img, destRect, new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        }

        private PointF PolarToCartesian(PointF center, float r, float a)
        {
            return new PointF
            (
                center.X + r * (float)Math.Cos(a * Math.PI / 180),
                center.Y + r * (float)Math.Sin(a * Math.PI / 180)
            );
        }

        private void DrawTapeSpool(Graphics g, PointF center, float outerRadius, float angle)
        {
            g.FillEllipse(tapeBrush, center.X - outerRadius, center.Y - outerRadius,
                outerRadius * 2, outerRadius * 2);
            g.FillEllipse(spoolBrush, center.X - spoolMinRadius, center.Y - spoolMinRadius,
                spoolMinRadius * 2, spoolMinRadius * 2);
            g.FillEllipse(backgroundBrush, center.X - spoolInnerRadius, center.Y - spoolInnerRadius,
                spoolInnerRadius * 2, spoolInnerRadius * 2);

            for (int i = 0; i < 6; ++i)
            {
                float a = 60 * i + angle;
                g.FillPolygon(spoolBrush, new PointF[]
                {
                    PolarToCartesian(center, 55 * scale, a),
                    PolarToCartesian(center, 55 * scale, a + 20),
                    PolarToCartesian(center, 80 * scale, a + 20),
                    PolarToCartesian(center, 80 * scale, a),
                });
            }
        }
    }
}
