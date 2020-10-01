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
    public class CassetteButtons : Control
    {
        public class Button
        {
            public enum State
            {
                UP, //pause when unpaused
                PRESSED_UP_DOWN, //pause when pausing
                DOWN, //pause when paused
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

            public event Action<CancelEventArgs> MouseDown;
            public event Action MouseUp;

            public void OnMouseDown(MouseEventArgs e)
            {
                var cancel = new CancelEventArgs();
                MouseDown?.Invoke(cancel);

                if (!cancel.Cancel)
                {
                    switch (ButtonState)
                    {
                        case State.UP: ButtonState = State.PRESSED_UP_DOWN; break;
                        case State.DOWN: ButtonState = State.PRESSED_DOWN_UP; break;
                    }
                }
            }

            public void OnMouseUp(MouseEventArgs e)
            {
                MouseUp?.Invoke();

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

            Size buttonSize = new Size(65, 65);
            Point buttonsOrigin = new Point(25, 25);
            int offset = 60;

            RecButton.Location = new Point(buttonsOrigin.X + offset * 0, buttonsOrigin.Y);
            PlayButton.Location = new Point(buttonsOrigin.X + offset * 1, buttonsOrigin.Y);
            RewButton.Location = new Point(buttonsOrigin.X + offset * 2, buttonsOrigin.Y);
            FfButton.Location = new Point(buttonsOrigin.X + offset * 3, buttonsOrigin.Y);
            StopEjectButton.Location = new Point(buttonsOrigin.X + offset * 4, buttonsOrigin.Y);
            PauseButton.Location = new Point(buttonsOrigin.X + offset * 5, buttonsOrigin.Y);

            RecButton.Size = PlayButton.Size = RewButton.Size
                = FfButton.Size = StopEjectButton.Size = PauseButton.Size = buttonSize;

            RecButton.ButtonType = Button.Type.LOCKING;
            PlayButton.ButtonType = Button.Type.LOCKING;
            RewButton.ButtonType = Button.Type.LOCKING;
            FfButton.ButtonType = Button.Type.LOCKING;
            StopEjectButton.ButtonType = Button.Type.NOT_LOCKING;
            PauseButton.ButtonType = Button.Type.BISTABLE;

            buttons.AddRange(new Button[] { RecButton, PlayButton, RewButton, FfButton, StopEjectButton, PauseButton });
        }

        private List<Button> buttons = new List<Button>();

        private Button buttonPendingMouseUp = null;

        internal Button RecButton { get; set; } = new Button();
        internal Button PlayButton { get; set; } = new Button();
        internal Button RewButton { get; set; } = new Button();
        internal Button FfButton { get; set; } = new Button();
        internal Button StopEjectButton { get; set; } = new Button();
        internal Button PauseButton { get; set; } = new Button();

        private Pen borderPen = new Pen(Color.FromArgb(0, 0, 0));
        private Brush coverBrush = new SolidBrush(Color.FromArgb(169, 169, 169));
        private Brush buttonFaceBrush = new SolidBrush(Color.FromArgb(169, 169, 169));
        private Brush buttonTopBrush = new SolidBrush(Color.FromArgb(211, 211, 211));
        private Brush buttonLeftBrush = new SolidBrush(Color.FromArgb(128, 128, 128));

        private Brush symbolBlackBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
        private Brush symbolRedBrush = new SolidBrush(Color.FromArgb(255, 0, 0));

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            const int depthUp = 15;
            const int depthDown = 6;
            const int depthPressed = 3;

            int depth;

            Rectangle hole = new Rectangle(28, 28, 354, 54);
            //cover bottom
            e.Graphics.FillRectangle(coverBrush, 0, hole.Bottom, Width, Height - hole.Bottom);
            e.Graphics.DrawLine(borderPen, hole.Left, hole.Bottom, hole.Right, hole.Bottom);
            //cover right
            e.Graphics.FillRectangle(coverBrush, hole.Right, 0, Width - hole.Right, Height);
            e.Graphics.DrawLine(borderPen, hole.Right, hole.Top, hole.Right, hole.Bottom);

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

                var faceRect = new Rectangle(button.Location.X + depth, button.Location.Y + depth, buttonFaceWidth, buttonFaceHeight);

                //front face
                e.Graphics.FillRectangle(buttonFaceBrush, faceRect);
                e.Graphics.DrawRectangle(borderPen, faceRect);

                //symbols
                var symbolRect = new Rectangle(faceRect.Left + faceRect.Width / 2, faceRect.Top + faceRect.Height / 2,
                    buttonFaceWidth / 4, buttonFaceHeight / 4);
                symbolRect.X -= symbolRect.Width / 2;
                symbolRect.Y -= symbolRect.Height / 2;

                if (button == RecButton)
                {
                    e.Graphics.FillEllipse(symbolRedBrush, symbolRect);
                }
                else if (button == PlayButton)
                {
                    e.Graphics.FillPolygon(symbolBlackBrush, new PointF[]
                    {
                        new PointF(symbolRect.Left, symbolRect.Top),
                        new PointF(symbolRect.Left, symbolRect.Bottom),
                        new PointF(symbolRect.Right, symbolRect.Top + symbolRect.Width / 2),
                    });
                }
                else if (button == RewButton)
                {
                    e.Graphics.FillPolygon(symbolBlackBrush, new PointF[]
                    {
                        new PointF(symbolRect.Right + symbolRect.Width / 2, symbolRect.Top),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Top + symbolRect.Width / 2),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Top),
                        new PointF(symbolRect.Left - symbolRect.Width / 2, symbolRect.Top + symbolRect.Width / 2),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Bottom),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Top + symbolRect.Width / 2),
                        new PointF(symbolRect.Right + symbolRect.Width / 2, symbolRect.Bottom),
                    });
                }
                else if (button == FfButton)
                {
                    e.Graphics.FillPolygon(symbolBlackBrush, new PointF[]
                    {
                        new PointF(symbolRect.Left - symbolRect.Width / 2, symbolRect.Top),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Top + symbolRect.Width / 2),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Top),
                        new PointF(symbolRect.Right + symbolRect.Width / 2, symbolRect.Top + symbolRect.Width / 2),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Bottom),
                        new PointF(symbolRect.Right - symbolRect.Width / 2, symbolRect.Top + symbolRect.Width / 2),
                        new PointF(symbolRect.Left - symbolRect.Width / 2, symbolRect.Bottom),
                    });
                }
                else if (button == StopEjectButton)
                {
                    var leftSymbolRect = symbolRect;
                    leftSymbolRect.X -= symbolRect.Width;

                    var rightSymbolRect = symbolRect;
                    rightSymbolRect.X += symbolRect.Width;

                    e.Graphics.FillRectangle(symbolBlackBrush, leftSymbolRect);

                    e.Graphics.FillPolygon(symbolBlackBrush, new PointF[]
                    {
                        new PointF(rightSymbolRect.Left + rightSymbolRect.Width / 2, rightSymbolRect.Top),
                        new PointF(rightSymbolRect.Left - 1, rightSymbolRect.Top + rightSymbolRect.Height / 2 + 1),
                        new PointF(rightSymbolRect.Right + 1, rightSymbolRect.Top + rightSymbolRect.Height / 2 + 1),
                    });
                    e.Graphics.FillRectangle(symbolBlackBrush, rightSymbolRect.Left, rightSymbolRect.Top + 3 * rightSymbolRect.Height / 4,
                        rightSymbolRect.Width, rightSymbolRect.Height / 4);
                }
                else if (button == PauseButton)
                {
                    e.Graphics.FillRectangle(symbolBlackBrush, symbolRect.Left, symbolRect.Top,
                        symbolRect.Width / 3, symbolRect.Height);
                    e.Graphics.FillRectangle(symbolBlackBrush, symbolRect.Left + 2 * symbolRect.Width / 3, symbolRect.Top,
                        symbolRect.Width / 3, symbolRect.Height);
                }

                var topPolygon = new PointF[]
                {
                new PointF(button.Location.X, button.Location.Y),
                new PointF(button.Location.X + buttonFaceWidth, button.Location.Y),
                new PointF(button.Location.X + buttonFaceWidth + depth, button.Location.Y + depth),
                new PointF(button.Location.X + depth, button.Location.Y + depth),
                };

                //top face
                e.Graphics.FillPolygon(buttonTopBrush, topPolygon);
                e.Graphics.DrawPolygon(borderPen, topPolygon);

                var leftPolygon = new PointF[]
                {
                new PointF(button.Location.X, button.Location.Y),
                new PointF(button.Location.X, button.Location.Y + buttonFaceHeight),
                new PointF(button.Location.X + depth, button.Location.Y + buttonFaceHeight + depth),
                new PointF(button.Location.X + depth, button.Location.Y + depth),
                };

                //left face
                e.Graphics.FillPolygon(buttonLeftBrush, leftPolygon);
                e.Graphics.DrawPolygon(borderPen, leftPolygon);
            }

            //cover left
            e.Graphics.FillRectangle(coverBrush, 0, 0, hole.Left, Height);
            e.Graphics.DrawLine(borderPen, hole.Left, hole.Top, hole.Left, hole.Bottom);
            //cover top
            e.Graphics.FillRectangle(coverBrush, 0, 0, Width, hole.Top);
            e.Graphics.DrawLine(borderPen, hole.Left, hole.Top, hole.Right, hole.Top);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var button in buttons)
            {
                if (new Rectangle(button.Location, button.Size).Contains(e.Location))
                {
                    button.OnMouseDown(e);
                    buttonPendingMouseUp = button;
                    break;
                }
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (buttonPendingMouseUp != null)
            {
                buttonPendingMouseUp.OnMouseUp(null);
                buttonPendingMouseUp = null;
            }

            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (buttonPendingMouseUp != null)
            {
                buttonPendingMouseUp.OnMouseUp(null);
                buttonPendingMouseUp = null;
            }

            Invalidate();
        }
    }
}
