using System;
using System.Drawing;
using System.Windows.Forms;

namespace cassettePlayerSimulator
{
    public class Counter : Control
    {
        private Font font;
        private Bitmap wheelBitmap;

        public float Position = 0.0f;

        public float ZeroPosition = 0.0f;

        private int digitWidth;
        private int digitHeight;

        private Pen borderPen = new Pen(Color.FromArgb(0, 0, 0));
        private Brush coverBrush = new SolidBrush(Color.FromArgb(169, 169, 169));
        private Brush buttonTopBrush = new SolidBrush(Color.FromArgb(211, 211, 211));
        private Brush buttonLeftBrush = new SolidBrush(Color.FromArgb(128, 128, 128));

        public Counter()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            font = new Font(FontFamily.GenericMonospace, 20.0f, FontStyle.Bold);
            var digitSize = TextRenderer.MeasureText("0", font);
            digitWidth = (int)(digitSize.Width * 0.8);
            digitHeight = (int)(digitSize.Height * 0.8);

            wheelBitmap = new Bitmap(digitWidth, digitHeight * 11);
            using (var g = Graphics.FromImage(wheelBitmap))
            using (var wheelBrush = new SolidBrush(Color.FromArgb(96, 96, 96)))
            {
                g.FillRectangle(Brushes.Black, new RectangleF(0, 0, wheelBitmap.Width, wheelBitmap.Height));

                for (int i = 0; i <= 10; ++i)
                {
                    //wheel background
                    g.FillRectangle(wheelBrush,
                        new RectangleF(digitWidth * 0.1f, i * digitHeight,
                        digitWidth * 0.7f, digitHeight));

                    //right gears
                    g.FillRectangle(wheelBrush,
                        new RectangleF(digitWidth * 0.8f, i * digitHeight,
                        digitWidth * 0.1f, digitHeight * 0.25f));
                    g.FillRectangle(wheelBrush,
                        new RectangleF(digitWidth * 0.8f, i * digitHeight + digitHeight * 0.5f,
                        digitWidth * 0.1f, digitHeight * 0.25f));

                    TextRenderer.DrawText(g, (i % 10).ToString(), font,
                        new Rectangle(0, i * digitHeight - (int)(digitHeight * 0.1f),
                        digitWidth, digitHeight),
                        Color.White, wheelBrush.Color,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                //left gears (next to "7")
                g.FillRectangle(wheelBrush,
                    new RectangleF(digitWidth * 0.0f, 7 * digitHeight,
                    digitWidth * 0.1f, digitHeight * 0.25f));
                g.FillRectangle(wheelBrush,
                    new RectangleF(digitWidth * 0.0f, 7 * digitHeight + digitHeight * 0.5f,
                    digitWidth * 0.1f, digitHeight * 0.25f));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (mouseIsDown)
            {
                ZeroPosition = Position;
            }

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

            int counterDepth = 5;
            var counterRect = new RectangleF(0, 0, digitWidth * 3 + counterDepth, digitHeight + counterDepth);
            e.Graphics.FillRectangle(Brushes.Black, counterRect);

            var counterOrigin = new PointF(counterDepth, counterDepth);

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
            e.Graphics.FillPolygon(buttonTopBrush, bottomPolygon);
            e.Graphics.DrawPolygon(borderPen, bottomPolygon);

            var rightPolygon = new PointF[]
            {
                new PointF(counterRect.Width, 0),
                new PointF(counterRect.Width, counterRect.Height),
                new PointF(counterRect.Width + counterDepth, counterRect.Height + counterDepth),
                new PointF(counterRect.Width + counterDepth, counterDepth),
            };

            //right face
            e.Graphics.FillPolygon(buttonLeftBrush, rightPolygon);
            e.Graphics.DrawPolygon(borderPen, rightPolygon);

            //cover top
            e.Graphics.FillRectangle(coverBrush, 0, 0, Width, counterDepth);
            e.Graphics.DrawLine(borderPen, counterDepth, counterDepth, counterRect.Width + counterDepth, counterDepth);

            //cover left
            e.Graphics.FillRectangle(coverBrush, 0, 0, counterDepth, Height);
            e.Graphics.DrawLine(borderPen, counterDepth, counterDepth, counterDepth, counterRect.Height + counterDepth);
        }

        private bool mouseIsDown = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseIsDown = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseIsDown = false;
        }
    }
}
