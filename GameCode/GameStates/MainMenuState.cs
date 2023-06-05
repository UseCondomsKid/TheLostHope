using LostHope.Engine;
using LostHope.Engine.Rendering;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.GameStates
{
    public class MainMenuState : GameState
    {
        private SpriteFont _font;

        private Button _playButton;
        private Button _quitButton;

        public MainMenuState(Game1 gameRef, GameStateManager stateManager) : base(gameRef, stateManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _font = Globals.Font;

            _playButton = new Button(_gameRef, _gameRef.UIManager,  new Rectangle(0, 0, 100, 80));
            _quitButton = new Button(_gameRef, _gameRef.UIManager, new Rectangle(0, 100, 100, 80), UIAnchor.Center, false);

            // Set text for each one
            _playButton.SetText(_font, "Play");
            _quitButton.SetText(_font, "Quit");

            // On Click events
            _playButton.OnClick += PlayButtonPressed;
            _quitButton.OnClick += QuitButtonPressed;

            AddComponent(_playButton);
            AddComponent(_quitButton);
        }

        // Transition to Level1
        private void PlayButtonPressed()
        {
            return;
        }
        // Exit the game
        private void QuitButtonPressed()
        {
            _gameRef.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var spriteBatch = Globals.SpriteBatch;

            spriteBatch.Begin(transformMatrix: _gameRef.UIManager.CenterAnchorMatrix);

            spriteBatch.DrawString(_font, "The Lost Delivery",
                SpriteBatchExtensions.GetTextPosition(_font, "The Lost Delivery", new Vector2(0, -200), 1.4f),
                Color.White, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0f);

            spriteBatch.End();
        }
    }
}
