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
    public partial class CassetteButton : UserControl
    {
        public CassetteButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
        }

        public enum State
        {
            UP, //pause when paused
            PRESSED_UP_DOWN, //pause when pausing
            DOWN, //pause when unpaused
            PRESSED_DOWN_UP //pause when unpausing
        }

        public enum Type
        {
            NOT_LOCKING, //stop/eject
            LOCKING, //play
            BISTABLE, //pause
        }

        public State ButtonState { get; set; } = State.UP;
        public Type ButtonType { get; set; } = Type.BISTABLE;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush b = Brushes.Red;

            switch (ButtonState)
            {
                case State.UP: b = Brushes.LightGray; break;
                case State.PRESSED_UP_DOWN: b = Brushes.DarkRed; break;
                case State.DOWN: b = Brushes.Gray; break;
                case State.PRESSED_DOWN_UP: b = Brushes.DarkOrange; break;
            }

            e.Graphics.FillRectangle(b, new RectangleF(0, 0, Width, Height));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            switch (ButtonState)
            {
                case State.UP: ButtonState = State.PRESSED_UP_DOWN; break;
                case State.DOWN: ButtonState = State.PRESSED_DOWN_UP; break;
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (ButtonType == Type.NOT_LOCKING)
            {
                switch (ButtonState)
                {
                    case State.PRESSED_UP_DOWN: ButtonState = State.UP; break;
                    case State.PRESSED_DOWN_UP: ButtonState = State.DOWN; break;
                }
            }
            else if (ButtonType == Type.LOCKING)
            {
                switch (ButtonState)
                {
                    case State.PRESSED_UP_DOWN: ButtonState = State.DOWN; break;
                    case State.PRESSED_DOWN_UP: ButtonState = State.DOWN; break;
                }
            }
            else if (ButtonType == Type.BISTABLE)
            {
                switch (ButtonState)
                {
                    case State.PRESSED_UP_DOWN: ButtonState = State.DOWN; break;
                    case State.PRESSED_DOWN_UP: ButtonState = State.UP; break;
                }
            }

            Invalidate();
        }
    }
}
