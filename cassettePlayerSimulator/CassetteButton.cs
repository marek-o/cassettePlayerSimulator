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

            const int depthUp = 18;
            const int depthDown = 6;
            const int depthPressed = 3;

            int depth;

            if (ButtonState == State.UP)
            {
                depth = depthUp;
            }
            else if (ButtonState == State.DOWN)
            {
                depth = depthDown;
            }
            else
            {
                depth = depthPressed;
            }

            int buttonFaceWidth = Width - depthUp;
            int buttonFaceHeight = Height - depthUp;

            e.Graphics.FillRectangle(Brushes.DarkGray, depth, depth, buttonFaceWidth, buttonFaceHeight);

            //top face
            e.Graphics.FillPolygon(Brushes.LightGray, new PointF[]
            {
                new PointF(0, 0),
                new PointF(buttonFaceWidth, 0),
                new PointF(buttonFaceWidth + depth, depth),
                new PointF(depth, depth),
            });

            //left face
            e.Graphics.FillPolygon(Brushes.Gray, new PointF[]
            {
                new PointF(0, 0),
                new PointF(0, buttonFaceHeight),
                new PointF(depth, buttonFaceHeight + depth),
                new PointF(depth, depth),
            });
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
