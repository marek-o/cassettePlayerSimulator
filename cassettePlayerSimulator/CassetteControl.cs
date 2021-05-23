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

            DoLayout();
        }

        private Font tapeLabelFont;
        private Font tapeSideFont;

        public Scaler scaler;
        public float scale;

        internal float spoolMinRadius => scaler.S(144.0f);

        private float spoolLeftRadius;
        private float spoolRightRadius;

        private float spoolLeftRadiusReal;
        private float spoolRightRadiusReal;

        internal PointF cassetteOffset;

        internal PointF centerLeft => PointF.Add(cassetteOffset, scaler.S(new SizeF(371, 377)));
        internal PointF centerRight => PointF.Add(cassetteOffset, scaler.S(new SizeF(899, 377)));

        internal PointF capstan => PointF.Add(cassetteOffset, scaler.S(new SizeF(942, 774)));
        internal PointF roller => PointF.Add(cassetteOffset, scaler.S(new SizeF(942, rollerEngaged ? 833 : 863)));
        internal PointF head => PointF.Add(cassetteOffset, scaler.S(new SizeF(632, headEngaged ? 788 : 843)));

        internal float capstanRadius => scaler.S(10.0f);
        internal float rollerRadius => scaler.S(50.0f);

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

                if (value != null)
                {
                    tapeDuration = value.Parent.Length;
                }

                animationFrame = animationSuspendedFrameCount; //force full repaint
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

            DoLayout();
        }

        private void DoLayout()
        {
            scaler = new Scaler(Width / baseSize.Width);
            scale = Width / baseSize.Width;

            if (baseSize.Height * scale > Height)
            {
                scale = Height / baseSize.Height;
            }

            tapeLabelFont?.Dispose();
            tapeLabelFont = new Font(FontFamily.GenericSansSerif, Math.Max(scaler.S(27.5f), 1.0f), FontStyle.Regular);

            tapeSideFont?.Dispose();
            tapeSideFont = new Font(FontFamily.GenericSansSerif, Math.Max(scaler.S(45.0f), 1.0f), FontStyle.Bold);

            cassetteOffset = new PointF((int)(img.Width * 0.05f * scale), (int)(img.Height * 0.05f * scale));

            var spoolSize = scaler.S(new Size(160, 160));
            
            spoolControlLeft.scale = scale;
            spoolControlLeft.Location = new Point((int)centerLeft.X - spoolSize.Width / 2, (int)centerLeft.Y - spoolSize.Height / 2);
            spoolControlLeft.Size = spoolSize;
            spoolControlLeft.Invalidate();

            spoolControlRight.scale = scale;
            spoolControlRight.Location = new Point((int)centerRight.X - spoolSize.Width / 2, (int)centerRight.Y - spoolSize.Height / 2);
            spoolControlRight.Size = spoolSize;
            spoolControlRight.Invalidate();

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RectangleF destRect = scaler.S(new RectangleF(0, 0, img.Width, img.Height));

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (loadedTapeSide != null)
            {
                DrawTapeSpoolOuter(e.Graphics, centerLeft, spoolLeftRadius);
                DrawTapeSpoolOuter(e.Graphics, centerRight, spoolRightRadius);

                e.Graphics.DrawLine(Common.TapePen, 0.1f * Width, (int)(capstan.Y + capstanRadius),
                    0.9f * Width, (int)(capstan.Y + capstanRadius));
            }

            e.Graphics.FillEllipse(Common.AxisBrush, capstan.X - capstanRadius, capstan.Y - capstanRadius,
                capstanRadius * 2, capstanRadius * 2);
            e.Graphics.FillEllipse(Common.BlackWheelBrush, roller.X - rollerRadius, roller.Y - rollerRadius,
                rollerRadius * 2, rollerRadius * 2);
            e.Graphics.FillEllipse(Common.AxisBrush, roller.X - capstanRadius, roller.Y - capstanRadius,
                capstanRadius * 2, capstanRadius * 2);

            float headWidth = scaler.S(120.0f);
            float headRoundHeight = scaler.S(25.0f);
            float headHeight = scaler.S(70.0f);
            e.Graphics.FillPie(Common.AxisBrush, head.X - headWidth / 2, head.Y,
                headWidth, headRoundHeight * 2, 180, 180);
            e.Graphics.FillRectangle(Common.AxisBrush, head.X - headWidth / 2, head.Y + headRoundHeight - 1,
                headWidth, headHeight);
            e.Graphics.FillRectangle(Common.AxisBrush, head.X + headWidth / 2, head.Y - headRoundHeight / 4,
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

                TextRenderer.DrawText(e.Graphics, loadedTapeSide.Label, tapeLabelFont,
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
            g.FillEllipse(Common.TapeBrush, center.X - outerRadius, center.Y - outerRadius,
                outerRadius * 2, outerRadius * 2);
            g.FillEllipse(Common.SpoolBrush, center.X - spoolMinRadius, center.Y - spoolMinRadius,
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

        private int animationSuspendedFrameCount = 20;

        public void AnimateSpools(float seconds)
        {
            UpdateRadiusesOfSpools(seconds);

            if (animationFrame >= animationSuspendedFrameCount)
            {
                animationFrame = 0;
                Invalidate();
            }

            spoolControlLeft.angleDegrees = AngleRemainder(SpoolAngle(spoolLeftRadiusReal)) * 180 / (float)Math.PI;
            spoolControlLeft.Invalidate();
            spoolControlRight.angleDegrees = -AngleRemainder(SpoolAngle(spoolRightRadiusReal)) * 180 / (float)Math.PI;
            spoolControlRight.Invalidate();

            animationFrame++;
        }
    }
}
