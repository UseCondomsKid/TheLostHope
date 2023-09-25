using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode;
using System.Reflection.Emit;
using System.Net.Http.Headers;

namespace LostHope.Engine.Camera
{
    // This class implement a simple orthographic camera.
    public class OrthographicCamera : GameComponent
    {
        #region Variables
        // The 2d transformation matrix
        public Matrix Transform { get; private set; }

        private GraphicsDevice _graphicsDevice;

        // Camera's position
        private Vector2 _position;
        // Camera's zoom
        private float _zoom;
        private int _startingScreenHeight;
        // tmp zoom
        private float _pixelScale = 1f;

        // This lets other scripts access the camera's position considering the position variable above is private
        public Vector2 Position { get { return _position; } }
        // This is the camera width and height
        public Vector2 Size { get { return new Vector2(Globals.CurrentScreenWidth, Globals.CurrentScreenHeight) / _zoom; }}
        // This lets other scripts access the camera's zoom considering the zoom variable above is private
        public float Zoom { get { return _zoom; } set { _zoom = value; } }
        #endregion

        #region Constructor
        // Constructor
        public OrthographicCamera(Game game, int startingScreenHeight) : base(game)
        {
            // Initialize the transformation matrix
            Transform = new();

            _graphicsDevice = game.GraphicsDevice;

            _startingScreenHeight = startingScreenHeight;

            // Initialize the position
            _position = new Vector2(0, 0);
            // Initialize the zoom
            _zoom = 1f;
        }
        #endregion

        #region Methods
        public void SetPosition(Vector2 position)
        {
            _position = position;
        }
        public void SetPosition(float x, float y)
        {
            _position = new Vector2(x, y);
        }
        public override void Initialize()
        {
            base.Initialize();

            // Set the starting camera zoom. Should be 1.
            _pixelScale = Math.Max(_graphicsDevice.Viewport.Height / _startingScreenHeight * _zoom, _zoom);
            _zoom = _pixelScale;

            // Subscribe to the window resize event
            Game.Window.ClientSizeChanged += WindowSizeChanged;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Unsubscribe to the window resize event
            Game.Window.ClientSizeChanged -= WindowSizeChanged;
        }

        // Called when the window resizes
        private void WindowSizeChanged(object sender, EventArgs e)
        {
            // Get new zoom from the new window size
            _pixelScale = Math.Max((_graphicsDevice.Viewport.Height / _startingScreenHeight) * _zoom, _zoom);
            // Apply the new zoom
            _zoom = _pixelScale;
        }

        // Update function
        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            // Each update tick, we update the transformation matrix.
            Transform = Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) * Matrix.CreateScale(_zoom) * Matrix.CreateTranslation(_graphicsDevice.Viewport.Width / 2f, _graphicsDevice.Viewport.Height / 2f, 0);

            base.Update(gameTime);
        }

        // A function to transform a screen position to world position
        public Vector2 ScreenToWorld(Vector2 position)
        {
            Vector3 scale;
            Quaternion rotation;
            Vector3 translation;

            var transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(_zoom) * Matrix.CreateTranslation(_graphicsDevice.Viewport.Width / 2f, _graphicsDevice.Viewport.Height / 2f, 0);
            transform.Decompose(out scale, out rotation, out translation);

            return new Vector2(-translation.X, translation.Y);
        }
        #endregion
    }
}
