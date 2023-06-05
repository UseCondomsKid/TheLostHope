using LostHope.Engine.Input;
using LostHope.Engine.Rendering;
using LostHope.Engine.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.UI
{
    // UIAnchors anchor UI elements to their respective side of the screen.
    public enum UIAnchor { Center, Left, Top, Right, Bottom }

    // A class that manages UI in the game.
    public class UIManager : GameComponent
    {
        // Selectables
        private List<Selectable> _selectables;
        private Selectable _selectedSelectable;

        // Matrix for each anchor, this is used when rendering, we set the transformation matrix to be any one of these.
        private Matrix _centerAnchorMatrix;
        private Matrix _leftAnchorMatrix;
        private Matrix _rightAnchorMatrix;
        private Matrix _topAnchorMatrix;
        private Matrix _bottomAnchorMatrix;

        // A reference to Game1
        private Game1 _gameRef;

        // Canvas size, used to scale the UI
        private float _pixelScale;
        private float _canvasScale = 1f;

        // For transforming coordinates from screen to canvas
        private Vector3 _scale;
        private Quaternion _rotation;
        private Vector3 _translation;


        // Public getters for each matrix
        public Matrix CenterAnchorMatrix { get { return _centerAnchorMatrix; } }
        public Matrix LeftAnchorMatrix { get { return _leftAnchorMatrix; } }
        public Matrix RightAnchorMatrix { get { return _rightAnchorMatrix; } }
        public Matrix TopAnchorMatrix { get { return _topAnchorMatrix; } }
        public Matrix BottomAnchorMatrix { get { return _bottomAnchorMatrix; } }
        // public getter for the Canvas Scale
        public float CanvasZoom { get { return _canvasScale; } }

        // Constructor
        public UIManager(Game1 game) : base(game)
        {
            // Initilization
            _gameRef = game;
            _selectables = new List<Selectable>();

            // Before we load any new state, we reset our selectables list. This needs to happen
            // because every new state is a new scene.
            GameStateManager.OnStatePreChanged += () => { _selectables = new List<Selectable>(); };

            // When the window size changes, we update the canvas scale to suit the new size.
            game.Window.ClientSizeChanged += WindowSizeChanged;
            _pixelScale = Math.Max(game.GraphicsDevice.Viewport.Height / Globals.GraphicsDeviceManager.PreferredBackBufferHeight, 1);
            _canvasScale = _pixelScale;
        }

        private void WindowSizeChanged(object sender, EventArgs e)
        {
            _pixelScale = Math.Max((Game.GraphicsDevice.Viewport.Height / Globals.GraphicsDeviceManager.PreferredBackBufferHeight) * _canvasScale, _canvasScale);
            _canvasScale = _pixelScale;
        }

        public void RegisterSelectable(Selectable selectable, bool setAsSelected)
        {
            if (_selectables.Contains(selectable)) return;

            if (setAsSelected)
            {
                if (_selectedSelectable != null) _selectedSelectable.InvokeOnDeselect();

                _selectedSelectable = selectable;
                _selectedSelectable.InvokeOnSelect();
                _selectedSelectable.InvokeOnEnter();
            }

            _selectables.Add(selectable);
        }
        public void SetSelected(Selectable selectable)
        {
            if (!_selectables.Contains(selectable)) return;

            if (_selectedSelectable != null) _selectedSelectable.InvokeOnDeselect();

            _selectedSelectable = selectable;
            _selectedSelectable.InvokeOnSelect();
            _selectedSelectable.InvokeOnEnter();
        }
        public bool IsSelected(Selectable selectable)
        {
            return _selectedSelectable == selectable;
        }

        public override void Update(GameTime gameTime)
        {
            // We update the matrices
            _centerAnchorMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f, 0);
            _leftAnchorMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(0 , Game.GraphicsDevice.Viewport.Height / 2f, 0);
            _rightAnchorMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height / 2f, 0);
            _topAnchorMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width / 2f, 0, 0);
            _bottomAnchorMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height, 0);

            base.Update(gameTime);
        }

        // Function that converts Screen positions to UICanvas positions, depending on the anchor.
        public Vector2 UIScreenToCanvas(Vector2 position, UIAnchor anchor)
        {
            Vector2 newPosition = new Vector2(0, 0);
            Matrix transform = new Matrix();

            switch (anchor)
            {
                case UIAnchor.Center:
                    transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f, 0);
                    transform.Decompose(out _scale, out _rotation, out _translation);
                    newPosition = new Vector2(-_translation.X, _translation.Y);
                    break;
                case UIAnchor.Left:
                    transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(0 , Game.GraphicsDevice.Viewport.Height / 2f, 0);
                    transform.Decompose(out _scale, out _rotation, out _translation);
                    newPosition = new Vector2(-_translation.X, _translation.Y);
                    break;
                case UIAnchor.Right:
                    transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height / 2f, 0);
                    transform.Decompose(out _scale, out _rotation, out _translation);
                    newPosition = new Vector2(-_translation.X, _translation.Y);
                    break;
                case UIAnchor.Top:
                    transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width / 2f, 0, 0);
                    transform.Decompose(out _scale, out _rotation, out _translation);
                    newPosition = new Vector2(-_translation.X, _translation.Y);
                    break;
                case UIAnchor.Bottom:
                    transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateScale(_canvasScale) * Matrix.CreateTranslation(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height, 0);
                    transform.Decompose(out _scale, out _rotation, out _translation);
                    newPosition = new Vector2(-_translation.X, _translation.Y);
                    break;
            }

            return newPosition;
        }
    }
}
