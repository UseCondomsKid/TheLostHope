using Apos.Input;
using LDtkTypes;
using TheLostHope.Engine.ContentManagement;
using TheLostHope.GameCode.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using TheLostHopeEngine.EngineCode.StateManagement;
using TheLostHopeEngine.EngineCode.UI;
using TheLostHope.GameCode.GameStates.Core;

namespace TheLostHope.GameCode.GameStates.SubStates
{
    public class MainMenuState : GameState
    {
        private Menu _mainMenu;
        private UIEText _titleText;

        private UIEText _playButtonText;
        private UIEButton _playButton;

        private UIEText _exitButtonText;
        private UIEButton _exitButton;

        public MainMenuState(Game1 gameRef, StateManager stateManager) : base(gameRef, stateManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _mainMenu = new Menu(_gameRef.GraphicsDevice);

            ContentLoader.LoadTexture("button", "Graphics/button");
            var buttonTex = ContentLoader.GetTexture("button");

            _titleText = new UIEText(_gameRef.GraphicsDevice, UIAnchor.Top, 0f, .1f, _mainMenu, "The Lost Hope", 35f, Color.White);
            _mainMenu.Children.Add(_titleText);

            _playButton = new UIEButton(_gameRef.GraphicsDevice, UIAnchor.Center, 0f, -.1f, _mainMenu, new Vector2(100, 100), buttonTex, PlayButtonEnter);
            _playButtonText = new UIEText(_gameRef.GraphicsDevice, UIAnchor.Center, 0f, 0f, _playButton, "Play", 20f, Color.Black);
            _playButton.Children.Add(_playButtonText);
            _mainMenu.Children.Add(_playButton);

            _exitButton = new UIEButton(_gameRef.GraphicsDevice, UIAnchor.Center, 0f, .1f, _mainMenu, new Vector2(100, 100), buttonTex, ExitButtonEnter);
            _exitButtonText = new UIEText(_gameRef.GraphicsDevice, UIAnchor.Center, 0f, 0f, _exitButton, "Exit", 20f, Color.Black);
            _exitButton.Children.Add(_exitButtonText);
            _mainMenu.Children.Add(_exitButton);

            _mainMenu.SetSelected(_playButton);
            _gameRef.UIManager.SetActiveMenu(_mainMenu);

            // Start the gameplay manager
            GameplayManager.Instance.Start(_gameRef, _stateManager, Worlds.World.Iid);
        }

        private void PlayButtonEnter()
        {
            // Load the first level
            GameplayManager.Instance.LoadLevel("Level_0");
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
