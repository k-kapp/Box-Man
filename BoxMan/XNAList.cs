﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoxMan
{
    public class XNAList : XNAForm
    {
        int _numRows, _numCols;

        int _scrollerWidth = 40;

        int _elementsWidth, _elementsHeight;

        ScrollBar _scrollBar;

        List<XNAListElement> _reserveElementsDown;
        List<XNAListElement> _reserveElementsUp;

        protected List<XNAListElement> _elements;
        XNAListElement _activeElement;

        public XNAList(int x, int y, int width, int height, string title, int numRows, FormMgr parent) : base(x, y, width, height, parent, title, true)
        {
            _elements = new List<XNAListElement>();

            _reserveElementsDown = new List<XNAListElement>();
            _reserveElementsUp = new List<XNAListElement>();

            Console.WriteLine("XNAList XAbs: " + XAbs);
            Console.WriteLine("XNAList YAbs" + YAbs);

            _elementsWidth = InnerWidth - _scrollerWidth;
            _elementsHeight = InnerHeight / numRows;

            _numRows = numRows;

            _scrollBar = new BoxMan.ScrollBar(_scrollerWidth, this);
            _scrollBar.BaseColor = Color.LightGray;

            //AddForm(_scrollBar);
        }

        private void _updateElementPosses()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].SetPosition(i);
            }
        }

        public void ElementClicked(object elementObj, ButtonEventArgs args)
        {
            Console.WriteLine("Element clicked");

            var element = elementObj as XNAListElement;
            if (_activeElement != null)
            {
                _activeElement.MakeInactive();

                Console.WriteLine("Element not null");
            }
            _activeElement = element;
            _activeElement.MakeActive();
        }

        private IEnumerable<XNAListElement> GetReserves()
        {
            foreach (var el in _reserveElementsUp)
                yield return el;
            foreach (var el in _reserveElementsDown)
                yield return el;
        }

        private IEnumerable<XNAListElement> GetAll()
        {
            foreach (var el in _elements)
                yield return el;
            foreach (var el in _reserveElementsUp)
                yield return el;
            foreach (var el in _reserveElementsDown)
                yield return el;
        }

        private void _updateElementSizes()
        {
            foreach (var element in GetAll())
            {
                element.SetSize();
            }
        }

        public void ScrollDown(object sender, ButtonEventArgs args)
        {
            if (_reserveElementsDown.Count == 0)
                return;

            _reserveElementsUp.Add(_elements[0]);
            _elements.RemoveAt(0);

            _elements.Add(_reserveElementsDown[0]);
            _reserveElementsDown.RemoveAt(0);

            _updateElementPosses();
        }

        public void ScrollUp(object sender, ButtonEventArgs args)
        {
            Button buttonSender = sender as Button;
            if (_reserveElementsUp.Count == 0)
                return;


            _reserveElementsDown.Insert(0, _elements[_elements.Count - 1]);
            _elements.RemoveAt(_elements.Count - 1);

            _elements.Insert(0, _reserveElementsUp[_reserveElementsUp.Count - 1]);
            _reserveElementsUp.RemoveAt(_reserveElementsUp.Count - 1);

            _updateElementPosses();
        }

        public override void AddClickable(Clickable clickable)
        {
            var element = clickable as XNAListElement;
            if (element != null)
            {
                AddElement(element);
            }
            else
                base.AddClickable(clickable);
        }

        public void AddElement(XNAListElement element)
        {
            element.EventCalls += ElementClicked;
            element.Parent = this;
            element.MakeInactive();
            _elements.Add(element);

            element.Width = _elementsWidth;
            element.Height = _elementsHeight;

            if (_elements.Count > _numRows)
            {
                _reserveElementsDown.Add(_elements[_elements.Count - 1]);
                _elements.RemoveAt(_elements.Count - 1);
            }
            _updateElementPosses();
        }

        public void RemoveElement(XNAListElement element)
        {
            _elements.Remove(element);

            element.EventCalls -= ElementClicked;

            if (_reserveElementsDown.Count > 0)
            {
                _elements.Add(_reserveElementsDown[0]);
                _reserveElementsDown.RemoveAt(0);
            }
            else if (_reserveElementsUp.Count > 0)
            {
                _elements.Add(_reserveElementsUp[_reserveElementsUp.Count - 1]);
                _reserveElementsUp.RemoveAt(_reserveElementsUp.Count - 1);
            }
            _updateElementPosses();
        }

        public void DeactivateActiveElement()
        {
            if (_activeElement != null)
            {
                _activeElement.MakeInactive();
                _activeElement = null;
            }
        }

        public XNAListElement RemoveActiveElement()
        {
            if (_activeElement == null)
                return null;

            RemoveElement(_activeElement);

            XNAListElement returnElement = _activeElement;
            _activeElement.MakeInactive();
            _activeElement = null;

            return returnElement;
        }

        public void AddElement(Texture2D appearance)
        {
            XNAListElement newElement = new BoxMan.XNAListElement(appearance, this);
            AddElement(newElement);
        }

        public override void DrawMisc(GameTime gameTime)
        {
            base.DrawMisc(gameTime);
            foreach(var element in _elements)
            {
                element.Draw();
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach(var element in _elements)
            {
                element.Update();
            }
        }
        
        public void AddElements(int num)
        {
            Texture2D crate = _gameMgr.Content.Load<Texture2D>("Crate");
            for (int i = 0; i < num; i++)
            {
                AddElement(crate);
            }
        }

        public int ScrollerWidth
        {
            get
            {
                return _scrollerWidth;
            }
        }

        public int ElementsWidth
        {
            get
            {
                return _elementsWidth;
            }

            private set
            {
                _elementsWidth = value;

                _updateElementSizes();
            }
        }

        public int ElementsHeight
        {
            get
            {
                return _elementsHeight;
            }

            private set
            {
                _elementsHeight = value;

                _updateElementSizes();
                _updateElementPosses();
            }
        }

        public int ElementsAboveCount
        {
            get
            {
                return _reserveElementsUp.Count;
            }
        }

        public int ElementsBelowCount
        {
            get
            {
                return _reserveElementsDown.Count;
            }
        }

        public int ElementsViewedCount
        {
            get
            {
                return _elements.Count;
            }
        }

        public int AllElementsCount
        {
            get
            {
                return ElementsViewedCount + ElementsBelowCount + ElementsAboveCount;
            }
        }

        public XNAListElement ActiveElement
        {
            get
            {
                return _activeElement;
            }
        }

    }
}
