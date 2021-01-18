using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoxMan
{
    public enum ClickType
    {
        RELEASE,
        DOWN,
        DRAG
    }

    public class ButtonEventArgs : EventArgs
    {
        public List<Clickable> Clickables;
        public List<XNAForm> forms;

        public void AddClickable(Clickable clickable)
        {
            if (Clickables == null)
            {
                Clickables = new List<Clickable>();
            }
            Clickables.Add(clickable);
        }

        public void AddForm(XNAForm form)
        {
            if (forms == null)
            {
                forms = new List<XNAForm>();
            }
            forms.Add(form);
        }
    }

    public abstract class Clickable
    {
        ButtonEventArgs _eventArgs;
        public delegate void ButtonClickCallback(object caller, ButtonEventArgs args);

        public event ButtonClickCallback EventCalls = delegate { };

        protected Rectangle _mainRect;
        protected Rectangle _drawRect;
        protected GameMgr _gameMgr;
        protected XNAForm _parent;


        protected RenderTarget2D _renderTargetTemp;
        protected RenderTarget2D _renderTarget;

        protected bool held;

        protected bool _clickedUp;
        protected bool _clickedDown;

        protected ClickType _type;

        protected Texture2D _background;
        protected Color _drawCol, _activeCol, _inactiveCol;
        protected bool active;

        public Clickable(int x, int y, int width, int height, ClickType type, XNAForm parent)
        {
            _eventArgs = new BoxMan.ButtonEventArgs();

            _type = type;

            active = false;

            _drawCol = Color.White;
            _activeCol = Color.White;
            _inactiveCol = Color.White;

            _mainRect = new Rectangle(x, y, width, height);
            _drawRect = new Rectangle(0, 0, width, height);

            _parent = parent;
            _gameMgr = _parent.GameMgr;

            held = false;
            new RenderTarget2D(_gameMgr.GraphicsDevice, width, height);

            _renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, width, height, false, _gameMgr.GraphicsDevice.PresentationParameters.BackBufferFormat, 
                                                _gameMgr.GraphicsDevice.PresentationParameters.DepthStencilFormat, _gameMgr.GraphicsDevice.PresentationParameters.MultiSampleCount,  RenderTargetUsage.PreserveContents);

            parent.AddClickable(this);
        }

        public Clickable(Clickable other, XNAForm parent) : this(other.X, other.Y, other.Width, other.Height, other._type, parent)
        {
            _drawCol = other._drawCol;
            _activeCol = other._activeCol;
            _inactiveCol = other._inactiveCol;
            BackgroundTexture = other.BackgroundTexture;
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

        public ButtonEventArgs ButtonEventArgs
        {
            get
            {
                return _eventArgs;
            }

            set
            {
                _eventArgs = value;
            }
        }


        public virtual void OnClick()
        {
            EventCalls(this, _eventArgs);
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

        protected virtual void _drawMisc()
        {

        }

        public virtual void Draw()
        {
            _renderTargetTemp = _parent.RenderTarget;

            _gameMgr.SpriteBatch.End();
            _gameMgr.SetRenderTarget(_renderTarget);
            _gameMgr.SpriteBatch.Begin();

            _parent.GameMgr.DrawSprite(_background, _drawRect, _drawCol);

            _drawMisc();

            _gameMgr.SpriteBatch.End();
            _gameMgr.SetRenderTarget(_renderTargetTemp);
            _gameMgr.SpriteBatch.Begin();

            _gameMgr.DrawSprite(_renderTarget, _mainRect, Color.White);
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
                _renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, value, Height, false, _gameMgr.GraphicsDevice.PresentationParameters.BackBufferFormat, 
                                                    _gameMgr.GraphicsDevice.PresentationParameters.DepthStencilFormat, _gameMgr.GraphicsDevice.PresentationParameters.MultiSampleCount,  RenderTargetUsage.PreserveContents);
                _drawRect.Width = value;
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
                _renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, Width, value, false, _gameMgr.GraphicsDevice.PresentationParameters.BackBufferFormat, 
                                                    _gameMgr.GraphicsDevice.PresentationParameters.DepthStencilFormat, _gameMgr.GraphicsDevice.PresentationParameters.MultiSampleCount,  RenderTargetUsage.PreserveContents);
                _drawRect.Height = value;
                _mainRect.Height = value;
            }

            get
            {
                return _mainRect.Height;
            }
        }

    }
}
