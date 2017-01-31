using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
/*
 * TODO: Implement scroller for forms
 */


namespace Sokoban
{

    class ScrollBar : XNAForm
    {

        //GameMgr _gameMgr;
        XNAList _parent;

        Button _upButton, _downButton, _scroller;


        //Rectangle _mainRect = new Rectangle(0, 0, _width, 0);
        //Rectangle _mainOuterRect = new Rectangle(0, 0, _width, 0);

        //Texture2D _upButtonTexture, _downButtonTexture;

        public ScrollBar(int width, XNAList parent) : base(parent.InnerWidth - width, 0, width, parent.InnerHeight, parent, "", false)
        {
            BorderWidth = 1;

            _parent = parent;
            _gameMgr = parent.GameMgr;

            _initialize();
        }

        private Texture2D _createArrowButtonTexture(string filename, Color arrowCol)
        {
            RenderTarget2D temp = _parent.RenderTarget;

            _gameMgr.SpriteBatch.Begin();

            Rectangle drawRect = new Rectangle(0, 0, InnerWidth, InnerWidth);
            RenderTarget2D textureRenderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, InnerWidth, InnerWidth);
            _gameMgr.SetRenderTarget(textureRenderTarget);
            _gameMgr.GraphicsDevice.Clear(Color.White);

            Texture2D backTexture = new Texture2D(_gameMgr.GraphicsDevice, InnerWidth, InnerWidth);

            Color[] colors = new Color[backTexture.Height * backTexture.Width];

            for (int i = 0; i < backTexture.Height; i++)
            {
                for (int j = 0; j < backTexture.Width; j++)
                {
                    colors[i * backTexture.Width + j] = Color.Gray;
                }
            }

            backTexture.SetData(colors);

            Texture2D arrowTexture = _gameMgr.Content.Load<Texture2D>(filename);
            _gameMgr.SpriteBatch.Draw(backTexture, drawRect, Color.White);
            _gameMgr.SpriteBatch.Draw(arrowTexture, drawRect, Color.White);

            _gameMgr.SpriteBatch.End();

            _gameMgr.SetRenderTarget(temp);

            return textureRenderTarget;
        }

        private void _initialize()
        {
            _upButton = new Sokoban.Button("", 0, 0, InnerWidth, InnerWidth, this);
            _upButton.newBackground(_createArrowButtonTexture("UpArrow", Color.Black));
            _upButton.EventCalls += _parent.ScrollUp;
            AddButton(_upButton);

            _downButton = new Sokoban.Button("", 0, InnerHeight - InnerWidth, InnerWidth, InnerWidth, this);
            _downButton.newBackground(_createArrowButtonTexture("DownArrow", Color.Black));
            _downButton.EventCalls += _parent.ScrollDown;
            AddButton(_downButton);

            _scroller = new Button("", 0, InnerWidth + 1, InnerWidth, InnerHeight - 2 - 2 * InnerWidth, this);
            _scroller.EventCalls += _gameMgr.MainMenuCallback;
            AddButton(_scroller);
        }
    }
}
