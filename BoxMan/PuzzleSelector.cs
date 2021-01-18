using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoxMan
{
    class PuzzleSelector : StateBase
    {


        private XNAForm _mainForm;

        PuzzleList _listForm1, _listForm2;
        PuzzleListElement _activeElement = null;
        PuzzleList _activeList = null;

        public PuzzleSelector(GameMgr gameMgr) : base(gameMgr)
        {
            _initialize();

        }

        public void LeftToRight(object sender, ButtonEventArgs args)
        {
            var element = _listForm1.RemoveActiveElement();
            if (element != null)
                _listForm2.AddElement(element);
            _activeElement = null;
            _activeList = null;
        }

        public void RightToLeft(object sender, ButtonEventArgs args)
        {
            var element = _listForm2.RemoveActiveElement();
            if (element != null)
                _listForm1.AddElement(element);
            _activeElement = null;
            _activeList = null;
        }

        protected override void ImportTextures()
        { }

        public void SaveAll(object sender, ButtonEventArgs args)
        {
            _gameMgr.PuzzlePaths = _listForm2.GetActivePuzzleFilepaths();

            FileStream fs = new FileStream(GameMgr.ActivePuzzlesPathsFilepath, FileMode.Create);

            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                bf.Serialize(fs, _gameMgr.PuzzlePaths);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Could not save active puzzle settings file");
                throw;
            }
            finally
            {
                fs.Close();
            }

            _gameMgr.MainMenuCallback(sender, args);
        }

        private void DeleteButtonPressed(object sender, ButtonEventArgs args)
        {
            if (_activeElement == null)
            {
                var popup = PopupDialog.MakePopupDialog("No puzzle selected", "Error", true, this);
                popup.AddButton(Utilities.ClickableDestroyParent, "OK");
                _gameMgr.centerFormX(popup);
                _gameMgr.centerFormY(popup);
                AddForm(popup);
            }
            else
            {
                var popup = PopupDialog.MakePopupDialog("Are you sure that you want to delete the selected puzzle?", "Confirm", true, this);
                popup.AddButton(DeletePuzzle, "Yes");
                popup.AddButton(Utilities.ClickableDestroyParent, "No");
                _gameMgr.centerFormX(popup);
                _gameMgr.centerFormY(popup);
                AddForm(popup);
            }
        }

        private void DeletePuzzle(object sender, ButtonEventArgs args)
        {
            Utilities.ClickableDestroyParent(sender, null);

            string filepath = _activeElement.Filepath;
            _activeList.RemoveActiveElement();

            System.IO.File.Delete(filepath);

            if (_activeList == _listForm2)
            {
                _gameMgr.PuzzlePaths.Remove(filepath);
            }
        }

        private void _initialize()
        {
            int inbetweenSpace = 100;
            int listWidth = 300;
            int listHeight = 500;
            int totalListWidth = listWidth * 2 + inbetweenSpace;


            _mainForm = new BoxMan.XNAForm(10, 10, 750, 700, this, "Select Puzzles", true);

            int startX = (_mainForm.Width - totalListWidth) / 2;

            _listForm1 = new PuzzleList(startX, 50, listWidth, listHeight, "Reserve puzzles", 5, _mainForm);

            List<string> allPuzzles = PuzzleGrid.getPuzzleFilenames("Puzzles");

            foreach(var puzzleFilename in allPuzzles)
            {
                if (!_gameMgr.PuzzlePaths.Contains(puzzleFilename))
                {
                    _listForm1.AddElement(puzzleFilename);
                }
            }

            _listForm2 = new PuzzleList(startX + listWidth + inbetweenSpace, 50, listWidth, listHeight, "Active puzzles", 5, _mainForm);

            foreach (var puzzleFilename in _gameMgr.PuzzlePaths)
            {
                _listForm2.AddElement(puzzleFilename);
            }

            int buttonsWidth = 100;
            int buttonsHeight = 50;
            int buttonsInbetweenSpace = 30;
            int buttonsSpace = 2 * buttonsWidth + buttonsInbetweenSpace;

            int buttonsStartX = (_mainForm.Width - buttonsSpace) / 2;
            int buttonsY = 580;

            Button okButton = new Button("OK", buttonsStartX, buttonsY, buttonsWidth, buttonsHeight, _mainForm);
            okButton.EventCalls += SaveAll;

            Button deleteButton = new Button("Delete", buttonsStartX + buttonsWidth + buttonsInbetweenSpace, buttonsY, buttonsWidth, buttonsHeight, _mainForm);
            deleteButton.EventCalls += DeleteButtonPressed;

            Button rightButton = new BoxMan.Button("", (_mainForm.Width - 60)/2, 100, 60, 60, _mainForm);
            rightButton.newBackground(_gameMgr.Content.Load<Texture2D>("RightArrow"));
            rightButton.EventCalls += LeftToRight;

            Button leftButton = new BoxMan.Button("", (_mainForm.Width - 60)/2, 200, 60, 60, _mainForm);
            leftButton.newBackground(_gameMgr.Content.Load<Texture2D>("LeftArrow"));
            leftButton.EventCalls += RightToLeft;

            /*
            _mainForm.AddForm(_listForm1);
            _mainForm.AddForm(_listForm2);
            _mainForm.AddButton(leftButton);
            _mainForm.AddButton(rightButton);
            _mainForm.AddButton(okButton);
            _mainForm.AddButton(deleteButton);

            AddForm(_mainForm);
            */
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_listForm1.ActiveElement != null)
            {
                if (_activeList == _listForm2)
                {
                    _activeList.DeactivateActiveElement();
                }
                _activeElement = (PuzzleListElement)_listForm1.ActiveElement;
                _activeList = _listForm1;
            }
            if (_listForm2.ActiveElement != null)
            {
                if (_activeList == _listForm1)
                {
                    _activeList.DeactivateActiveElement();
                }
                _activeElement = (PuzzleListElement)_listForm2.ActiveElement;
                _activeList = _listForm2;
            }
        }

    }
}
