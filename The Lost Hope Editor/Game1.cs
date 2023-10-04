using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui;
using TheLostHopeEditor.EditorCode;
using TheLostHopeEditor.EditorCode.StateManagement;
using TheLostHopeEditor.EditorCode.States;
using TheLostHopeEngine.EngineCode.Camera;

namespace TheLostHopeEditor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ImGuiRenderer _imguiRenderer;
        private EditorStateManager _gameStateManager;
        private OrthographicCamera _camera;
        public OrthographicCamera Camera { get { return _camera; } }


        // States
        public EditorBaseState EditorBaseState { get; private set; }
        public GunsEditorState GunsEditorState { get; private set; }

        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Window properties
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            Window.Title = "The Lost Hope Editor";

            // Time step
            IsFixedTimeStep = true;

            // Window Size
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _camera = new OrthographicCamera(this);
            _camera.Initialize();
            _camera.Zoom = 8f;
            Globals.Camera = _camera;

            // Initialize systems
            _imguiRenderer = new ImGuiRenderer(this).Initialize().RebuildFontAtlas();
            _gameStateManager = new EditorStateManager();

            // ---- Initialize States ----
            EditorBaseState = new EditorBaseState(this, _gameStateManager, "Base");
            GunsEditorState = new GunsEditorState(this, _gameStateManager, "Guns Editor");
            // ---------------------------

            // Set starter state
            _gameStateManager.SetState(EditorBaseState);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            Globals.CurrentScreenWidth = GraphicsDevice.Viewport.Width;
            Globals.CurrentScreenHeight = GraphicsDevice.Viewport.Height;

            _camera.Update(gameTime);

            _gameStateManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);

            _gameStateManager.DrawGame(gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);

            _imguiRenderer.BeginLayout(gameTime);
            _gameStateManager.DrawImGui(gameTime);
            _imguiRenderer.EndLayout();
        }
    }
}