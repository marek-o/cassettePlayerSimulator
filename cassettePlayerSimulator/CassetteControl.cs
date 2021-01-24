using System;
using System.Drawing;
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
            baseSize = new SizeF(img.Width * 1.1f, img.Height * 1.1f);
        }

        private Brush tapeBrush = new SolidBrush(Color.FromArgb(128, 64, 0));
        private Brush spoolBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
        private Brush blackWheelBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
        private Brush axisBrush = new SolidBrush(Color.FromArgb(192, 192, 192));
        private Pen tapePen = new Pen(Color.FromArgb(128, 64, 0));

        private Font tapeSideFont = new Font(FontFamily.GenericSansSerif, 15.0f, FontStyle.Bold);

        public float scale;

        internal float spoolMinRadius => 144 * scale;

        private float spoolLeftRadius;
        private float spoolRightRadius;

        private float spoolLeftRadiusReal;
        private float spoolRightRadiusReal;

        internal PointF cassetteOffset;

        internal PointF centerLeft => new PointF(cassetteOffset.X + 371 * scale, cassetteOffset.Y + 377 * scale);
        internal PointF centerRight => new PointF(cassetteOffset.X + 899 * scale, cassetteOffset.Y + 377 * scale);

        internal PointF capstan => new PointF(cassetteOffset.X + 942 * scale, cassetteOffset.Y + 774 * scale);
        internal PointF roller => new PointF(cassetteOffset.X + 942 * scale, cassetteOffset.Y + (rollerEngaged ? 833 : 863) * scale);
        internal PointF head => new PointF(cassetteOffset.X + 632 * scale, cassetteOffset.Y + (headEngaged ? 788 : 843) * scale);

        internal float capstanRadius => 10 * scale;
        internal float rollerRadius => 50 * scale;

        internal SpoolControl spoolControlLeft;
        internal SpoolControl spoolControlRight;

        private Bitmap img;
        private Bitmap imgScaled;

        private SizeF baseSize;

        private TapeSide loadedTapeSide;
        public TapeSide LoadedTapeSide
        {
            set
            {
                loadedTapeSide = value;

                spoolControlLeft.CassetteInserted = value != null;
                spoolControlRight.CassetteInserted = value != null;

                Invalidate();
            }
        }

        private Color prevCassetteColor = Color.Transparent;

        private bool headEngaged = false;
        public bool HeadEngaged
        {
            set
            {
                if (headEngaged != value)
                {
                    headEngaged = value;
                    Invalidate();
                }
            }
        }

        private bool rollerEngaged = false;
        public bool RollerEngaged
        {
            set
            {
                if (rollerEngaged != value)
                {
                    rollerEngaged = value;
                    Invalidate();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            scale = Width / baseSize.Width;

            if (baseSize.Height * scale > Height)
            {
                scale = Height / baseSize.Height;
            }

            cassetteOffset = new PointF((int)(img.Width * 0.05f * scale), (int)(img.Height * 0.05f * scale));

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

            if (loadedTapeSide != null)
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
            e.Graphics.FillEllipse(axisBrush, roller.X - capstanRadius, roller.Y - capstanRadius,
                capstanRadius * 2, capstanRadius * 2);

            float headWidth = 120 * scale;
            float headRoundHeight = 25 * scale;
            float headHeight = 70 * scale;
            e.Graphics.FillPie(axisBrush, head.X - headWidth / 2, head.Y,
                headWidth, headRoundHeight * 2, 180, 180);
            e.Graphics.FillRectangle(axisBrush, head.X - headWidth / 2, head.Y + headRoundHeight - 1,
                headWidth, headHeight);
            e.Graphics.FillRectangle(axisBrush, head.X + headWidth / 2, head.Y - headRoundHeight / 4,
                5 * scale, headRoundHeight / 4 + headRoundHeight + headHeight);

            Color cassetteColor = loadedTapeSide?.Parent.Color ?? Color.Transparent;

            if (imgScaled == null
                || imgScaled.Width != (int)destRect.Width
                || imgScaled.Height != (int)destRect.Height
                || prevCassetteColor != cassetteColor)
            {
                imgScaled?.Dispose();
                imgScaled = new Bitmap((int)destRect.Width, (int)destRect.Height);

                prevCassetteColor = cassetteColor;

                using (Graphics g = Graphics.FromImage(imgScaled))
                using (var b = new SolidBrush(cassetteColor))
                {
                    g.FillRectangle(b, 50 * scale, 200 * scale, 230 * scale, 400 * scale);
                    g.FillRectangle(b, 980 * scale, 200 * scale, 230 * scale, 400 * scale);
                    g.FillRectangle(b, 50 * scale, 200 * scale, 1200 * scale, 70 * scale);
                    g.FillRectangle(b, 50 * scale, 480 * scale, 1200 * scale, 130 * scale);
                    g.DrawImage(img, destRect, new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                }
            }

            if (loadedTapeSide != null)
            {
                e.Graphics.DrawImage(imgScaled, cassetteOffset);

                TextRenderer.DrawText(e.Graphics, loadedTapeSide.Label, Font,
                    new Rectangle((int)(266 * scale), (int)(153 * scale), (int)(813 * scale), (int)(70 * scale)),
                    Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(e.Graphics, (loadedTapeSide.Parent.SideA == loadedTapeSide) ? "A" : "B", tapeSideFont,
                    new Rectangle((int)(180 * scale), (int)(142 * scale), (int)(90 * scale), (int)(90 * scale)),
                    Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);

                TextRenderer.DrawText(e.Graphics, ((int)Math.Round(loadedTapeSide.Parent.Length / 900.0f) * 30).ToString(), tapeSideFont,
                    new Rectangle((int)(1112 * scale), (int)(410 * scale), (int)(90 * scale), (int)(90 * scale)),
                    Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
            }

            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
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

        public float GetSpoolAngleDegrees(bool rightSpool, float seconds)
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
