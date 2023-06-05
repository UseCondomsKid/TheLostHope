using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.StateManagement
{
    // This represents a scene
    public class GameState 
    {
        // A list of child components
        // Child components will update and draw automatically.
        protected List<GameComponent> _childComponents;

        // A reference to the state machine
        protected readonly GameStateManager _stateManager;
        // A reference to Game1
        protected readonly Game1 _gameRef;

        // Public proptery to access the child components
        public List<GameComponent> Components { get { return _childComponents; } }

        // Constructor
        public GameState(Game1 gameRef, GameStateManager stateManager)
        {
            _gameRef = gameRef;
            _stateManager = stateManager;

            // Initialise list
            _childComponents = new List<GameComponent>();
        }

        // Called when the state is activated
        public virtual void Enter()
        {
            Debug.WriteLine("Entering State: " + this);

            _childComponents = new List<GameComponent>();

            LoadContent();
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

        public virtual void Draw(GameTime gameTime)
        {
            // Update all child components if they are Drawabke and visible
            foreach (var child in _childComponents)
            {
                if (child is DrawableGameComponent component && component.Visible)
                {
                    component.Draw(gameTime);
                }
            }
        }

        // Called when the state is exited
        public virtual void Exit()
        {
            Debug.WriteLine("Exiting State: " + this);

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
