using Apos.Input;
using LDtkTypes;
using LostHope.Engine.ContentLoading;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostHope.GameCode.GameStates
{
    public class MainMenuState : GameState
    {
        private Menu _mainMenu;
        private UIEText _titleText;

        private UIEText _playButtonText;
        private UIEButton _playButton;

        private UIEText _exitButtonText;
        private UIEButton _exitButton;

        public MainMenuState(Game1 gameRef, GameStateManager stateManager) : base(gameRef, stateManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _mainMenu = new Menu(MainMenuBack, MainMenuEscape);

            ContentLoader.LoadTexture("button", "button");
            var buttonTex = ContentLoader.GetTexture("button");

            _titleText = new UIEText(UIAnchor.Top, 0f, .1f, _mainMenu, "The Lost Hope", 35f, Color.White);
            _mainMenu.Children.Add(_titleText);

            _playButton = new UIEButton(UIAnchor.Center, 0f, -.1f, _mainMenu, new Vector2(100, 100), buttonTex, PlayButtonEnter);
            _playButtonText = new UIEText(UIAnchor.Center, 0f, 0f, _playButton, "Play", 20f, Color.Black);
            _playButton.Children.Add(_playButtonText);
            _mainMenu.Children.Add(_playButton);

            _exitButton = new UIEButton(UIAnchor.Center, 0f, .1f, _mainMenu, new Vector2(100, 100), buttonTex, ExitButtonEnter);
            _exitButtonText = new UIEText(UIAnchor.Center, 0f, 0f, _exitButton, "Exit", 20f, Color.Black);
            _exitButton.Children.Add(_exitButtonText);
            _mainMenu.Children.Add(_exitButton);

            _mainMenu.SetSelected(_playButton);
            _gameRef.UIManager.SetActiveMenu(_mainMenu);

            // Start the gameplay manager
            GameplayManager.Instance.Start(_gameRef, _stateManager, Worlds.World.Iid);
        }

        private void MainMenuBack()
        {
            Debug.WriteLine("Back");
        }
        private void MainMenuEscape()
        {
            Debug.WriteLine("Escape");
        }

        private void PlayButtonEnter()
        {
            // Load the first level
            GameplayManager.Instance.LoadLevel("Level_0");

            _gameRef.UIManager.SetActiveMenu(null);
        }

        private void ExitButtonEnter()
        {
            _gameRef.Exit();
        }

        protected override Matrix? GetGameplayTransformMatrix()
        {
            return null;
        }
    }
}
