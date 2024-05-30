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

        private int counterDepth;
        private RectangleF counterRect;
        private PointF counterOrigin;
        private Point buttonOrigin;
        private int buttonFaceWidth;
        private int buttonFaceHeight;
        private int buttonDepthUp;
        private int buttonDepthDown;

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
            Scaler scaler = new Scaler(Width / 141.0f);

            counterDepth = scaler.S(10);
            buttonDepthUp = scaler.S(8);
            buttonDepthDown = scaler.S(2);

            font?.Dispose();
            font = new Font(FontFamily.GenericMonospace, Math.Max(scaler.S(20.0f / DpiScaler.DpiScaleFactor), 1.0f), FontStyle.Bold);
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

            counterOrigin = new PointF(counterDepth / 2 + scaler.S(17), counterDepth * 2);
            counterRect = new RectangleF(counterOrigin.X - counterDepth / 2, counterOrigin.Y - counterDepth, digitWidth * 3 + counterDepth / 2, digitHeight + counterDepth);
            buttonOrigin = new Point((int)counterRect.Right + counterDepth, counterDepth);
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

            var topPolygon = new PointF[]
            {
                new PointF(counterOrigin.X - counterDepth / 2, counterOrigin.Y - counterDepth),
                new PointF(counterOrigin.X - counterDepth / 2 + counterRect.Width, counterOrigin.Y - counterDepth),
                new PointF(counterOrigin.X + counterDepth / 2 + counterRect.Width, counterOrigin.Y),
                new PointF(counterOrigin.X, counterOrigin.Y),
            };

            //top face
            e.Graphics.FillPolygon(Common.ButtonLeftBrush, topPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, topPolygon);

            var leftPolygon = new PointF[]
            {
                new PointF(counterOrigin.X - counterDepth / 2, counterOrigin.Y - counterDepth),
                new PointF(counterOrigin.X, counterOrigin.Y),
                new PointF(counterOrigin.X, counterOrigin.Y - counterDepth + counterRect.Height),
                new PointF(counterOrigin.X - counterDepth / 2, counterOrigin.Y - counterDepth + counterRect.Height),
            };

            //left face
            e.Graphics.FillPolygon(Common.ButtonRightBrush, leftPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, leftPolygon);

            //cover bottom
            e.Graphics.FillRectangle(Common.CoverBrush, counterOrigin.X - counterDepth / 2, counterOrigin.Y - counterDepth + counterRect.Height, counterRect.Width + counterDepth / 2, counterDepth);
            e.Graphics.DrawLine(Common.BorderPen, counterOrigin.X - counterDepth / 2, counterOrigin.Y - counterDepth + counterRect.Height, counterOrigin.X - counterDepth / 2 + counterRect.Width, counterOrigin.Y - counterDepth + counterRect.Height);

            //cover right
            e.Graphics.FillRectangle(Common.CoverBrush, counterOrigin.X - counterDepth / 2 + counterRect.Width, counterOrigin.Y - counterDepth, counterDepth, counterRect.Height);
            e.Graphics.DrawLine(Common.BorderPen, counterOrigin.X - counterDepth / 2 + counterRect.Width, counterOrigin.Y - counterDepth, counterOrigin.X - counterDepth / 2 + counterRect.Width, counterOrigin.Y - counterDepth + counterRect.Height);

            //drawing button
            int depth = buttonDepthUp;
            if (isResetPressed)
            {
                depth = buttonDepthDown;
            }

            var faceRect = new Rectangle(buttonOrigin.X - depth / 2, buttonOrigin.Y - depth, buttonFaceWidth, buttonFaceHeight);

            //front face
            e.Graphics.FillRectangle(Common.ButtonFaceBrush, faceRect);
            e.Graphics.DrawRectangle(Common.BorderPen, faceRect);

            var bottomButtonPolygon = new PointF[]
            {
                new PointF(buttonOrigin.X, buttonOrigin.Y + buttonFaceHeight),
                new PointF(buttonOrigin.X + buttonFaceWidth, buttonOrigin.Y + buttonFaceHeight),
                new PointF(buttonOrigin.X + buttonFaceWidth - depth / 2, buttonOrigin.Y + buttonFaceHeight - depth),
                new PointF(buttonOrigin.X - depth / 2, buttonOrigin.Y + buttonFaceHeight - depth),
            };

            //bottom face
            e.Graphics.FillPolygon(Common.ButtonLeftBrush, bottomButtonPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, bottomButtonPolygon);

            var rightButtonPolygon = new PointF[]
            {
                new PointF(buttonOrigin.X + buttonFaceWidth, buttonOrigin.Y),
                new PointF(buttonOrigin.X + buttonFaceWidth - depth / 2, buttonOrigin.Y - depth),
                new PointF(buttonOrigin.X + buttonFaceWidth - depth / 2, buttonOrigin.Y + buttonFaceHeight - depth),
                new PointF(buttonOrigin.X + buttonFaceWidth, buttonOrigin.Y + buttonFaceHeight),
            };

            //right face
            e.Graphics.FillPolygon(Common.ButtonRightBrush, rightButtonPolygon);
            e.Graphics.DrawPolygon(Common.BorderPen, rightButtonPolygon);
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
