using Apos.Input;
using LDtkTypes;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace LostHope.GameCode.GameStates
{
    public class MainMenuState : GameState
    {
        private ICondition playButton;

        public MainMenuState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager) : base(gameRef, stateManager, uiManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            playButton = new AnyCondition(new KeyboardCondition(Keys.P), new GamePadCondition(GamePadButton.A, 0));

            // Start the gameplay manager
            GameplayManager.Instance.Start(_gameRef, _stateManager, _uiManager, Worlds.World.Iid);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (playButton.Pressed())
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
