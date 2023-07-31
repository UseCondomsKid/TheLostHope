using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.StateManagement
{
    // Class that manages the states/scenes
    public class GameStateManager : DrawableGameComponent
    {
        // Is triggered beofre the state changes
        public static event Action OnStatePreChanged;
        // Is triggered when the state changes
        public static event Action OnStateChanged;

        // a reference to the currently activate game state
        private GameState _currentGameState;

        public GameState CurrentGameState { get { return _currentGameState; } }

        public GameStateManager(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // if enabled is false we return
            if (!Enabled) return;

            // We update the current state if it's not null
            if (_currentGameState != null)
            {
                _currentGameState.Update(gameTime);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // We draw the current state if it's not null
            if (_currentGameState != null)
            {
                _currentGameState.Draw(gameTime);
            }
        }

        // Changes the current state
        public void SetState(GameState state)
        {
            // if we have a state already active, we call it's exit function
            if (_currentGameState != null)
            {
                _currentGameState.Exit();
            }

            // Invoke the pre changed event
            OnStatePreChanged?.Invoke();

            // Change state
            _currentGameState = state;
            // Call enter on the new state
            _currentGameState.Enter();

            // Invoke changed event
            OnStateChanged?.Invoke();

            // Call Post Enter of the new state
            _currentGameState.PostEnter();
        }
    }
}
