using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TheLostHopeEditor.EditorCode.StateManagement
{
    // This represents a scene
    public abstract class EditorState 
    {
        // A reference to the state machine
        protected readonly EditorStateManager _stateManager;
        // A reference to Game1
        protected readonly Game1 _gameRef;
        // Name of the editor tool
        protected readonly string _name;

        // Public getter for the name
        public string ToolName { get { return _name; } }

        // Constructor
        public EditorState(Game1 gameRef, EditorStateManager stateManager, string name)
        {
            _gameRef = gameRef;
            _stateManager = stateManager;
            _name = name;
        }

        // Called when the state is activated
        public virtual void Enter()
        {
        }
        // Called just after enter
        public virtual void PostEnter()
        {
        }
        // Called when the state is exited
        public virtual void Exit()
        {
        }


        public abstract void Update(GameTime gameTime);
        public abstract void DrawGame(GameTime gameTime);
        public abstract void DrawImGui(GameTime gameTime);
    }
}
