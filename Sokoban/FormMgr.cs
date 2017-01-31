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
    public abstract class FormMgr
    {
        protected List<XNAForm> forms;
        protected List<XNAForm> formsRemove;
        protected List<XNAForm> formsAdd;

        protected GameMgr _gameMgr;

        protected RenderTarget2D renderTarget;

        protected bool _update;

        public FormMgr(GameMgr gameMgr)
        {
            renderTarget = null;

            _gameMgr = gameMgr;

            _update = true;

            forms = new List<XNAForm>();
            formsRemove = new List<XNAForm>();
            formsAdd = new List<XNAForm>();
        }

        public virtual void DrawMisc(GameTime gameTime)
        { }

        public virtual void UpdateMisc(GameTime gameTime)
        { }

        public virtual void Draw(GameTime gameTime)
        {
            /*
            int i = forms.Count - 1;

            for (; i >= 0; i--)
            {
                forms[i].Draw(gameTime);
            }
            */

            foreach(XNAForm form in forms)
            {
                form.Draw(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            int count = 0;

            if (_update)
                foreach (XNAForm form in forms)
                {
                    form.Update(gameTime);
                    count++;
                }
            RemoveForms();
            AddForms();
        }

        public virtual void RemoveForms()
        {
            foreach (XNAForm form in formsRemove)
            {
                forms.Remove(form);
            }
            formsRemove.Clear();
        }

        public virtual void AddForms()
        {
            foreach (XNAForm form in formsAdd)
            {
                forms.Add(form);
            }
            formsAdd.Clear();
        }

        public virtual void RemoveForm(XNAForm form)
        {
            formsRemove.Add(form);
        }

        public virtual void AddForm(XNAForm form)
        {
            formsAdd.Add(form);
        }

        public GameMgr GameMgr
        {
            get
            {
                return _gameMgr;
            }
        }

        public virtual RenderTarget2D RenderTarget
        {
            get
            {
                return renderTarget;
            }
        }
    }
}
