﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    public class SpoolControl : Control
    {
        public SpoolControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private SolidBrush backgroundBrush = new SolidBrush(DefaultBackColor);

        public float scale;

        public float angleDegrees = 0;

        private float spoolInnerRadius => 68 * scale;
        private float blackWheelRadius => 50 * scale;
        private float axisRadius => 10 * scale;

        public bool CassetteInserted { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            backgroundBrush.Color = BackColor;

            if (CassetteInserted)
            {
                e.Graphics.FillRectangle(Common.CassetteBodyBrush, new RectangleF(0, 0, Width, Height));
            }

            DrawTapeSpool(e.Graphics, new PointF(Width / 2, Height / 2), angleDegrees);
        }

        private PointF PolarToCartesian(PointF center, float r, float angleDegrees)
        {
            return new PointF
            (
                center.X + r * (float)Math.Cos(angleDegrees * Math.PI / 180),
                center.Y + r * (float)Math.Sin(angleDegrees * Math.PI / 180)
            );
        }

        private void DrawTapeSpool(Graphics g, PointF center, float angleDegrees)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.FillEllipse(backgroundBrush, center.X - spoolInnerRadius, center.Y - spoolInnerRadius,
                spoolInnerRadius * 2, spoolInnerRadius * 2);

            g.FillEllipse(Common.BlackWheelBrush, center.X - blackWheelRadius, center.Y - blackWheelRadius,
                blackWheelRadius * 2, blackWheelRadius * 2);

            g.FillEllipse(Common.AxisBrush, center.X - axisRadius, center.Y - axisRadius,
                axisRadius * 2, axisRadius * 2);

            for (int i = 0; i < 6; ++i)
            {
                float a = 60 * i + angleDegrees;
                if (CassetteInserted)
                {
                    g.FillPolygon(Common.SpoolBrush, new PointF[]
                    {
                        PolarToCartesian(center, 55 * scale, a - 2),
                        PolarToCartesian(center, 55 * scale, a + 22),
                        PolarToCartesian(center, spoolInnerRadius, a + 20),
                        PolarToCartesian(center, spoolInnerRadius, a),
                    });
                }

                g.FillPolygon(Common.BlackWheelBrush, new PointF[]
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
