using LostHope.Engine.UI;
using LostHope.GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.StateManagement
{
    // This represents a scene
    public abstract class GameState 
    {
        // A list of child components
        // Child components will update and draw automatically.
        protected List<GameComponent> _childComponents;

        // A reference to the state machine
        protected readonly GameStateManager _stateManager;
        // A reference to the UI manager
        protected readonly UIManager _uiManager;
        // A reference to Game1
        protected readonly Game1 _gameRef;

        protected UIAnchor _currentUIAnchor;

        // Public proptery to access the child components
        public List<GameComponent> Components { get { return _childComponents; } }

        // Constructor
        public GameState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager)
        {
            _gameRef = gameRef;
            _stateManager = stateManager;
            _uiManager = uiManager;

            // Initialise list
            _childComponents = new List<GameComponent>();
        }

        // Called when the state is activated
        public virtual void Enter()
        {
            _childComponents = new List<GameComponent>();
            LoadContent();
        }
        public virtual void PostEnter()
        {
            if (_uiManager.UIElements.Count > 0)
            {
                _currentUIAnchor = _uiManager.UIElements[0].Anchor;
            }
        }

        protected virtual void LoadContent()
        {
        }
        protected virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            // Update all child components if they are active
            foreach (var child in _childComponents)
            {
                if (child.Enabled)
                {
                    child.Update(gameTime);
                }
            }
        }

        protected void BeginSpriteBatch(Matrix? transformationMatrix = null)
        {
            // We call the begin with those parameters because the game is pixel art
            _gameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: transformationMatrix);
        }
        protected void EndSpriteBatch()
        {
            _gameRef.SpriteBatch.End();
        }


        protected virtual void DrawGameplay(GameTime gameTime)
        {
            // Draw all child components if they are Drawable and visible
            foreach (var child in _childComponents)
            {
                if (child is DrawableGameComponent component && component.Visible)
                {
                    component.Draw(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            BeginSpriteBatch(Globals.GameCamera.Transform);
            DrawGameplay(gameTime);
            EndSpriteBatch();

            for (int i = 0; i < _uiManager.UIElements.Count; i++)
            {
                BeginSpriteBatch(_uiManager.GetMatrixFromAnchor(_currentUIAnchor));
                // Draw the element
                _uiManager.UIElements[i].Draw(gameTime);
                EndSpriteBatch();

                // Change the anchor based on the next element's anchor
                if (i != _uiManager.UIElements.Count - 1)
                {
                    _currentUIAnchor = _uiManager.UIElements[i + 1].Anchor;
                }
            }
        }

        // Called when the state is exited
        public virtual void Exit()
        {
            UnloadContent();
        }

        // Add and remove components
        public void AddComponent(GameComponent component)
        {
            _childComponents.Add(component);
        }
        public void RemoveComponent(GameComponent component)
        {
            _childComponents?.Remove(component);
        }
    }
}
