using LDtkTypes;
using LostHope.Engine.Input;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LostHope.GameCode.GameStates
{
    public class MainMenuState : GameState
    {
        public MainMenuState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager) : base(gameRef, stateManager, uiManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // Start the gameplay manager
            GameplayManager.Instance.Start(_gameRef, _stateManager, _uiManager, Worlds.World.Iid);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Keys.P))
            {
                // Load the first level
                GameplayManager.Instance.LoadLevel("Level_0");
            }
        }

        protected override Matrix? GetGameplayTransformMatrix()
        {
            return null;
        }
    }
}
