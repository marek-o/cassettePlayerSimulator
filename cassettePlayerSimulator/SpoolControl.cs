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
    public class SpoolControl : Control
    {
        public SpoolControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private Brush spoolBrush = new SolidBrush(Color.FromArgb(240, 240, 240));

        private Brush backgroundBrush = new SolidBrush(Color.FromArgb(169, 169, 169));

        private Brush cassetteBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
        
        private Brush blackWheelBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
        
        private Brush axisBrush = new SolidBrush(Color.FromArgb(192, 192, 192));

        public float scale;

        public float angle = 0;

        internal float spoolInnerRadius => 68 * scale;
        internal float spoolMinRadius => 135 * scale;
        internal float spoolMaxRadius => 295 * scale;
        internal float blackWheelRadius => 50 * scale;
        internal float axisRadius => 10 * scale;

        internal PointF centerLeft => new PointF(371 * scale, 377 * scale);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(cassetteBrush, new RectangleF(0, 0, Width, Height));
            DrawTapeSpool(e.Graphics, new PointF(Width / 2, Height / 2), angle);
        }

        private PointF PolarToCartesian(PointF center, float r, float a)
        {
            return new PointF
            (
                center.X + r * (float)Math.Cos(a * Math.PI / 180),
                center.Y + r * (float)Math.Sin(a * Math.PI / 180)
            );
        }

        private void DrawTapeSpool(Graphics g, PointF center, float angle)
        {
            g.FillEllipse(backgroundBrush, center.X - spoolInnerRadius, center.Y - spoolInnerRadius,
                spoolInnerRadius * 2, spoolInnerRadius * 2);

            g.FillEllipse(blackWheelBrush, center.X - blackWheelRadius, center.Y - blackWheelRadius,
                blackWheelRadius * 2, blackWheelRadius * 2);

            g.FillEllipse(axisBrush, center.X - axisRadius, center.Y - axisRadius,
                axisRadius * 2, axisRadius * 2);

            for (int i = 0; i < 6; ++i)
            {
                float a = 60 * i + angle;
                g.FillPolygon(spoolBrush, new PointF[]
                {
                    PolarToCartesian(center, 55 * scale, a - 2),
                    PolarToCartesian(center, 55 * scale, a + 22),
                    PolarToCartesian(center, spoolInnerRadius, a + 20),
                    PolarToCartesian(center, spoolInnerRadius, a),
                });

                g.FillPolygon(blackWheelBrush, new PointF[]
                {
                    PolarToCartesian(center, 60 * scale, a + 30),
                    PolarToCartesian(center, 60 * scale, a + 50),
                    PolarToCartesian(center, blackWheelRadius, a + 55),
                    PolarToCartesian(center, blackWheelRadius, a + 25),
                });
            }
        }
    }
}
