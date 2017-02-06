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
        PaintButton[,] _paintButtonGrid;
        PuzzleGrid _grid;
        int _rows, _cols;
        int _saveButtonsSizeY = 50;
        int _saveButtonsSizeX = 100;
        int _saveButtonsSpaceX = 35;

        const int _maxRows = 9;
        const int _maxCols = 17;

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

            _paintButtonGrid = new PaintButton[rows, cols];

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

            int senderXCatg = senderButton.X / _toolbarButtonSize;

            if (senderXCatg <= 5)
                _cursorTexture = senderButton.BackgroundTexture;

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
                case (6):
                    break;
            }
        }

        private void _makeGridResizePopup(object sender, ButtonEventArgs args)
        {
            Console.WriteLine("Adding new form");

            XNAForm popupInput = new XNAForm(0, 0, 440, 380, this, "Resize grid", true);

            popupInput.AddLabel(new Sokoban.XNALabel("Number of columns (maximum " + _maxCols + "): ", _gameMgr.Content.Load<SpriteFont>("Courier New"), new Vector2(50, 15), 1.5f, popupInput));
            var columnsTextField = new Sokoban.TextField(50, 50, 200, 30, popupInput);
            columnsTextField.Text = _cols.ToString();
            popupInput.AddLabel(new Sokoban.XNALabel("Number of rows (maximum " + _maxRows + "): ", _gameMgr.Content.Load<SpriteFont>("Courier New"), new Vector2(50, 95), 1.5f, popupInput));
            var rowsTextField = new Sokoban.TextField(50, 150, 200, 30, popupInput);
            rowsTextField.Text = _rows.ToString();

            ButtonEventArgs eventArgs = new Sokoban.ButtonEventArgs();
            eventArgs.AddClickable(columnsTextField);
            eventArgs.AddClickable(rowsTextField);

            var popupButton = new Button("OK", 100, 220, 100, 50, popupInput);
            popupButton.ButtonEventArgs = eventArgs;
            popupButton.EventCalls += _checkGridResizeInput;

            var popupCancelButton = new Button("Cancel", 230, 220, 100, 50, popupInput);
            popupCancelButton.EventCalls += Utilities.ClickableDestroyParent;

            _gameMgr.centerFormX(popupInput);
            _gameMgr.centerFormY(popupInput);
        }

        private void _checkGridResizeInput(object sender, ButtonEventArgs args)
        {
            bool done = true;

            var button = sender as Button;

            XNAForm inputPopup = button.Parent;

            var columnsTextField = args.Clickables[0] as TextField;
            var rowsTextField = args.Clickables[1] as TextField;

            int result = 0;
            if (Int32.TryParse(columnsTextField.Text, out result))
            {
                if (result > _maxCols)
                {
                    var errorPopup = PopupDialog.MakePopupDialog("Maximum number of columns exceeded", "Error", true, this);
                    errorPopup.AddButton(Utilities.ClickableDestroyParent, "OK");
                    done = false;

                    _gameMgr.centerFormX(errorPopup);
                    _gameMgr.centerFormY(errorPopup);
                }
                else 
                    _cols = result;
            }
            else
            {
                var errorPopup = PopupDialog.MakePopupDialog("Invalid input: must be integers only", "Error", true, this);
                errorPopup.AddButton(Utilities.ClickableDestroyParent, "OK");
                done = false;
            }
            if (Int32.TryParse(rowsTextField.Text, out result))
            {
                if (result > _maxRows)
                {
                    var errorPopup = PopupDialog.MakePopupDialog("Maximum number of rows exceeded", "Error", true, this);
                    errorPopup.AddButton(Utilities.ClickableDestroyParent, "OK");
                    done = false;

                    _gameMgr.centerFormX(errorPopup);
                    _gameMgr.centerFormY(errorPopup);
                }
                else
                    _rows = result;
            }
            else if (done)
            {
                var errorPopup = PopupDialog.MakePopupDialog("Invalid input: must be integers only", "Error", true, this);
                errorPopup.AddButton(Utilities.ClickableDestroyParent, "OK");
                done = false;
            }
            if (done)
            {
                if ((_rows != _grid.NumRows()) || (_cols != _grid.NumCols()))
                    _updateGridSize();
                button.Parent.Destroy();
            }
        }

        public void SaveButtonClicked(object sender, ButtonEventArgs args)
        {
            List<Tile[,]> allSaved = PuzzleGrid.DeSerializeAll("Puzzles");

            var saveButton = sender as Button;
            saveButton.MakeInactive();

            if (!PuzzleGrid.IsValid(_grid.Tiles))
            {
                PopupDialog dialog = PopupDialog.MakePopupDialog("Puzzle invalid. Make sure that \n - It is enclosed by walls, and that all areas are reachable (not blocked by walls) \n - That the amount of targets equal the amount of crates (and that there are targets) \n - That there is a starting point (by using the 'man' button, second from the right in the toolbar)",
                    "Puzzle invalid", true, this);
                dialog.AddButton(Utilities.ClickableDestroyParent, "OK");
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
                    dialog.AddButton(callbackFunc, "OK");
                    _gameMgr.centerFormX(dialog);
                    _gameMgr.centerFormY(dialog);
                    AddForm(dialog);
                    return;
                }
            }

            _grid.Serialize();

            PopupDialog savedDialog = PopupDialog.MakePopupDialog("Puzzle successfully saved", "Success", true, this);
            Button.ButtonClickCallback callback = (a, b) => GameMgr.DestroyForm(savedDialog, a, b);
            savedDialog.AddButton(callback, "Continue designing");
            savedDialog.AddButton(_gameMgr.MainMenuCallback, "Exit to main menu");
            _gameMgr.centerFormX(savedDialog);
            _gameMgr.centerFormY(savedDialog);
            AddForm(savedDialog);

        }

        private void _updateGridSize()
        {
            Tile[,] newTileSet = new Tile[_rows, _cols];

            for (int currRow = 0; currRow < _rows; currRow++)
            {
                for (int currCol = 0; currCol < _cols; currCol++)
                {
                    if (currRow < _grid.NumRows() && currCol < _grid.NumCols())
                        newTileSet[currRow, currCol] = _grid[currRow, currCol];
                    else
                        newTileSet[currRow, currCol] = new Tile(false, Occpr.VOID);
                }
            }
            
            _makeDesignGrid();
            _grid.Tiles = newTileSet;
        }

        private void _makeDesignForm()
        {
            _designForm = new Sokoban.XNAForm(0, 0, _gameMgr.ScreenWidth, _gameMgr.ScreenHeight, this, "", false);
            _designForm.BaseColor = Color.Black;
        }

        private void _makeToolbar()
        {
            List<string> textureList = new List<string>();
            textureList.Add("BrickWall");
            textureList.Add("Empty");
            textureList.Add("Crate");
            textureList.Add("Target");
            textureList.Add("Eraser");

            _toolbar = new XNAForm(0, 0, _toolbarButtonSize * (textureList.Count + 2) + 2*borderWidth, _toolbarButtonSize + 2*borderWidth, _designForm, "m", false);
            _addToolbarButtons(textureList);
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
                    RenderTarget2D renderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, addTexture.Width, addTexture.Height, false, _gameMgr.GraphicsDevice.PresentationParameters.BackBufferFormat, 
                                                _gameMgr.GraphicsDevice.PresentationParameters.DepthStencilFormat, _gameMgr.GraphicsDevice.PresentationParameters.MultiSampleCount,  RenderTargetUsage.PreserveContents);
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
                //_toolbar.AddButton(newButton);
            }

            RenderTarget2D manRenderTarget = new RenderTarget2D(_gameMgr.GraphicsDevice, _gridTileSize, _gridTileSize);
            Texture2D manTexture = MainGame.drawManStill(_gameMgr, null, manRenderTarget, false);

            Button newButtonMan = new Button("", textures.Count * _toolbarButtonSize, 0, _toolbarButtonSize, _toolbarButtonSize, ToolbarButtonClicked, _toolbar);
            newButtonMan.newBackground(manTexture);
            newButtonMan.newActiveColor(Color.Gray);
            newButtonMan.newInactiveColor(Color.White);
            //_toolbar.AddButton(newButtonMan);


            Texture2D resizeTexture = _gameMgr.Content.Load<Texture2D>("GridResize");

            Button resizeButton = new Button("", (textures.Count + 1) * _toolbarButtonSize, 0, _toolbarButtonSize, _toolbarButtonSize, ToolbarButtonClicked, _toolbar);
            resizeButton.newBackground(_makeResizeTexture());
            resizeButton.newActiveColor(Color.Gray);
            resizeButton.newActiveColor(Color.White);
            //_toolbar.AddButton(resizeButton);
            resizeButton.EventCalls += _makeGridResizePopup;

            paintTextures.Add(manTexture);
        }

        private Texture2D _makeResizeTexture()
        {
            Texture2D resizeTexture = _gameMgr.Content.Load<Texture2D>("GridResize");
            RenderTarget2D resizeTextureBackground = new RenderTarget2D(_gameMgr.GraphicsDevice, _toolbarButtonSize, _toolbarButtonSize);

            _gameMgr.SetRenderTarget(resizeTextureBackground);
            _gameMgr.SpriteBatch.Begin();

            _gameMgr.DrawSprite(Utilities.MakeTexture(Color.White, _gameMgr.GraphicsDevice), 
                                 new Rectangle(0, 0, _toolbarButtonSize, _toolbarButtonSize), Color.White);
            _gameMgr.DrawSprite(resizeTexture, new Rectangle(0, 0, _toolbarButtonSize, _toolbarButtonSize), Color.White);

            _gameMgr.SpriteBatch.End();
            _gameMgr.SetRenderTarget(null);

            return resizeTextureBackground;
        }

        private void _makeDesignGrid()
        {
            PaintButton[,] tempPaintButtonGrid = _paintButtonGrid;
            _paintButtonGrid = new PaintButton[_rows, _cols];
            XNAForm tempDesignGrid = _designGridForm;
            _designGridForm = new Sokoban.XNAForm(100, 100, _cols * _gridTileSize + 2*borderWidth, _rows * _gridTileSize + 2*borderWidth, _designForm, "", false);
            _designGridForm.BorderWidth = borderWidth;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    PaintButton button;
                    if ((tempDesignGrid != null) && (row < _grid.NumRows() && col < _grid.NumCols() && _grid.Tiles[row, col].State != Occpr.VOID)
                        && (row < tempPaintButtonGrid.GetLength(0) && col < tempPaintButtonGrid.GetLength(1)))
                    {
                        button = new PaintButton(tempPaintButtonGrid[row, col], _designGridForm);
                    }
                    else
                        button = new Sokoban.PaintButton(col * _gridTileSize, row * _gridTileSize, _gridTileSize, _gridTileSize, _designGridForm);
                    button.ActiveColor = Color.Gray;
                    button.InactiveColor = Color.White;
                    button.EventCalls += CellButtonClicked;

                    _paintButtonGrid[row, col] = button;
                    //_designGridForm.AddClickable(button);
                }
            }

            if (tempDesignGrid != null)
            {
                _designForm.RemoveForm(tempDesignGrid);
            }
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
