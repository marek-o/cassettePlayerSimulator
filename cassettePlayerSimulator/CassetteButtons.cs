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
    public partial class CassetteButtons : UserControl
    {
        public class Button
        {
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

            public Point Location { get; set; }
            public Size Size { get; set; }

            public void OnMouseDown(MouseEventArgs e)
            {
                switch (ButtonState)
                {
                    case State.UP: ButtonState = State.PRESSED_UP_DOWN; break;
                    case State.DOWN: ButtonState = State.PRESSED_DOWN_UP; break;
                }
            }

            public void OnMouseUp(MouseEventArgs e)
            {
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
            }
        }

        public CassetteButtons()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            Size buttonSize = new Size(50, 80);
            int offset = 40;

            buttons.Add(new Button() { Location = new Point(offset * 0, 0), Size = buttonSize });
            buttons.Add(new Button() { Location = new Point(offset * 1, 0), Size = buttonSize });
            buttons.Add(new Button() { Location = new Point(offset * 2, 0), Size = buttonSize });
            buttons.Add(new Button() { Location = new Point(offset * 3, 0), Size = buttonSize });
            buttons.Add(new Button() { Location = new Point(offset * 4, 0), Size = buttonSize });
            buttons.Add(new Button() { Location = new Point(offset * 5, 0), Size = buttonSize });
        }

        private List<Button> buttons = new List<Button>();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            const int depthUp = 18;
            const int depthDown = 6;
            const int depthPressed = 3;

            int depth;

            foreach (var button in buttons.Reverse<Button>())
            {
                if (button.ButtonState == Button.State.UP)
                {
                    depth = depthUp;
                }
                else if (button.ButtonState == Button.State.DOWN)
                {
                    depth = depthDown;
                }
                else
                {
                    depth = depthPressed;
                }

                int buttonFaceWidth = button.Size.Width - depthUp;
                int buttonFaceHeight = button.Size.Height - depthUp;

                e.Graphics.FillRectangle(Brushes.DarkGray, button.Location.X + depth, button.Location.Y + depth, buttonFaceWidth, buttonFaceHeight);

                //top face
                e.Graphics.FillPolygon(Brushes.LightGray, new PointF[]
                {
                new PointF(button.Location.X, button.Location.Y),
                new PointF(button.Location.X + buttonFaceWidth, button.Location.Y),
                new PointF(button.Location.X + buttonFaceWidth + depth, button.Location.Y + depth),
                new PointF(button.Location.X + depth, button.Location.Y + depth),
                });

                //left face
                e.Graphics.FillPolygon(Brushes.Gray, new PointF[]
                {
                new PointF(button.Location.X, button.Location.Y),
                new PointF(button.Location.X, button.Location.Y + buttonFaceHeight),
                new PointF(button.Location.X + depth, button.Location.Y + buttonFaceHeight + depth),
                new PointF(button.Location.X + depth, button.Location.Y + depth),
                });
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var button in buttons)
            {
                if (new Rectangle(button.Location, button.Size).Contains(e.Location))
                {
                    button.OnMouseDown(e);
                    break;
                }
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            foreach (var button in buttons)
            {
                if (new Rectangle(button.Location, button.Size).Contains(e.Location))
                {
                    button.OnMouseUp(e);
                    break;
                }
            }

            Invalidate();
        }
    }
}
