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
    public class LevelDesigner : StateBase
    {
        PuzzleGrid _grid;
        int _rows, _cols;
        int _saveButtonsSizeY = 50;
        int _saveButtonsSizeX = 100;
        int _saveButtonsSpaceX = 35;

        const int borderWidth = 5;

        Tile _cursorPaint = new Tile(false, Occpr.VOID);

        List<Texture2D> paintTextures;

        XNAForm _designForm;
        XNAForm _designGridForm;
        XNAForm _toolbar;

        Texture2D _cursorTexture;
        Texture2D _erasedTexture;

        int _toolbarButtonSize = 70;
        int _gridTileSize = 50;

        Tile _humanTile;
        PaintButton _humanButton;

        public LevelDesigner(int rows, int cols, GameMgr gameMgr) : base(gameMgr)
        {

            paintTextures = new List<Texture2D>();
            _grid = new Sokoban.PuzzleGrid(rows, cols, _gameMgr);

            _cursorTexture = Utilities.StdCursorTexture;

            _erasedTexture = _gameMgr.Content.Load<Texture2D>("WhiteBlock");

            _rows = rows;
            _cols = cols;

            //forms = new List<XNAForm>();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    _grid[row, col] = new Sokoban.Tile(false, Occpr.VOID);
                }
            }

            _makeDesignForm();
            _makeDesignGrid();
            _makeToolbar();
            _makeButtons();
        }

        private bool _paintSelected()
        {
            return paintTextures.Contains(Utilities.CursorTexture);
        }

        public void CellButtonClicked(object sender, ButtonEventArgs args)
        {
            if (!_paintSelected())
                return;

            PaintButton senderButton = sender as PaintButton;

            int row = senderButton.Y / _gridTileSize;
            int col = senderButton.X / _gridTileSize;

            if (_cursorPaint.State == Occpr.HUMAN)
            {
                if (_humanTile != null)
                {
                    _humanTile.State = Occpr.EMPTY;
                    _humanButton.BackgroundTexture = paintTextures[1];   // make these indices enums, for better readability. index 1 refers to the empty texture
                }
            }

            _grid[row, col] = new Tile(_cursorPaint.Target, _cursorPaint.State);
            if (_cursorPaint.State == Occpr.VOID)
            {
                senderButton.BackgroundTexture = _erasedTexture;
            }
            else
                senderButton.BackgroundTexture = Utilities.CursorTexture;

            if (_cursorPaint.State == Occpr.HUMAN)
            {
                _humanTile = _grid[row, col];
                _humanButton = senderButton;
            }
        }

        public void ToolbarButtonClicked(object sender, ButtonEventArgs args)
        {
            var senderButton = sender as Button;
            _cursorTexture = senderButton.BackgroundTexture;

            int senderXCatg = senderButton.X / _toolbarButtonSize;

            switch (senderXCatg)
            {
                case (0):
                    _cursorPaint = new Tile(false, Occpr.WALL);
                    break;
                case (1):
                    _cursorPaint = new Tile(false, Occpr.EMPTY);
                    break;
                case (2):
                    _cursorPaint = new Tile(false, Occpr.CRATE);
                    break;
                case (3):
                    _cursorPaint = new Tile(true, Occpr.EMPTY);
                    break;
                case (4):
                    _cursorPaint = new Tile(false, Occpr.VOID);
                    break;
                case (5):
                    _cursorPaint = new Tile(false, Occpr.HUMAN);
                    break;
                
            }
        }

        public void SaveButtonClicked(object sender, ButtonEventArgs args)
        {
            List<Tile[,]> allSaved = PuzzleGrid.DeSerializeAll("Puzzles");


            if (!PuzzleGrid.IsValid(_grid.Tiles))
            {
                PopupDialog dialog = PopupDialog.MakePopupDialog("Puzzle invalid. Make sure that \n - It is enclosed by walls, and that all areas are reachable (not blocked by walls) \n - That the amount of targets equal the amount of crates (and that there are targets) \n - That there is a starting point (by using the 'man' button from the toolbar's far right)",
                    "Puzzle invalid", true, this);
                dialog.AddButton(0, 0, 0, 0, Utilities.ClickableDestroyParent, "OK");
                _gameMgr.centerFormX(dialog);
                _gameMgr.centerFormY(dialog);
                AddForm(dialog);
                return;
            }

            foreach (Tile [,] tileArr in allSaved)
            {
                if (PuzzleGrid.TilesEqual(tileArr, _grid.Tiles))
                {
                    PopupDialog dialog = PopupDialog.MakePopupDialog("Cannot save puzzle: Already exists", "Error", true, this);
                    Button.ButtonClickCallback callbackFunc = (senderArg, eventArgs) => _gameMgr.DestroyForm(dialog, senderArg, eventArgs);
                    dialog.AddButton(0, 0, _saveButtonsSizeX, _saveButtonsSizeY, callbackFunc, "OK");
                    _gameMgr.centerFormX(dialog);
                    _gameMgr.centerFormY(dialog);
                    AddForm(dialog);
                    return;
                }
            }

            _grid.Serialize();

            PopupDialog savedDialog = PopupDialog.MakePopupDialog("Puzzle successfully saved", "Success", true, this);
            Button.ButtonClickCallback callback = (a, b) => GameMgr.DestroyForm(savedDialog, a, b);
            savedDialog.AddButton(0, 0, _saveButtonsSizeX, _saveButtonsSizeY, callback, "Continue designing");
            savedDialog.AddButton(0, 0, _saveButtonsSizeX, _saveButtonsSizeY, _gameMgr.MainMenuCallback, "Exit to main menu");
            _gameMgr.centerFormX(savedDialog);
            _gameMgr.centerFormY(savedDialog);
            AddForm(savedDialog);

            var saveButton = sender as Button;
            saveButton.MakeInactive();
        }


        private void _makeDesignForm()
        {
            _designForm = new Sokoban.XNAForm(0, 0, _gameMgr.ScreenWidth, _gameMgr.ScreenHeight, this, "", false);
            AddForm(_designForm);
        }

        private void _makeToolbar()
        {
            List<string> textureList = new List<string>();
            textureList.Add("BrickWall");
            textureList.Add("Empty");
            textureList.Add("Crate");
            textureList.Add("Target");
            textureList.Add("Eraser");
            _toolbar = new XNAForm(0, 0, _toolbarButtonSize * (textureList.Count + 1) + 2*borderWidth, _toolbarButtonSize + 2*borderWidth, _designForm, "m", false);
            _addToolbarButtons(textureList);

            _designForm.AddForm(_toolbar);
        }

        private void _addToolbarButtons(List<string> textures)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                Button newButton = new Button("", i * _toolbarButtonSize, 0, _toolbarButtonSize, _toolbarButtonSize, ToolbarButtonClicked, _toolbar);
                Texture2D addTexture;
                if (textures[i] == "Eraser")
                {
                    addTexture = _gameMgr.Content.Load<Texture2D>("WhiteBlock");
                    RenderTarget2D renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, addTexture.Width, addTexture.Height);
                    _gameMgr.SetRenderTarget(renderTarget);

                    _gameMgr.SpriteBatch.Begin();

                    _gameMgr.DrawSprite(addTexture, new Rectangle(0, 0, addTexture.Width, addTexture.Height), Color.White);
                    _gameMgr.DrawSprite(_gameMgr.Content.Load<Texture2D>("Eraser"), new Rectangle(0, 0, addTexture.Width, addTexture.Height), Color.White);

                    _gameMgr.SpriteBatch.End();

                    _gameMgr.SetRenderTarget(null);

                    addTexture = renderTarget;

                }
                else
                {
                    addTexture = _gameMgr.Content.Load<Texture2D>(textures[i]);
                }
                paintTextures.Add(addTexture);

                newButton.newBackground(paintTextures[paintTextures.Count - 1]);
                newButton.newActiveColor(Color.Gray);
                newButton.newInactiveColor(Color.White);
                _toolbar.AddButton(newButton);
            }

            RenderTarget2D manRenderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, _gridTileSize, _gridTileSize);
            Texture2D manTexture = MainGame.drawManStill(_gameMgr, null, manRenderTarget, false);

            Button newButtonMan = new Button("", textures.Count * _toolbarButtonSize, 0, _toolbarButtonSize, _toolbarButtonSize, ToolbarButtonClicked, _toolbar);
            newButtonMan.newBackground(manTexture);
            newButtonMan.newActiveColor(Color.Gray);
            newButtonMan.newInactiveColor(Color.White);
            _toolbar.AddButton(newButtonMan);

            paintTextures.Add(manTexture);
        }

        private void _makeDesignGrid()
        {
            _designGridForm = new Sokoban.XNAForm(100, 100, _cols * _gridTileSize + 2*borderWidth, _rows * _gridTileSize + 2*borderWidth, _designForm, "", false);
            _designGridForm.BorderWidth = borderWidth;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    PaintButton button = new Sokoban.PaintButton(col * _gridTileSize, row * _gridTileSize, _gridTileSize, _gridTileSize, _designGridForm);
                    button.ActiveColor = Color.Gray;
                    button.InactiveColor = Color.White;
                    button.EventCalls += CellButtonClicked;

                    _designGridForm.AddClickable(button);
                }
            }

            _designForm.AddForm(_designGridForm);
        }

        private void _makeButtons()
        {
            int bottomY = _designGridForm.Y + _designGridForm.Height;

            int buttonsY = (_designForm.Height - bottomY - _saveButtonsSizeY) / 2 + bottomY;

            int buttonsStartX = (_designGridForm.Width - _saveButtonsSpaceX - 2 * _saveButtonsSizeX)/2 + _designGridForm.X;

            _designForm.AddButton(buttonsStartX, buttonsY, _saveButtonsSizeX, _saveButtonsSizeY, SaveButtonClicked, "Save");
            _designForm.AddButton(buttonsStartX + _saveButtonsSizeX + _saveButtonsSpaceX, buttonsY, _saveButtonsSizeX, _saveButtonsSizeY,
                                    _gameMgr.MainMenuCallback, "Cancel");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (forms.Count == 1 && _update && popups.Count == 0 && _designGridForm.MouseOnThis())
            {
                Utilities.CursorTexture = _cursorTexture;
            }
            else
            {
                Utilities.CursorTexture = Utilities.StdCursorTexture;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void ImportTextures()
        {
        }
    }
}
