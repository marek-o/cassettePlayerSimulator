using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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

            RecButton.ButtonType = Button.Type.LOCKING;
            PlayButton.ButtonType = Button.Type.LOCKING;
            RewButton.ButtonType = Button.Type.LOCKING;
            FfButton.ButtonType = Button.Type.LOCKING;
            StopEjectButton.ButtonType = Button.Type.NOT_LOCKING;
            PauseButton.ButtonType = Button.Type.BISTABLE;

            buttons.AddRange(new Button[] { RecButton, PlayButton, RewButton, FfButton, StopEjectButton, PauseButton });

            DoLayout();
        }

        private List<Button> buttons = new List<Button>();

        private Button buttonPendingMouseUp = null;

        internal Button RecButton { get; set; } = new Button();
        internal Button PlayButton { get; set; } = new Button();
        internal Button RewButton { get; set; } = new Button();
        internal Button FfButton { get; set; } = new Button();
        internal Button StopEjectButton { get; set; } = new Button();
        internal Button PauseButton { get; set; } = new Button();

        private Rectangle hole;
        private float depthUp;
        private float depthDown;
        private float depthPressed;

        private Point perspectiveCenter;

        private void DoLayout()
        {
            Scaler scaler = new Scaler(Width / 416.0f);

            Size buttonSize = scaler.S(new Size(50, 50));
            Point buttonsOrigin = scaler.S(new Point(40, 25));
            int offset = scaler.S(55);
            
            RecButton.Location = new Point(buttonsOrigin.X + offset * 0, buttonsOrigin.Y);
            PlayButton.Location = new Point(buttonsOrigin.X + offset * 1, buttonsOrigin.Y);
            RewButton.Location = new Point(buttonsOrigin.X + offset * 2, buttonsOrigin.Y);
            FfButton.Location = new Point(buttonsOrigin.X + offset * 3, buttonsOrigin.Y);
            StopEjectButton.Location = new Point(buttonsOrigin.X + offset * 4, buttonsOrigin.Y);
            PauseButton.Location = new Point(buttonsOrigin.X + offset * 5, buttonsOrigin.Y);

            RecButton.Size = PlayButton.Size = RewButton.Size
                = FfButton.Size = StopEjectButton.Size = PauseButton.Size = buttonSize;

            perspectiveCenter = new Point(RewButton.Location.X + (offset + buttonSize.Width) / 2, buttonsOrigin.Y + buttonSize.Height / 2);

            depthUp = scaler.S(5);
            depthDown = scaler.S(1.5f);
            depthPressed = scaler.S(0);

            hole = scaler.S(new Rectangle(36, 21, 333, 58));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            DoLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float depth;

            //cover bottom
            e.Graphics.FillRectangle(Common.CoverBrush, 0, hole.Bottom, Width, Height - hole.Bottom);
            e.Graphics.DrawLine(Common.BorderPen, hole.Left, hole.Bottom, hole.Right, hole.Bottom);
            //cover right
            e.Graphics.FillRectangle(Common.CoverBrush, hole.Right, 0, Width - hole.Right, Height);
            e.Graphics.DrawLine(Common.BorderPen, hole.Right, hole.Top, hole.Right, hole.Bottom);

            //cover left
            e.Graphics.FillRectangle(Common.CoverBrush, 0, 0, hole.Left, Height);
            e.Graphics.DrawLine(Common.BorderPen, hole.Left, hole.Top, hole.Left, hole.Bottom);
            //cover top
            e.Graphics.FillRectangle(Common.CoverBrush, 0, 0, Width, hole.Top);
            e.Graphics.DrawLine(Common.BorderPen, hole.Left, hole.Top, hole.Right, hole.Top);

            //TODO: simplify cover drawing

            //from outer to inner
            foreach (int buttonIndex in new int[] { 0, 5, 1, 4, 2, 3 })
            {
                var button = buttons[buttonIndex];

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

                //debug:
                //depth = depthPressed;

                var faceRectBase = new Rectangle(button.Location.X, button.Location.Y, button.Size.Width, button.Size.Height);

                var faceLTBase = new Point(faceRectBase.Left, faceRectBase.Top);
                var faceRBBase = new Point(faceRectBase.Right, faceRectBase.Bottom);

                var faceLT = Perspective(faceLTBase, depth);
                var faceRB = Perspective(faceRBBase, depth);

                var faceRect = new Rectangle(faceLT.X, faceLT.Y, faceRB.X - faceLT.X, faceRB.Y - faceLT.Y);

                //front face
                e.Graphics.FillRectangle(Common.ButtonFaceBrush, faceRect);
                e.Graphics.DrawRectangle(Common.BorderPen, faceRect);

                //symbols
                var symbolRect = new Rectangle(faceRect.Left + faceRect.Width / 2, faceRect.Top + faceRect.Height / 2,
                    faceRect.Width / 4, faceRect.Height / 4);
                symbolRect.X -= symbolRect.Width / 2;
                symbolRect.Y -= symbolRect.Height / 2;

                if (button == RecButton)
                {
                    e.Graphics.FillEllipse(Common.SymbolRedBrush, symbolRect);
                }
                else if (button == PlayButton)
                {
                    e.Graphics.FillPolygon(Common.SymbolBlackBrush, new PointF[]
                    {
                        new PointF(symbolRect.Left, symbolRect.Top),
                        new PointF(symbolRect.Left, symbolRect.Bottom),
                        new PointF(symbolRect.Right, symbolRect.Top + symbolRect.Width / 2),
                    });
                }
                else if (button == RewButton)
                {
                    e.Graphics.FillPolygon(Common.SymbolBlackBrush, new PointF[]
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
                    e.Graphics.FillPolygon(Common.SymbolBlackBrush, new PointF[]
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

                    e.Graphics.FillRectangle(Common.SymbolBlackBrush, leftSymbolRect);

                    e.Graphics.FillPolygon(Common.SymbolBlackBrush, new PointF[]
                    {
                        new PointF(rightSymbolRect.Left + rightSymbolRect.Width / 2, rightSymbolRect.Top),
                        new PointF(rightSymbolRect.Left - 1, rightSymbolRect.Top + rightSymbolRect.Height / 2 + 1),
                        new PointF(rightSymbolRect.Right + 1, rightSymbolRect.Top + rightSymbolRect.Height / 2 + 1),
                    });
                    e.Graphics.FillRectangle(Common.SymbolBlackBrush, rightSymbolRect.Left, rightSymbolRect.Top + 3 * rightSymbolRect.Height / 4,
                        rightSymbolRect.Width, rightSymbolRect.Height / 4);
                }
                else if (button == PauseButton)
                {
                    e.Graphics.FillRectangle(Common.SymbolBlackBrush, symbolRect.Left, symbolRect.Top,
                        symbolRect.Width / 3, symbolRect.Height);
                    e.Graphics.FillRectangle(Common.SymbolBlackBrush, symbolRect.Left + 2 * symbolRect.Width / 3, symbolRect.Top,
                        symbolRect.Width / 3, symbolRect.Height);
                }

                var leftPolygon = new PointF[]
                {
                    faceLT,
                    new PointF(faceLT.X, faceRB.Y),
                    new PointF(faceLTBase.X, faceRBBase.Y),
                    faceLTBase
                };
                var rightPolygon = new PointF[]
                {
                    faceRB,
                    new PointF(faceRB.X, faceLT.Y),
                    new PointF(faceRBBase.X, faceLTBase.Y),
                    faceRBBase
                };

                if (buttonIndex > 2)
                { 
                    //left face
                    e.Graphics.FillPolygon(Common.ButtonLeftBrush, leftPolygon);
                    e.Graphics.DrawPolygon(Common.BorderPen, leftPolygon);
                }
                else
                {
                    //right face
                    e.Graphics.FillPolygon(Common.ButtonRightBrush, rightPolygon);
                    e.Graphics.DrawPolygon(Common.BorderPen, rightPolygon);
                }
            }

            //debug
            e.Graphics.FillEllipse(Brushes.Red, perspectiveCenter.X - 2, perspectiveCenter.Y - 2, 4, 4);
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
        }

        private Point Perspective(Point input, float depth)
        {
            PointF v = new PointF(input.X - perspectiveCenter.X, input.Y - perspectiveCenter.Y);

            PointF delta = new PointF(v.X * depth / Math.Abs(v.Y), v.Y * depth / Math.Abs(v.Y));

            return new Point((int)(input.X + delta.X), (int)(input.Y + delta.Y));
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
