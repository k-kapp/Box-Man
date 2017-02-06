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

    
    public class Button : Clickable
    {

        int _horizOffset = 0;
        int _vertOffset = 0;

        string _text;

        Color _stringCol;

        Vector2 _stringPos;

        SpriteFont _font;


        int _textHeight, _textWidth;

        float _scale = 1.5f;

        public bool MouseButtonReleaseEvent = true;

        public Button(string text, int x, int y, int width, int height, XNAForm parent) : base(x, y, width, height, ClickType.RELEASE, parent)
        {
            _gameMgr = _parent.GameMgr;

            _text = text;

            Initialize();
        }

        public Button(string text, int x, int y, int width, int height, ButtonClickCallback callback, XNAForm parent) : this(text, x, y, width, height, parent)
        {
            EventCalls += callback;
        }

        public void newBackground(Texture2D texture)
        {
            _background = texture;
        }

        public void newActiveColor(Color color)
        {
            _activeCol = color;
        }

        public void newInactiveColor(Color color)
        {
            _inactiveCol = color;
        }

        public override int X
        {
            set
            {
                base.X = value;
                updateStringPos();
            }
        }

        public override int Y
        {
            set
            {
                base.Y = value;
                updateStringPos();
            }
        }

        public override int Width
        {
            set
            {
                base.Width = value;
                updateStringPos();
            }
        }

        public override int Height
        {
            set
            {
                base.Height = value;
                updateStringPos();
            }
        }

        public bool StringFitsX()
        {
            return !(_textWidth >= _mainRect.Width);
        }

        public bool StringFitsY()
        {
            return !(_textHeight >= _mainRect.Height);
        }

        public void AutoSize(double xScale = 1.3, double yScale = 1.1)
        {
            FitToStringX(xScale);
            FitToStringY(yScale);
        }

        public void FitToStringX(double scale)
        {
            Width = (int)(_textWidth * scale);
        }

        public void FitToStringY(double scale)
        {
            Height = (int)(_textHeight * scale);
        }

        protected void updateStringPos()
        {
            _textHeight = (int)(_font.MeasureString(_text).Y*_scale);
            _textWidth = (int)(_font.MeasureString(_text).X*_scale);
            //_stringPos = new Vector2(_mainRect.X + (_mainRect.Width - _textWidth)/2 + _horizOffset, _mainRect.Y + (_mainRect.Height - _textHeight)/2 + _vertOffset);
            _stringPos = new Vector2((_mainRect.Width - _textWidth)/2 + _horizOffset, (_mainRect.Height - _textHeight)/2 + _vertOffset);
        }

        protected void Initialize()
        {
            _stringCol = Color.White;
            _inactiveCol = Color.Red;
            _activeCol = Color.OrangeRed;

            _background = _gameMgr.Content.Load<Texture2D>("WhiteBlock");

            _font = _gameMgr.Content.Load<SpriteFont>("Courier New");
            _textHeight = (int)(_font.MeasureString(_text).Y*_scale);
            _textWidth = (int)(_font.MeasureString(_text).X * _scale);

            updateStringPos();
        }
        
        protected override void _drawMisc()
        {
            _gameMgr.SpriteBatch.DrawString(_font, _text, _stringPos, _stringCol, 0, new Vector2(0, 0), _scale, 0, 0);
        }

        /*
        public override void Draw()
        {
            base.Draw();
            _gameMgr.SpriteBatch.DrawString(_font, _text, _stringPos, _stringCol, 0, new Vector2(0, 0), _scale, 0, 0);
        }
        */

        public override void Update()
        {
            if (MouseOnClickable())
            {
                MakeActive();
            }
            else
            {
                MakeInactive();
            }

            base.Update();

        }

    }
}
