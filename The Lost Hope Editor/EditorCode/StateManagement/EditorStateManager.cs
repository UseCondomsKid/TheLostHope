using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHopeEditor.EditorCode.StateManagement
{
    // Class that manages the states/scenes
    public class EditorStateManager
    {
        // Is triggered beofre the state changes
        public event Action OnStatePreChanged;
        // Is triggered when the state changes
        public event Action OnStateChanged;
        public bool IsStateChanging { get; private set; }

        // a reference to the currently activate game state
        private EditorState _currentGameState;

        public EditorState CurrentGameState { get { return _currentGameState; } }

        public EditorStateManager()
        {
            IsStateChanging = false;
        }

        public void Update(GameTime gameTime)
        {
            // if enabled is false we return
            if (IsStateChanging) return;

            // We update the current state if it's not null
            if (_currentGameState != null)
            {
                _currentGameState.Update(gameTime);
            }
        }
        public void DrawGame(GameTime gameTime)
        {
            if (IsStateChanging) return;

            // We draw the current state if it's not null
            if (_currentGameState != null)
            {
                _currentGameState.DrawGame(gameTime);
            }
        }
        public void DrawImGui(GameTime gameTime)
        {
            if (IsStateChanging) return;

            // We draw the current state if it's not null
            if (_currentGameState != null)
            {
                ImGui.Begin(_currentGameState.ToolName, ImGuiWindowFlags.MenuBar);
                _currentGameState.DrawImGui(gameTime);
                ImGui.End();
            }
        }

        // Changes the current state
        public void SetState(EditorState state)
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
