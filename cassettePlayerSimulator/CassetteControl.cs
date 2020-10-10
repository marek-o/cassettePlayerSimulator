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
    public class CassetteControl : Control
    {
        public CassetteControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            spoolControlLeft = new SpoolControl();
            spoolControlRight = new SpoolControl();
            Controls.Add(spoolControlLeft);
            Controls.Add(spoolControlRight);
        }

        private Brush tapeBrush = new SolidBrush(Color.FromArgb(128, 64, 0));
        private Brush spoolBrush = new SolidBrush(Color.FromArgb(240, 240, 240));

        public float scale;

        internal float spoolMinRadius => 144 * scale;
        internal float spoolMaxRadius => 323 * scale;

        private float spoolLeftRadius;
        private float spoolRightRadius;

        private float spoolLeftRadiusReal;
        private float spoolRightRadiusReal;

        internal PointF centerLeft => new PointF(371 * scale, 377 * scale);
        internal PointF centerRight => new PointF(899 * scale, 377 * scale);

        internal SpoolControl spoolControlLeft;
        internal SpoolControl spoolControlRight;

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
            
            spoolControlLeft.scale = scale;
            spoolControlLeft.Location = new Point((int)centerLeft.X - spoolSize.Width / 2, (int)centerLeft.Y - spoolSize.Height / 2);
            spoolControlLeft.Size = spoolSize;

            spoolControlRight.scale = scale;
            spoolControlRight.Location = new Point((int)centerRight.X - spoolSize.Width / 2, (int)centerRight.Y - spoolSize.Height / 2);
            spoolControlRight.Size = spoolSize;
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

        private int animationFrame = 0;

        private const float tapeDuration = 2700; //s, C90 one side
        private const float tapeVelocity = 4.76f; //cm/s
        private const float spoolMinRadiusReal = 1.11f; //cm
        private const float spoolMaxRadiusReal = 2.49f; //cm

        private const float pixelsPerCm = 130;

        private void UpdateRadiusesOfSpools(float seconds)
        {
            float tapePercent = seconds / tapeDuration;

            spoolLeftRadiusReal = (float)Math.Sqrt(
                (1 - tapePercent) * Math.Pow(spoolMaxRadiusReal, 2)
                + tapePercent * Math.Pow(spoolMinRadiusReal, 2));

            spoolRightRadiusReal = (float)Math.Sqrt(
                tapePercent * Math.Pow(spoolMaxRadiusReal, 2)
                + (1 - tapePercent) * Math.Pow(spoolMinRadiusReal, 2));

            spoolLeftRadius = spoolLeftRadiusReal * pixelsPerCm * scale;
            spoolRightRadius = spoolRightRadiusReal * pixelsPerCm * scale;
        }

        public float AngularToLinear(bool rightSpool, float seconds, float angularOffset)
        {
            UpdateRadiusesOfSpools(seconds);

            var spoolRadiusReal = rightSpool ? spoolRightRadiusReal : spoolLeftRadiusReal;

            var linearOffset = spoolRadiusReal * (float)(angularOffset * Math.PI / 180);
            return linearOffset / tapeVelocity;
        }

        public float GetSpoolAngle(bool rightSpool, float seconds)
        {
            UpdateRadiusesOfSpools(seconds);
            var angle = SpoolAngle(rightSpool ? spoolRightRadiusReal : spoolLeftRadiusReal);
            return angle * 180 / (float)Math.PI;
        }

        private float SpoolAngle(float spoolRadiusReal)
        {
            var angle = 2 * tapeVelocity * tapeDuration * (spoolRadiusReal - spoolMinRadiusReal)
                / (float)(Math.Pow(spoolMaxRadiusReal, 2) - Math.Pow(spoolMinRadiusReal, 2));
            return angle;
        }

        private float AngleRemainder(float angle)
        {
            return (float)Math.IEEERemainder(angle, 2 * Math.PI);
        }

        public void AnimateSpools(float seconds)
        {
            UpdateRadiusesOfSpools(seconds);

            if (animationFrame >= 60)
            {
                animationFrame = 0;
                Invalidate();
            }

            spoolControlLeft.angle = AngleRemainder(SpoolAngle(spoolLeftRadiusReal)) * 180 / (float)Math.PI;
            spoolControlLeft.Invalidate();
            spoolControlRight.angle = -AngleRemainder(SpoolAngle(spoolRightRadiusReal)) * 180 / (float)Math.PI;
            spoolControlRight.Invalidate();

            animationFrame++;
        }
    }
}
