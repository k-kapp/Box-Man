using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
//using System.Windows.Forms; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sokoban
{

    public static class Utilities
    {
        public static Texture2D StdCursorTexture;
        public static Texture2D CursorTexture;
        private static int _keyHeldPrevSize = 128;

        public static bool[] KeyHeldPrev = new bool[_keyHeldPrevSize];


        public static void DrawCursor(GameMgr gameMgr)
        {
            MouseState mstate = Mouse.GetState();
            gameMgr.DrawSprite(CursorTexture, new Rectangle(mstate.Position, new Point(20, 20)), Color.White);
        }

        public static bool MouseOnRect(Rectangle rect, int innerXAbs, int innerYAbs)
        {
            MouseState mstate = Mouse.GetState();

            Point mPos = mstate.Position;

            mPos.X = mPos.X - innerXAbs;
            mPos.Y = mPos.Y - innerYAbs;

            if ((mPos.X > rect.X) && (mPos.X < rect.X + rect.Width)
                    && (mPos.Y > rect.Y) && (mPos.Y < rect.Y + rect.Height))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void ClickableDestroyParent(object sender, ButtonEventArgs arg)
        {
            var clickable = sender as Clickable;

            clickable.Parent.Destroy();
        }

        public static bool CoordValid(int row, int col, int numRows, int numCols)
        {
            return !(row < 0 || col < 0 || row >= numRows || col >= numCols);
        }

        public static Texture2D MakeTexture(Color color, GraphicsDevice device)
        {
            Color[] data = new Color[1];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }

            Texture2D returnTexture = new Texture2D(device, 1, 1);
            returnTexture.SetData(data);
            return returnTexture;
        }

        public static List<char> GetKeyboardInput()
        {
            KeyboardState keyState = Keyboard.GetState();
            Keys[] keysDown = keyState.GetPressedKeys();

            for (int i = 0; i < _keyHeldPrevSize; i++)
            {
                if (!keysDown.Contains((Keys)i))
                {
                    KeyHeldPrev[i] = false;
                }
            }

            List<char> chars = new List<char>();

            foreach(var keyDown in keysDown)
            {
                int value = (int)keyDown;

                if (value >= KeyHeldPrev.Length || Utilities.KeyHeldPrev[value])
                    continue;
                else
                    KeyHeldPrev[value] = true;

                if (((value >= (int)Keys.D0) && (value <= (int)Keys.D9))
                    || ((value >= (int)Keys.A) && (value <= (int)Keys.Z))
                    || ((value >= (int)Keys.NumPad0) && (value <= (int)Keys.NumPad9)))
                {
                    if (value >= (int)Keys.NumPad0)
                        value -= ((int)Keys.NumPad0 - (int)Keys.D0);
                    chars.Add((char)value);
                }
                else if (value == (int)Keys.Back)
                {
                    if (chars.Count > 0)
                    {
                        chars.RemoveAt(chars.Count - 1);
                    }
                    else
                    {
                        chars.Add((char)value);
                    }
                }
            }

            return chars;
        }
    }

    public class GameMgr : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool _changeState;

        StateBase _currState;
        Texture2D _whiteTexture;

        MainMenu _menu;
        MainGame _game;
        StateBase _savedState;

        int _screenHeight, _screenWidth;

        XNAForm _form;

        public List<string> PuzzlePaths;

        public const string ActivePuzzlesPathsFilepath = "ActivePuzzles.dat";

        public GameMgr(int screenWidth, int screenHeight)
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            //PuzzlePaths = new List<string>();
        }

        public int ScreenWidth
        {
            set
            {
                _screenWidth = value;
                graphics.PreferredBackBufferWidth = value;
            }

            get
            {
                return _screenWidth;
            }
        }

        public int ScreenHeight
        {
            set
            {
                _screenHeight = value;
                graphics.PreferredBackBufferHeight = value;

            }

            get
            {
                return _screenHeight;
            }
        }

        public void ChangeState()
        {
        }

        public void StateGame()
        {
            _currState = new MainGame(this);
        }

        public void StateMainMenu()
        {
            _currState = _makeMainMenu();
        }

        public void StateMainMenuResume()
        {
            _currState = _makeMainMenu();
        }

        public void StateExit()
        {
            Exit();
        }

        public void DoNothing(object caller, ButtonEventArgs args)
        {

        }

        public void GotoDesigner(object caller, ButtonEventArgs args)
        {
            LevelDesigner designer = new LevelDesigner(8, 9, this);
            _currState = designer;
        }

        public void GotoPuzzleSelector(object caller, ButtonEventArgs args)
        {
            PuzzleSelector selector = new PuzzleSelector(this);
            _currState = selector;
        }

        public void SaveGameState(object caller, ButtonEventArgs args)
        {
            _savedState = _currState;
        }

        public void MainMenuCallback(object caller, ButtonEventArgs args)
        {
            _currState = _makeMainMenu();
        }

        public void GotoButtonTest(object caller, ButtonEventArgs args)
        {
            _currState = new ButtonTest(this);
        }

        public void DestroyForm(XNAForm form, object sender, ButtonEventArgs args)
        {
            form.Destroy();
        }

        public void ResumeGameCallback(object caller, ButtonEventArgs args)
        {
            MainGame tempSaved;
            tempSaved = _savedState as MainGame;
            if (_savedState != null)
                _currState = _savedState;
            else
                _currState = new MainGame(this);
        }

        public void NewGameCallback(object caller, ButtonEventArgs args)
        {
            if (PuzzlePaths.Count == 0)
            {
                PopupDialog selectPuzzlePopup = PopupDialog.MakePopupDialog("No active puzzles. Please select the 'Select Puzzles' option in the main menu", 
                    "No active puzzles", false, _currState);
                Button.ButtonClickCallback callback = (a, b) => { var button = a as Button; button.Parent.Destroy(); };
                selectPuzzlePopup.AddButton(callback, "OK");
                centerFormX(selectPuzzlePopup);
                centerFormY(selectPuzzlePopup);
                AddPopup(selectPuzzlePopup);
            }
            else
                _currState = new MainGame(this);
        }

        public void ExitCallback(object caller, ButtonEventArgs args)
        {
            Exit();
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }

        public void centerFormX(XNAForm form)
        {
            form.X = (ScreenWidth - form.Width) / 2;
        }

        public void centerFormY(XNAForm form)
        {
            form.Y = (ScreenHeight - form.Height) / 2;
        }

        public void centerMenuXY(Menu menu)
        {
            centerFormX(menu);
            centerFormY(menu);
        }


        private MainMenu _makeMainMenu()
        {
            int menuHeight = 500;
            int menuWidth = 300;

            MainGame temp = _currState as MainGame;
            bool resume = temp != null;
            _menu = new MainMenu(10, 10, menuWidth, menuHeight, "Sokoban", resume, this);

            return _menu;
        }

        private void _setActivePuzzlePaths()
        {
            FileStream fs;
            try
            {
                fs = new FileStream(ActivePuzzlesPathsFilepath, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                PuzzlePaths = new List<string>();
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();

            PuzzlePaths = (List<string>)bf.Deserialize(fs);

            fs.Close();

        }

        private void _checkPuzzlesExist()
        {
            FileStream fs = null;

            List<string> removePaths = new List<string>();

            foreach (var path in PuzzlePaths)
            {
                try
                {
                    fs = new FileStream(path, FileMode.Open);
                }
                catch (FileNotFoundException e)
                {
                    removePaths.Add(path);
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }

            foreach (var path in removePaths)
            {
                PuzzlePaths.Remove(path);
            }

            if (removePaths.Count > 0)
            {
                fs = new FileStream(ActivePuzzlesPathsFilepath, FileMode.Create);

                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(fs, PuzzlePaths);

                fs.Close();
            }
        }

        public void SetRenderTarget(RenderTarget2D target)
        {
            GraphicsDevice.SetRenderTarget(target);
        }

        protected override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            int menuHeight = 800;
            int menuWidth = 400;

            Utilities.StdCursorTexture = Content.Load<Texture2D>("StandardCursor");

            _setActivePuzzlePaths();

            _currState = _makeMainMenu();

            _whiteTexture = Content.Load<Texture2D>("WhiteBlock");


            //so that background images do not get wiped when we overlay textures
            GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            Utilities.CursorTexture = Content.Load<Texture2D>("StandardCursor");
        }


        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _currState.Update(gameTime);

        }

        public void AddPopup(PopupDialog popup)
        {
            _currState.AddForm(popup);
        }

        public void RemovePopup(PopupDialog popup)
        {
            _currState.RemoveForm(popup);
        }

        public void GotoGame()
        {
            _currState = new MainGame(this);
        }

        public void DrawSprite(Texture2D texture, Rectangle destRect, Color color)
        {
            spriteBatch.Draw(texture, destRect, color);
        }

        public void DrawString(SpriteFont font, string text, Vector2 stringPos, Color stringCol, float scale)
        {
            spriteBatch.DrawString(font, text, stringPos, stringCol, 0, new Vector2(0, 0), scale, 0, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            int screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            Rectangle backRect = new Rectangle(0, 0, screenWidth, screenHeight);


            spriteBatch.Begin();
            spriteBatch.Draw(_whiteTexture, backRect, Color.Black);

            _currState.Draw(gameTime);


            spriteBatch.End();
        }

        public bool ShowCursor
        {
            set
            {
                _currState.ShowCursor = value;
            }

            get
            {
                return _currState.ShowCursor;
            }
        }

    }
}
