using TheLostHope.GameCode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.Engine.StateManagement
{
    // Class that manages the states/scenes
    public class GameStateManager : DrawableGameComponent
    {
        // Is triggered beofre the state changes
        public event Action OnStatePreChanged;
        // Is triggered when the state changes
        public event Action OnStateChanged;
        public bool IsStateChanging { get; private set; }

        // a reference to the currently activate game state
        private GameState _currentGameState;

        public GameState CurrentGameState { get { return _currentGameState; } }

        public GameStateManager(Game game) : base(game)
        {
            IsStateChanging = false;
        }

        public override void Update(GameTime gameTime)
        {
            // if enabled is false we return
            if (!Enabled || IsStateChanging) return;

            // We update the current state if it's not null
            if (_currentGameState != null)
            {
                _currentGameState.Update(gameTime);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            if (IsStateChanging) return;

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

            IsStateChanging = true;
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
            IsStateChanging = false;
        }
    }
}
