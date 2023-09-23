using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LostHope.GameCode.GameStates;
using LostHope.GameCode;
using LostHope.Engine.ContentLoading;
using Apos.Input;
using LostHope.GameCode.GameSettings;
using FontStashSharp;
using LostHope.Engine.Localization;

namespace LostHope
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameStateManager _stateManager;
        private UIManager _uiManager;

        #region Properties
        public bool IsPaused;
        public GraphicsDeviceManager GraphicsManager { get { return _graphics; } }
        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }
        public UIManager UIManager { get { return _uiManager; } }
        #endregion

        #region Game States
        public MainMenuState MainMenuState { get; private set; }
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Globals.Settings = Settings.EnsureJson<Settings>("Settings.json", SettingsContext.Default.Settings);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = SpriteBatch;

            InputHelper.Setup(this);
        }
        protected override void UnloadContent()
        {
            if (!Globals.Settings.IsFullscreen)
            {
                SaveWindow();
            }

            Settings.SaveJson<Settings>("Settings.json", Globals.Settings, SettingsContext.Default.Settings);

            base.UnloadContent();
        }

        protected override void Initialize()
        {
            // Load all global things here.
            ContentLoader.Initialize(Content);
            // Add a font
            ContentLoader.AddFont("PeaberryMono.ttf");
            // LDtk File
            ContentLoader.LoadLDtkFile("World");
            // Init the localization system
            LocalizationSystem.Init();

            IsPaused = false;

            // Window properties
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            // Time step
            IsFixedTimeStep = Globals.Settings.IsFixedTimeStep;

            Globals.Settings.IsFullscreen = Globals.Settings.IsFullscreen || Globals.Settings.IsBorderless;
            _graphics.SynchronizeWithVerticalRetrace = Globals.Settings.IsVSync;

            Window.Title = "The Lost Hope";
            RestoreWindow();
            if (Globals.Settings.IsFullscreen)
            {
                ApplyFullscreenChange(false);
            }
            Globals.GraphicsDeviceManager = _graphics;

            // Initialize Game State Manager
            _stateManager = new GameStateManager(this);
            // Initialize UI Manager
            _uiManager = new UIManager(this);

            // Add Components
            Components.Add(_stateManager);

            // Initilize game states here
            MainMenuState = new MainMenuState(this, _stateManager);

            // Set start game state
            _stateManager.SetState(MainMenuState);

            base.Initialize();
        }

        public void ToggleFullscreen()
        {
            bool oldIsFullscreen = Globals.Settings.IsFullscreen;

            if (Globals.Settings.IsBorderless)
            {
                Globals.Settings.IsBorderless = false;
            }
            else
            {
                Globals.Settings.IsFullscreen = !Globals.Settings.IsFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }
        public void ToggleBorderless()
        {
            bool oldIsFullscreen = Globals.Settings.IsFullscreen;

            Globals.Settings.IsBorderless = !Globals.Settings.IsBorderless;
            Globals.Settings.IsFullscreen = Globals.Settings.IsBorderless;

            ApplyFullscreenChange(oldIsFullscreen);
        }
        private void ApplyFullscreenChange(bool oldIsFullscreen)
        {
            if (Globals.Settings.IsFullscreen)
            {
                if (oldIsFullscreen)
                {
                    ApplyHardwareMode();
                }
                else
                {
                    SetFullscreen();
                }
            }
            else
            {
                UnsetFullscreen();
            }
        }
        private void ApplyHardwareMode()
        {
            _graphics.HardwareModeSwitch = !Globals.Settings.IsBorderless;
            _graphics.ApplyChanges();
        }
        private void SetFullscreen()
        {
            SaveWindow();

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.HardwareModeSwitch = !Globals.Settings.IsBorderless;

            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }
        private void UnsetFullscreen()
        {
            _graphics.IsFullScreen = false;
            RestoreWindow();
        }
        private void SaveWindow()
        {
            Globals.Settings.X = Window.ClientBounds.X;
            Globals.Settings.Y = Window.ClientBounds.Y;
            Globals.Settings.Width = Window.ClientBounds.Width;
            Globals.Settings.Height = Window.ClientBounds.Height;
        }
        private void RestoreWindow()
        {
            Window.Position = new Point(Globals.Settings.X, Globals.Settings.Y);
            _graphics.PreferredBackBufferWidth = Globals.Settings.Width;
            _graphics.PreferredBackBufferHeight = Globals.Settings.Height;
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            //Call UpdateSetup at the start.
            InputHelper.UpdateSetup();

            Globals.CurrentScreenWidth = GraphicsDevice.Viewport.Width;
            Globals.CurrentScreenHeight = GraphicsDevice.Viewport.Height;

            GameplayManager.Instance.Update(gameTime);
            _uiManager.Update(gameTime);

            if (IsPaused) return;

            Globals.GameTime = gameTime;
            base.Update(gameTime);

            //Call UpdateCleanup at the end.
            InputHelper.UpdateCleanup();
        }

        protected override void Draw(GameTime gameTime)
        {
            // Choose any other color you want to clear the screen to
            GraphicsDevice.Clear(Color.Black);

            // Draw Game
            base.Draw(gameTime);

            // Draw UI
            _uiManager.Draw(_spriteBatch);
        }
    }
}