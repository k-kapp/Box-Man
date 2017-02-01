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
    class PaintButton : Clickable
    {
        public PaintButton(int x, int y, int width, int height, XNAForm parent) : base(x, y, width, height, ClickType.DRAG, parent)
        {
            BackgroundTexture = _gameMgr.Content.Load<Texture2D>("WhiteBlock");
        }

        public override void Update()
        {
            base.Update();

            if (MouseOnClickable())
            {
                _drawCol = ActiveColor;
            }
            else
            {
                _drawCol = InactiveColor;
            }

        }
    }
}
