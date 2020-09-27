﻿using System;
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
            spoolControl1 = new SpoolControl();
            spoolControl2 = new SpoolControl();
            Controls.Add(spoolControl1);
            Controls.Add(spoolControl2);
        }

        private Brush tapeBrush = new SolidBrush(Color.FromArgb(128, 64, 0));
        private Brush spoolBrush = new SolidBrush(Color.FromArgb(240, 240, 240));

        public float scale;

        internal float spoolMinRadius => 144 * scale;
        internal float spoolMaxRadius => 323 * scale;

        private float spoolLeftRadius;
        private float spoolRightRadius;

        internal PointF centerLeft => new PointF(371 * scale, 377 * scale);
        internal PointF centerRight => new PointF(899 * scale, 377 * scale);

        internal SpoolControl spoolControl1;
        internal SpoolControl spoolControl2;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var img = Properties.Resources.cassette;
            scale = (float)Width / img.Width;

            if (img.Height * scale > Height)
            {
                scale = (float)Height / img.Height;
            }

            var spoolSize = new Size((int)(160 * scale), (int)(160 * scale));
            
            spoolControl1.scale = scale;
            spoolControl1.Location = new Point((int)centerLeft.X - spoolSize.Width / 2, (int)centerLeft.Y - spoolSize.Height / 2);
            spoolControl1.Size = spoolSize;

            spoolControl2.scale = scale;
            spoolControl2.Location = new Point((int)centerRight.X - spoolSize.Width / 2, (int)centerRight.Y - spoolSize.Height / 2);
            spoolControl2.Size = spoolSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var img = Properties.Resources.cassette;

            RectangleF destRect = new RectangleF(0, 0, img.Width * scale, img.Height * scale);

            DrawTapeSpoolOuter(e.Graphics, centerLeft, spoolLeftRadius);
            DrawTapeSpoolOuter(e.Graphics, centerRight, spoolRightRadius);

            e.Graphics.DrawImage(img, destRect, new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        }

        private void DrawTapeSpoolOuter(Graphics g, PointF center, float outerRadius)
        {
            g.FillEllipse(tapeBrush, center.X - outerRadius, center.Y - outerRadius,
                outerRadius * 2, outerRadius * 2);
            g.FillEllipse(spoolBrush, center.X - spoolMinRadius, center.Y - spoolMinRadius,
                spoolMinRadius * 2, spoolMinRadius * 2);
        }

        public void AnimateSpools(float seconds)
        {
            float tapeDuration = 2700; //s, C90 one side
            float tapeVelocity = 4.76f; //cm/s
            float radiusMin = 1.11f; //cm
            float radiusMax = 2.49f; //cm

            float tapePercent = seconds / tapeDuration;

            float radiusLeft = (float)Math.Sqrt(
                (1 - tapePercent) * Math.Pow(radiusMax, 2)
                + tapePercent * Math.Pow(radiusMin, 2));

            float radiusRight = (float)Math.Sqrt(
                tapePercent * Math.Pow(radiusMax, 2)
                + (1 - tapePercent) * Math.Pow(radiusMin, 2));

            float pixelsPerCm = 130;

            spoolLeftRadius = radiusLeft * pixelsPerCm * scale;
            spoolRightRadius = radiusRight * pixelsPerCm * scale;
            Invalidate(); //debug

            float angle = -DateTime.Now.Millisecond / 1000.0f * 360.0f / 3;

            spoolControl1.angle = angle;
            spoolControl1.Invalidate();
            spoolControl2.angle = angle;
            spoolControl2.Invalidate();
        }
    }
}
