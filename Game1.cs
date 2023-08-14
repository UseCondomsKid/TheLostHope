using LostHope.Engine.Camera;
using LostHope.Engine.Input;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LostHope.GameCode.GameStates;
using MonoGame.Aseprite;
using LostHope.Engine.Animations;
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

        private OrthographicCamera _gameCamera;

        // Screen width and height
        private const int _screenWidth = 1280;
        private const int _screenHeight = 720;

        private InputManager _inputManager;


        #region Properties
        public bool IsPaused;
        public GraphicsDeviceManager GraphicsManager { get { return _graphics; } }
        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }
        public UIManager UIManager { get { return _uiManager; } }
        public OrthographicCamera GameCamera { get { return _gameCamera; } set { _gameCamera = value; } }
        #endregion

        #region Game States
        public MainMenuState MainMenuState { get; private set; }
        public GameplayState GameplayState { get; private set; }
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
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.ApplyChanges();
            Globals.GraphicsDeviceManager = _graphics;

            // Initialize Game State Manager
            _stateManager = new GameStateManager(this);
            // Initialize UI Manager
            _uiManager = new UIManager(this);
            // Initialize the Game Camera
            _gameCamera = new OrthographicCamera(this, _screenHeight);
            // Initialize the input manager
            _inputManager = new InputManager(this);

            _gameCamera.Position = new Vector2(0, 0);
            Globals.GameCamera = _gameCamera;

            // Add Components
            Components.Add(_stateManager);
            Components.Add(_uiManager);
            Components.Add(_gameCamera);

            // Initilize game states here
            MainMenuState = new MainMenuState(this, _stateManager, _uiManager);
            GameplayState = new GameplayState(this, _stateManager, _uiManager);
            // TODO: initialize other states here
            // Set an active game state
            _stateManager.SetState(GameplayState);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

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
            GraphicsDevice.Clear(Color.DeepSkyBlue);

            base.Draw(gameTime);
        }
    }
}