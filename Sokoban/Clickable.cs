using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{
    public enum ClickType
    {
        RELEASE,
        DOWN,
        DRAG
    }

    public abstract class Clickable
    {
        public delegate void ButtonClickCallback(object caller, ButtonEventArgs args);

        public event ButtonClickCallback EventCalls = delegate { };

        protected Rectangle _mainRect;
        protected GameMgr _gameMgr;
        protected XNAForm _parent;

        protected bool held;

        protected bool _clickedUp;
        protected bool _clickedDown;

        protected ClickType _type;

        protected Texture2D _background;
        protected Color _drawCol, _activeCol, _inactiveCol;
        protected bool active;

        public Clickable(int x, int y, int width, int height, ClickType type, XNAForm parent)
        {
            _type = type;

            active = false;

            _drawCol = Color.White;
            _activeCol = Color.White;
            _inactiveCol = Color.White;

            _mainRect = new Rectangle(x, y, width, height);
            _parent = parent;
            _gameMgr = _parent.GameMgr;

            held = false;
        }

        public bool MouseOnClickable()
        {
            return Utilities.MouseOnRect(_mainRect, _parent.InnerXAbs, _parent.InnerYAbs);
        }

        public virtual void Update()
        {
            _clickedUp = clickedUp();
            _clickedDown = clickedDown();

            if (_type == ClickType.RELEASE)
            {
                if (_clickedUp)
                    OnClick();
            }
            else
            {
                if (_clickedDown)
                    OnClick();
            }

            MouseState mstate = Mouse.GetState();

            held = mstate.LeftButton == ButtonState.Pressed;
        }

        public virtual void OnClick()
        {
            ButtonEventArgs args = new ButtonEventArgs();

            EventCalls(this, args);
        }

        public bool ClickedUp
        {
            get
            {
                return _clickedUp;
            }
        }

        public bool ClickedDown
        {
            get
            {
                return _clickedDown;
            }
        }

        protected bool clickedUp()
        {
            if (!MouseOnClickable())
                return false;

            if (!held)
                return false;

            MouseState mstate = Mouse.GetState();

            if (mstate.LeftButton == ButtonState.Released)
            {
                return true;
            }
            else
                return false;
        }

        protected bool clickedDown()
        {
            if (!MouseOnClickable())
                return false;

            if (_type == ClickType.DOWN)
                if (held)
                    return false;

            MouseState mstate = Mouse.GetState();

            if (mstate.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            else
                return false;
        }

        public XNAForm Parent
        {
            get
            {
                return _parent;
            }
        }

        public virtual void Draw()
        {
            _parent.GameMgr.DrawSprite(_background, _mainRect, _drawCol);
        }

        public Texture2D BackgroundTexture
        {
            get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }

        public Color ActiveColor
        {
            get
            {
                return _activeCol;
            }

            set
            {
                _activeCol = value;
            }
        }

        public Color InactiveColor
        {
            get
            {
                return _inactiveCol;
            }

            set
            {
                _inactiveCol = value;
            }
        }

        public void MakeInactive()
        {
            active = false;
            _drawCol = _inactiveCol;
        }

        public void MakeActive()
        {
            active = true;
            _drawCol = _activeCol;
        }

        public virtual int X
        {
            set
            {
                _mainRect.X = value;
            }

            get
            {
                return _mainRect.X;
            }
        }

        public virtual int Y
        {
            set
            {
                _mainRect.Y = value;
            }

            get
            {
                return _mainRect.Y;
            }
        }

        public virtual int Width
        {
            set
            {
                _mainRect.Width = value;
            }

            get
            {
                return _mainRect.Width;
            }
        }

        public virtual int Height
        {
            set
            {
                _mainRect.Height = value;
            }

            get
            {
                return _mainRect.Height;
            }
        }

    }
}
