using LostHope.Engine.Input;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LostHope.GameCode.GameStates;
using LostHope.GameCode;
using LostHope.Engine.ContentLoading;

namespace LostHope
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameStateManager _stateManager;
        private UIManager _uiManager;
        private InputManager _inputManager;


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
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = SpriteBatch;

            // Load all global things here.
            ContentLoader.Initialize(Content);
            // Font
            ContentLoader.LoadSpriteFont("Font");
            // LDtk File
            ContentLoader.LoadLDtkFile("World");

            IsPaused = false;

            // Window properties
            Window.AllowUserResizing = false;
            IsMouseVisible = true;

            // Set the screen width and height, and apply
            _graphics.PreferredBackBufferWidth = Globals.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Globals.ScreenHeight;
            _graphics.ApplyChanges();
            Globals.GraphicsDeviceManager = _graphics;

            // Initialize the input manager
            _inputManager = new InputManager(this);
            // Initialize Game State Manager
            _stateManager = new GameStateManager(this);
            // Initialize UI Manager
            _uiManager = new UIManager(this);

            // Add Components
            Components.Add(_stateManager);
            Components.Add(_uiManager);

            // Initilize game states here
            MainMenuState = new MainMenuState(this, _stateManager, _uiManager);

            // Set start game state
            _stateManager.SetState(MainMenuState);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);
            GameplayManager.Instance.Update(gameTime);

            if (InputManager.KeyPressed(Keys.Escape))
                //&& _stateManager.CurrentGameState.GetType() == typeof(ForestMap))
                // If we are in a gameplay state
            {
                IsPaused = !IsPaused;
            }

            if (IsPaused) return;

            Globals.GameTime = gameTime;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Choose any other color you want to clear the screen to
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}