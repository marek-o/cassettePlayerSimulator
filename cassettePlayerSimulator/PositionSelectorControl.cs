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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float TotalLengthSeconds { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float SelectionPosition { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float SelectionLength { get; set; }

        private List<Marker> markers = new List<Marker>();

        private Marker beginMarker = new Marker(0, false);
        private Marker endMarker = new Marker(0, false);
        private Marker selectionBeginMarker = new Marker(0, true);
        private Marker selectionEndMarker = new Marker(0, true);

        private int marginHorizontal = 32;
        private int marginVertical = 16;

        public PositionSelectorControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            markers.Add(beginMarker);
            markers.Add(endMarker);
            markers.Add(selectionBeginMarker);
            markers.Add(selectionEndMarker);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(SystemColors.Control);

            var barRect = BarRectangle();

            e.Graphics.FillRectangle(Brushes.LightGray, barRect);

            endMarker.Position = TotalLengthSeconds;
            selectionBeginMarker.Position = SelectionPosition;
            selectionEndMarker.Position = SelectionPosition + SelectionLength;

            //layout
            foreach (var marker in markers)
            {
                int x = WorldToScreen(marker.Position);

                string text = Common.FormatTime((int)marker.Position);

                var textSize = TextRenderer.MeasureText(text, Font);
                int y = marker.IsAbove ? barRect.Top - textSize.Height : barRect.Bottom;

                marker.ScreenPosition = x;
                marker.ScreenRectangle = new Rectangle(x - textSize.Width / 2, y, textSize.Width, textSize.Height);
            }

            //selection bar
            e.Graphics.FillRectangle(Brushes.Yellow,
                new Rectangle(selectionBeginMarker.ScreenPosition, barRect.Top,
                selectionEndMarker.ScreenPosition - selectionBeginMarker.ScreenPosition, barRect.Height));

            Rectangle overflowBegin = new Rectangle(selectionBeginMarker.ScreenPosition, barRect.Top,
                barRect.Left - selectionBeginMarker.ScreenPosition, barRect.Height);
            Rectangle overflowEnd = new Rectangle(barRect.Right, barRect.Top,
                selectionEndMarker.ScreenPosition - barRect.Right, barRect.Height);

            e.Graphics.FillRectangle(Brushes.Red, overflowBegin);
            e.Graphics.FillRectangle(Brushes.Red, overflowEnd);
            e.Graphics.DrawRectangle(Pens.Black, overflowBegin);
            e.Graphics.DrawRectangle(Pens.Black, overflowEnd);

            //fix overlapping markers
            if (selectionBeginMarker.ScreenRectangle.IntersectsWith(selectionEndMarker.ScreenRectangle))
            {
                int overlap = selectionBeginMarker.ScreenRectangle.Right - selectionEndMarker.ScreenRectangle.Left;
                selectionBeginMarker.ScreenRectangle.Offset(-overlap / 2, 0);
                selectionEndMarker.ScreenRectangle.Offset(overlap / 2, 0);
            }

            int markerOffsetValue = 0;

            if (selectionBeginMarker.ScreenRectangle.Left < 0 || selectionEndMarker.ScreenRectangle.Left < 0)
            {
                markerOffsetValue = -selectionBeginMarker.ScreenRectangle.Left;
            }

            if (selectionBeginMarker.ScreenRectangle.Right >= Width || selectionEndMarker.ScreenRectangle.Right >= Width)
            {
                markerOffsetValue = -(selectionEndMarker.ScreenRectangle.Right - Width);
            }

            selectionBeginMarker.ScreenRectangle.Offset(markerOffsetValue, 0);
            selectionEndMarker.ScreenRectangle.Offset(markerOffsetValue, 0);

            //paint
            foreach (var marker in markers)
            {
                e.Graphics.DrawLine(Pens.Black, marker.ScreenPosition, barRect.Top, marker.ScreenPosition, barRect.Bottom);
                string text = Common.FormatTime((int)marker.Position);

                TextRenderer.DrawText(e.Graphics, text, Font,
                    marker.ScreenRectangle,
                    Color.Black, TextFormatFlags.HorizontalCenter);
            }

            e.Graphics.DrawRectangle(Pens.Black, barRect);
        }

        private Rectangle BarRectangle()
        {
            return new Rectangle(marginHorizontal, marginVertical, Width - 2 * marginHorizontal, Height - 2 * marginVertical);
        }

        private int WorldToScreen(float position)
        {
            var fraction = position / TotalLengthSeconds;

            if (TotalLengthSeconds == 0)
            {
                fraction = 0;
            }

            var barRect = BarRectangle();
            return barRect.Left + (int)(barRect.Width * fraction);
        }

        private class Marker
        {
            public float Position;
            public bool IsAbove;

            public int ScreenPosition;
            public Rectangle ScreenRectangle;

            public Marker(float position, bool isAbove)
            {
                Position = position;
                IsAbove = isAbove;
            }
        }
    }
}
