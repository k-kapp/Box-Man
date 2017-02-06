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
    public class TextField : Clickable 
    {
        Rectangle _inserterRect;

        Color _inserterColor;

        int _insertPos = 0;
        bool _showInserter = false;

        int _margin = 2;

        SpriteFont _font;
        Vector2 _textStartPos;

        string _text;

        RenderTarget2D _renderTarget;

        Texture2D _baseTexture;

        float _scale;

        bool _keyboardPrevDown = false;

        public TextField(int x, int y, int width, int height, XNAForm parent) : base(x, y, width, height, ClickType.DOWN, parent)
        {
            _text = "";

            _font = _gameMgr.Content.Load<SpriteFont>("Courier New");

            _setScale();

            _inserterColor = Color.Gray;

            _inserterRect.X = _margin;
            _inserterRect.Y = _margin;
            _inserterRect.Width = (int)(_font.MeasureString("W").X * _scale);
            _inserterRect.Height = height - 2 * _margin;

            _textStartPos = new Vector2(_margin, _margin);

            _baseTexture = Utilities.MakeTexture(Color.White, _gameMgr.GraphicsDevice);
            _background = _baseTexture;

            EventCalls += _showInserterCallback;

            _activeCol = Color.LightBlue;
            _inactiveCol = Color.LightBlue;
            _drawCol = Color.LightBlue;
        }

        private void _setScale()
        {
            int baseTextHeight = (int)_font.MeasureString("W").Y;
            int desiredHeight = Height - 2 * _margin;

            _scale = ((float)desiredHeight) / baseTextHeight;
        }

        private void _showInserterCallback(object sender, ButtonEventArgs args)
        {
            Parent.DeactivateAllTextFields();
            Active = true;
        }

        private void _updateInserter()
        {
            _inserterRect.X = (int)(_font.MeasureString(_text).X * _scale) + _margin;
        }

        public bool Active
        {
            set
            {
                _showInserter = value;
            }

            get
            {
                return _showInserter;
            }
        }

        private void _handleInput()
        {
            if (!_showInserter)
                return;
            List<char> charsPressed = Utilities.GetKeyboardInput();
            if (!_keyboardPrevDown)
            {
                foreach(char ch in charsPressed)
                {
                    if ((Keys)ch == Keys.Back)
                    {
                        if (_text.Length > 0)
                        {
                            _text = _text.Remove(_text.Length - 1);
                            _insertPos -= 1;
                        }
                    }
                    else
                    {
                        _text += ch;
                        _insertPos += 1;
                    }
                    _updateInserter();
                }
            }
            if (charsPressed.Count > 0)
            {
                _keyboardPrevDown = true;
            }
            else
            {
                _keyboardPrevDown = false;
            }
        }

        protected override void _drawMisc()
        {
            base._drawMisc();
            if (_showInserter)
            {
                _gameMgr.DrawSprite(_baseTexture, _inserterRect, _inserterColor);
            }
            _gameMgr.DrawString(_font, _text, _textStartPos, Color.Black, _scale);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Update()
        {
            base.Update();

            _handleInput();
        }

        public string Text
        {
            get
            {
                string copied = "";
                foreach(var ch in _text)
                {
                    copied += ch;
                }

                return copied;
            }


            set
            {
                _text = value;
                _updateInserter();
            }
        }
    }
}
