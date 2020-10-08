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

        private int digitWidth;
        private int digitHeight;

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

            e.Graphics.FillRectangle(Brushes.Brown, new RectangleF(0, 0, Width, Height));

            var positionFractional = Position % 1.0f;
            var position001 = (float)Math.Floor(Position / 1.0f) % 10.0f;
            var position010 = (float)Math.Floor(Position / 10.0f) % 10.0f;
            var position100 = (float)Math.Floor(Position / 100.0f) % 10.0f;

            position001 += positionFractional;

            if (position001 >= 9.0f)
            {
                position010 += positionFractional;

                if (position010 >= 9.0f)
                {
                    position100 += positionFractional;
                }
            }

            e.Graphics.DrawImage(wheelBitmap,
                new RectangleF(digitWidth * 0, 0, digitWidth, digitHeight),
                new RectangleF(0, digitHeight * position100, digitWidth, digitHeight),
                GraphicsUnit.Pixel);
            e.Graphics.DrawImage(wheelBitmap,
                new RectangleF(digitWidth * 1, 0, digitWidth, digitHeight),
                new RectangleF(0, digitHeight * position010, digitWidth, digitHeight),
                GraphicsUnit.Pixel);
            e.Graphics.DrawImage(wheelBitmap,
                new RectangleF(digitWidth * 2, 0, digitWidth, digitHeight),
                new RectangleF(0, digitHeight * position001, digitWidth, digitHeight),
                GraphicsUnit.Pixel);
        }
    }
}
