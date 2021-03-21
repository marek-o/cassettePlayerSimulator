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
    public class PositionSelectorControl : Control
    {
        public float TotalLengthSeconds { get; set; }

        private List<Marker> markers = new List<Marker>();

        private Marker beginMarker = new Marker(0, false);
        private Marker endMarker = new Marker(0, false);
        private Marker selectionBeginMarker = new Marker(0, true);
        private Marker selectionEndMarker = new Marker(0, true);

        private int margin = 16;

        public PositionSelectorControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                TotalLengthSeconds = 2700;

                endMarker.Position = TotalLengthSeconds;
                selectionBeginMarker.Position = 900;
                selectionEndMarker.Position = 1080;

                markers.Clear();
                markers.Add(beginMarker);
                markers.Add(endMarker);
                markers.Add(selectionBeginMarker);
                markers.Add(selectionEndMarker);
            }

            base.OnPaint(e);

            e.Graphics.Clear(SystemColors.Control);

            var barRect = BarRectangle();

            e.Graphics.FillRectangle(Brushes.LightYellow, barRect);
            e.Graphics.DrawRectangle(Pens.Black, barRect);

            foreach (var marker in markers)
            {
                int x = WorldToScreen(marker.Position);

                e.Graphics.DrawLine(Pens.Black, x, barRect.Top, x, barRect.Bottom);
                string text = Common.FormatTime((int)marker.Position);

                var textSize = TextRenderer.MeasureText(text, Font);

                TextRenderer.DrawText(e.Graphics, text, Font,
                    new Rectangle(x - textSize.Width / 2, barRect.Bottom, textSize.Width, textSize.Height),
                    Color.Black, TextFormatFlags.HorizontalCenter);
            }
        }

        private Rectangle BarRectangle()
        {
            return new Rectangle(margin, margin, Width - 2 * margin, Height - 2 * margin);
        }

        private int WorldToScreen(float position)
        {
            var fraction = position / TotalLengthSeconds;
            var barRect = BarRectangle();
            return barRect.Left + (int)(barRect.Width * fraction);
        }

        private class Marker
        {
            public float Position;
            public bool IsAbove;

            public Marker(float position, bool isAbove)
            {
                Position = position;
                IsAbove = isAbove;
            }
        }
    }
}
