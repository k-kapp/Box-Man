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
    public class XNAListElement : Clickable
    {

        new XNAList _parent;

        public XNAListElement(XNAList parent) : base(0, 0, parent.ElementsWidth, parent.ElementsHeight, ClickType.DOWN, parent)
        {
            _parent = parent;

            _activeCol = Color.Gray;
            _inactiveCol = Color.White;

            _drawCol = _inactiveCol;
        }

        public XNAListElement(Texture2D background, XNAList parent) : this(parent)
        {
            _background = background;
        }

        public XNAListElement(Texture2D background, Color activeColor, Color inactiveColor, XNAList parent)
            : this(background, parent)
        {
            ActiveColor = activeColor;
            InactiveColor = inactiveColor;
        }

        public Color ActiveColor
        {
            set
            {
                _activeCol = value;

                if (active)
                    _drawCol = _activeCol;
            }
        }

        public Color InactiveColor
        {
            set
            {
                _inactiveCol = value;

                if (!active)
                    _drawCol = _inactiveCol;
            }
        }

        public Texture2D Background
        {
            private get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }

        public int X
        {
            get
            {
                return _mainRect.X;
            }

            set
            {
                _mainRect.X = value;
            }
        }

        public int Y
        {
            get
            {
                return _mainRect.Y;
            }

            set
            {
                _mainRect.Y = value;
            }
        }

        public int Width
        {
            get
            {
                return _mainRect.Width;
            }

            set
            {
                _mainRect.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return _mainRect.Height;
            }

            set
            {
                _mainRect.Height = value;
            }
        }

        public XNAForm Parent
        {
            set
            {
                var parent = value as XNAList;
                if (parent == null)
                    return;

                _parent = parent;

                base._parent = value;
            }

            get
            {
                return _parent;
            }
        }

        public void SetPosition(int index)
        {
            X = 0;
            Y = index * _parent.ElementsHeight;
        }

        public void SetSize()
        {
            Width = _parent.ElementsWidth;
            Height = _parent.ElementsHeight;
        }

        public override void OnClick()
        {
            base.OnClick();
        }

    }
}
