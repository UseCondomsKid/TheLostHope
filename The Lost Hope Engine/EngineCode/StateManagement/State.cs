using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TheLostHopeEngine.EngineCode.UI;

namespace TheLostHopeEngine.EngineCode.StateManagement
{
    // This represents a scene
    public abstract class State 
    {
        // A list of child components
        // Child components will update and draw automatically.
        protected List<GameComponent> _childComponents;

        // A reference to the state machine
        protected readonly StateManager _stateManager;
        // A reference to Game1
        protected readonly Game _gameRef;
        protected readonly SpriteBatch _spriteBatch;

        protected UIAnchor _currentUIAnchor;

        // Public proptery to access the child components
        public List<GameComponent> Components { get { return _childComponents; } }

        // Constructor
        public State(Game gameRef, SpriteBatch spriteBatch, StateManager stateManager)
        {
            _gameRef = gameRef;
            _spriteBatch = spriteBatch;
            _stateManager = stateManager;

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
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: transformationMatrix);
        }
        protected void EndSpriteBatch()
        {
            _spriteBatch.End();
        }


        protected abstract Matrix? GetGameplayTransformMatrix();
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
            BeginSpriteBatch(GetGameplayTransformMatrix());
            DrawGameplay(gameTime);
            EndSpriteBatch();
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
