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

            img = Properties.Resources.cassette;
        }

        private Brush tapeBrush = new SolidBrush(Color.FromArgb(128, 64, 0));
        private Brush spoolBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
        private Brush blackWheelBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
        private Brush axisBrush = new SolidBrush(Color.FromArgb(192, 192, 192));
        private Pen tapePen = new Pen(Color.FromArgb(128, 64, 0));

        public float scale;

        internal float spoolMinRadius => 144 * scale;

        private float spoolLeftRadius;
        private float spoolRightRadius;

        private float spoolLeftRadiusReal;
        private float spoolRightRadiusReal;

        internal PointF centerLeft => new PointF(371 * scale, 377 * scale);
        internal PointF centerRight => new PointF(899 * scale, 377 * scale);

        internal PointF capstan => new PointF(942 * scale, 774 * scale);
        internal PointF roller => new PointF(942 * scale, 813 * scale);

        internal float capstanRadius => 10 * scale;
        internal float rollerRadius => 30 * scale;

        internal SpoolControl spoolControlLeft;
        internal SpoolControl spoolControlRight;

        private Bitmap img;
        private Bitmap imgScaled;

        private bool cassetteInserted = false;
        public bool CassetteInserted
        {
            get
            {
                return cassetteInserted;
            }
            set
            {
                cassetteInserted = value;
                spoolControlLeft.CassetteInserted = value;
                spoolControlRight.CassetteInserted = value;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

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

            RectangleF destRect = new RectangleF(0, 0, img.Width * scale, img.Height * scale);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (CassetteInserted)
            {
                DrawTapeSpoolOuter(e.Graphics, centerLeft, spoolLeftRadius);
                DrawTapeSpoolOuter(e.Graphics, centerRight, spoolRightRadius);

                e.Graphics.DrawLine(tapePen, 0.1f * Width, (int)(capstan.Y + capstanRadius),
                    0.9f * Width, (int)(capstan.Y + capstanRadius));
            }

            e.Graphics.FillEllipse(axisBrush, capstan.X - capstanRadius, capstan.Y - capstanRadius,
                capstanRadius * 2, capstanRadius * 2);
            e.Graphics.FillEllipse(blackWheelBrush, roller.X - rollerRadius, roller.Y - rollerRadius,
                rollerRadius * 2, rollerRadius * 2);

            if (imgScaled == null || imgScaled.Width != (int)destRect.Width || imgScaled.Height != (int)destRect.Height)
            {
                imgScaled?.Dispose();
                imgScaled = new Bitmap((int)destRect.Width, (int)destRect.Height);

                using (Graphics g = Graphics.FromImage(imgScaled))
                {
                    g.DrawImage(img, destRect, new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                }
            }

            if (CassetteInserted)
            {
                e.Graphics.DrawImage(imgScaled, Point.Empty);
            }
        }

        private void DrawTapeSpoolOuter(Graphics g, PointF center, float outerRadius)
        {
            g.FillEllipse(tapeBrush, center.X - outerRadius, center.Y - outerRadius,
                outerRadius * 2, outerRadius * 2);
            g.FillEllipse(spoolBrush, center.X - spoolMinRadius, center.Y - spoolMinRadius,
                spoolMinRadius * 2, spoolMinRadius * 2);
        }

        private int animationFrame = 0;

        private float tapeDuration = 2700; //s, (default for C90 tape)
        private const float tapeVelocity = 4.76f; //cm/s
        private const float spoolMinRadiusReal = 1.11f; //cm
        private const float tapeThickness = 0.0012f; //cm

        private const float pixelsPerCm = 130;

        private void UpdateRadiusesOfSpools(float seconds)
        {
            spoolLeftRadiusReal = (float)Math.Sqrt(
                (tapeDuration - seconds) * tapeVelocity * tapeThickness / Math.PI
                + Math.Pow(spoolMinRadiusReal, 2));

            spoolRightRadiusReal = (float)Math.Sqrt(
                seconds * tapeVelocity * tapeThickness / Math.PI
                + Math.Pow(spoolMinRadiusReal, 2));

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
            var angle = 2 * (float)Math.PI * (spoolRadiusReal - spoolMinRadiusReal) / tapeThickness;

            return angle;
        }

        private float AngleRemainder(float angle)
        {
            return (float)Math.IEEERemainder(angle, 2 * Math.PI);
        }

        public void AnimateSpools(float seconds)
        {
            UpdateRadiusesOfSpools(seconds);

            if (animationFrame >= 20)
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

        public void SetTapeDuration(float seconds)
        {
            tapeDuration = seconds;
        }
    }
}
