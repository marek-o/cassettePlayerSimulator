using System;
using System.Drawing;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    public class Counter : Control
    {
        private Font font;
        private Bitmap wheelBitmap;

        private float Position = 0.0f;

        private float ZeroPosition = 0.0f;

        private int digitWidth;
        private int digitHeight;

        private int counterDepth = 5;
        private RectangleF counterRect;
        private PointF counterOrigin;
        private Point buttonOrigin;
        private int buttonFaceWidth;
        private int buttonFaceHeight;
        private int buttonDepthUp = 8;
        private int buttonDepthDown = 2;

        private bool ignoreNextSetPosition = false;

        public void SetPosition(float newPos)
        {
            if (ignoreNextSetPosition)
            {
                var posAfterReset = Position - ZeroPosition;
                ZeroPosition = newPos - posAfterReset;
                ignoreNextSetPosition = false;
            }

            Position = newPos;

            if (isResetPressed)
            {
                ZeroPosition = Position;
            }

            Invalidate();
        }

        public void IgnoreNextSetPosition()
        {
            ignoreNextSetPosition = true;
        }

        public Counter()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            DoLayout();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            DoLayout();
        }

        private void DoLayout()
        {
            font?.Dispose();
            font = new Font(FontFamily.GenericMonospace, 20.0f, FontStyle.Bold);
            var digitSize = TextRenderer.MeasureText("0", font);
            digitWidth = (int)(digitSize.Width * 0.8);
            digitHeight = (int)(digitSize.Height * 0.8);

            wheelBitmap?.Dispose();
            wheelBitmap = new Bitmap(digitWidth, digitHeight * 11);
            using (var g = Graphics.FromImage(wheelBitmap))
            {
                g.FillRectangle(Brushes.Black, new RectangleF(0, 0, wheelBitmap.Width, wheelBitmap.Height));

                for (int i = 0; i <= 10; ++i)
                {
                    //wheel background
                    g.FillRectangle(Common.CounterWheelBrush,
                        new RectangleF(digitWidth * 0.1f, i * digitHeight,
                        digitWidth * 0.7f, digitHeight));

                    //right gears
                    g.FillRectangle(Common.CounterWheelBrush,
                        new RectangleF(digitWidth * 0.8f, i * digitHeight,
                        digitWidth * 0.1f, digitHeight * 0.25f));
                    g.FillRectangle(Common.CounterWheelBrush,
                        new RectangleF(digitWidth * 0.8f, i * digitHeight + digitHeight * 0.5f,
                        digitWidth * 0.1f, digitHeight * 0.25f));

                    TextRenderer.DrawText(g, (i % 10).ToString(), font,
                        new Rectangle(0, i * digitHeight - (int)(digitHeight * 0.1f),
                        digitWidth, digitHeight),
                        Color.White, Common.CounterWheelColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                //left gears (next to "7")
                g.FillRectangle(Common.CounterWheelBrush,
                    new RectangleF(digitWidth * 0.0f, 7 * digitHeight,
                    digitWidth * 0.1f, digitHeight * 0.25f));
                g.FillRectangle(Common.CounterWheelBrush,
                    new RectangleF(digitWidth * 0.0f, 7 * digitHeight + digitHeight * 0.5f,
                    digitWidth * 0.1f, digitHeight * 0.25f));
            }

            counterRect = new RectangleF(0, 0, digitWidth * 3 + counterDepth, digitHeight + counterDepth);
            counterOrigin = new PointF(counterDepth, counterDepth);
            buttonOrigin = new Point((int)counterRect.Right + counterDepth * 2, counterDepth);
            buttonFaceWidth = digitWidth + counterDepth;
            buttonFaceHeight = digitWidth + counterDepth;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var posAfterReset = Position - ZeroPosition;

            var posNormalized = Math.Abs(posAfterReset) % 1000.0f;

            if (posAfterReset < 0.0f)
            {
                posNormalized = 1000.0f - posNormalized;
            }

            var positionFractional = posNormalized % 1.0f;
            var position001 = (float)Math.Floor(posNormalized / 1.0f) % 10.0f;
            var position010 = (float)Math.Floor(posNormalized / 10.0f) % 10.0f;
            var position100 = (float)Math.Floor(posNormalized / 100.0f) % 10.0f;

            position001 += positionFractional;

            if (position001 >= 9.0f)
            {
                position010 += positionFractional;

                if (position010 >= 9.0f)
                {
                    position100 += positionFractional;
                }
            }

            e.Graphics.FillRectangle(Brushes.Black, counterRect);

            //wheels
            e.Graphics.DrawImage(wheelBitmap,
                new RectangleF(counterOrigin.X + digitWidth * 0, counterOrigin.Y, digitWidth, digitHeight),
                new RectangleF(0, digitHeight * position100, digitWidth, digitHeight),
                GraphicsUnit.Pixel);
            e.Graphics.DrawImage(wheelBitmap,
                new RectangleF(counterOrigin.X + digitWidth * 1, counterOrigin.Y, digitWidth, digitHeight),
                new RectangleF(0, digitHeight * position010, digitWidth, digitHeight),
                GraphicsUnit.Pixel);
            e.Graphics.DrawImage(wheelBitmap,
                new RectangleF(counterOrigin.X + digitWidth * 2, counterOrigin.Y, digitWidth, digitHeight),
                new RectangleF(0, digitHeight * position001, digitWidth, digitHeight),
                GraphicsUnit.Pixel);

            var bottomPolygon = new PointF[]
            {
                new PointF(0, counterRect.Height),
                new PointF(counterRect.Width, counterRect.Height),
                new PointF(counterRect.Width + counterDepth, counterRect.Height + counterDepth),
                new PointF(counterDepth, counterRect.Height + counterDepth),
            };

            //bottom face
            e.Graphics.FillPolygon(Common.ButtonTopBrush, bottomPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, bottomPolygon);

            var rightPolygon = new PointF[]
            {
                new PointF(counterRect.Width, 0),
                new PointF(counterRect.Width, counterRect.Height),
                new PointF(counterRect.Width + counterDepth, counterRect.Height + counterDepth),
                new PointF(counterRect.Width + counterDepth, counterDepth),
            };

            //right face
            e.Graphics.FillPolygon(Common.ButtonLeftBrush, rightPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, rightPolygon);

            //cover top
            e.Graphics.FillRectangle(Common.CoverBrush, 0, 0, Width, counterDepth);
            e.Graphics.DrawLine(Common.BorderPen, counterDepth, counterDepth, counterRect.Width + counterDepth, counterDepth);

            //cover left
            e.Graphics.FillRectangle(Common.CoverBrush, 0, 0, counterDepth, Height);
            e.Graphics.DrawLine(Common.BorderPen, counterDepth, counterDepth, counterDepth, counterRect.Height + counterDepth);

            //drawing button
            int depth = buttonDepthUp;
            if (isResetPressed)
            {
                depth = buttonDepthDown;
            }

            var faceRect = new Rectangle(buttonOrigin.X + depth, buttonOrigin.Y + depth, buttonFaceWidth, buttonFaceHeight);

            //front face
            e.Graphics.FillRectangle(Common.ButtonFaceBrush, faceRect);
            e.Graphics.DrawRectangle(Common.BorderPen, faceRect);

            var topPolygon = new PointF[]
            {
                new PointF(buttonOrigin.X, buttonOrigin.Y),
                new PointF(buttonOrigin.X + buttonFaceWidth, buttonOrigin.Y),
                new PointF(buttonOrigin.X + buttonFaceWidth + depth, buttonOrigin.Y + depth),
                new PointF(buttonOrigin.X + depth, buttonOrigin.Y + depth),
            };

            //top face
            e.Graphics.FillPolygon(Common.ButtonTopBrush, topPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, topPolygon);

            var leftPolygon = new PointF[]
            {
                new PointF(buttonOrigin.X, buttonOrigin.Y),
                new PointF(buttonOrigin.X, buttonOrigin.Y + buttonFaceHeight),
                new PointF(buttonOrigin.X + depth, buttonOrigin.Y + buttonFaceHeight + depth),
                new PointF(buttonOrigin.X + depth, buttonOrigin.Y + depth),
            };

            //left face
            e.Graphics.FillPolygon(Common.ButtonLeftBrush, leftPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, leftPolygon);
        }

        private bool isResetPressed = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            var buttonRect = new Rectangle(buttonOrigin.X, buttonOrigin.Y,
                buttonFaceWidth + buttonDepthUp, buttonFaceHeight + buttonDepthUp);

            if (buttonRect.Contains(e.Location))
            {
                isResetPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isResetPressed = false;
            Invalidate();
        }
    }
}
