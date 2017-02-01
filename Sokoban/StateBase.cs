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
    abstract public class StateBase : FormMgr
    {

        SpriteFont _font;

        abstract protected void ImportTextures();

        protected List<PopupDialog> popups;
        protected List<PopupDialog> popupsAdd;
        protected List<PopupDialog> popupsRemove;

        protected bool _showCursor;

        public StateBase(GameMgr gameMgr) : base(gameMgr)
        {
            renderTarget = null;
            _font = _gameMgr.Content.Load<SpriteFont>("Courier New");

            _showCursor = true;

            popups = new List<PopupDialog>();
            popupsAdd = new List<PopupDialog>();
            popupsRemove = new List<PopupDialog>();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (var popup in popups)
                popup.Draw(gameTime);

            if (_showCursor)
                Utilities.DrawCursor(_gameMgr);

            MouseState mstate = Mouse.GetState();

            _gameMgr.DrawString(_font, mstate.Position.ToString(), new Vector2(_gameMgr.ScreenWidth - 100, _gameMgr.ScreenHeight - 100), Color.White, 1.0f);
        }
        
        protected void _exit()
        {
            _gameMgr.ChangeState();
        }

        public override void AddForm(XNAForm form)
        {
            var popupForm = form as PopupDialog;
            if (popupForm == null)
                base.AddForm(form);
            else
                popupsAdd.Add(popupForm);
        }

        public override void RemoveForm(XNAForm form)
        {
            var popupForm = form as PopupDialog;

            if (popupForm == null)
            {
                base.RemoveForm(form);
            }
            else
            {
                popupsRemove.Add(popupForm);
            }
        }

        public override void RemoveAllForms()
        {
            base.RemoveAllForms();
            foreach (var popup in popups)
            {
                RemoveForm(popup);
            }
        }

        public override void AddForms()
        {
            base.AddForms();
            foreach (var popup in popupsAdd)
                popups.Add(popup);
            popupsAdd.Clear();
        }

        public override void RemoveForms()
        {
            base.RemoveForms();
            foreach (var popup in popupsRemove)
            {
                Console.WriteLine("Removing popup");
                popups.Remove(popup);
            }
            popupsRemove.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            if (popups.Count > 0)
            {
                _update = false;
                popups[popups.Count - 1].Update(gameTime);
            }
            else
                _update = true;
            UpdateMisc(gameTime);
            base.Update(gameTime);
        }

        public GameMgr GameMgr
        {
            get
            {
                return _gameMgr;
            }
        }

        public bool ShowCursor
        {
            set
            {
                _showCursor = value;
            }

            get
            {
                return _showCursor;
            }
        }

    }
}
