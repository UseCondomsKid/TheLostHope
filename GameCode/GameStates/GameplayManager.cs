using LDtk;
using LDtkTypes;
using LostHope.Engine.Camera;
using LostHope.Engine.ContentLoading;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.UI.Elements;
using LostHope.GameCode.Weapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostHope.GameCode.GameStates
{
    public class GameplayManager
    {
        private static GameplayManager _instance;
        private static readonly object _lock = new object();
        public static GameplayManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GameplayManager();
                    }
                }
                return _instance;
            }
        }

        // General Gameplay Globals
        private OrthographicCamera _camera;
        public OrthographicCamera GameCamera { get { return _camera; } set { _camera = value; } }
        public LDtkWorld CurrentWorld { get; private set; }
        public LevelState CurrentLevelState { get; private set; }
        public bool Started { get; private set; }

        // UI Menus
        private Menu _gameMenu;
        private Menu _pauseMenu;

        // Player specific
        public Player Player { get; set; }
        public Dictionary<string, LDtkGun> UnlockedGuns { get; private set; }
        public Gun EquippedGun { get; private set; }


        private Game1 _gameRef;
        private GameStateManager _stateManager;

        public GameplayManager()
        {
            Started = false;
        }

        public void Start(Game1 gameRef, GameStateManager stateManager, Guid startWorldId)
        {
            // Set these up first
            _gameRef = gameRef;
            _stateManager = stateManager;

            // Initialize the camera
            _camera = new OrthographicCamera(_gameRef, Globals.CurrentScreenHeight);

            // Set starter world
            SetWorld(startWorldId);

            // Player specific
            UnlockedGuns = new Dictionary<string, LDtkGun>();

            // Set flag
            Started = true;

            // Menus
            _gameMenu = new Menu(null, SwitchToPauseMenu);
            _pauseMenu = new Menu(SwitchToGameMenu, SwitchToGameMenu);

            UIEText gameMenuText = new UIEText(UIAnchor.Top, 0f, .1f, _gameMenu, "I'm the game menu", 20f, Color.White);
            _gameMenu.Children.Add(gameMenuText);

            UIEText pauseMenuTitle = new UIEText(UIAnchor.Top, 0f, .2f, _pauseMenu, "Paused", 35f, Color.White);

            UIEButton resumeButton = new UIEButton(UIAnchor.Center, 0f, -.1f, _pauseMenu, new Vector2(100, 100), ContentLoader.GetTexture("button"),
                SwitchToGameMenu);
            UIEText resumeButtonText = new UIEText(UIAnchor.Center, 0f, 0f, resumeButton, "Resume", 20f, Color.Black);
            resumeButton.Children.Add(resumeButtonText);

            UIEButton exitButton = new UIEButton(UIAnchor.Center, 0f, .1f, _pauseMenu, new Vector2(100, 100), ContentLoader.GetTexture("button"),
                BackToMainMenuState);
            UIEText extButtonText = new UIEText(UIAnchor.Center, 0f, 0f, exitButton, "Exit", 20f, Color.Black);
            exitButton.Children.Add(extButtonText);

            _pauseMenu.Children.Add(pauseMenuTitle);
            _pauseMenu.Children.Add(resumeButton);
            _pauseMenu.Children.Add(exitButton);

            _pauseMenu.SetSelected(resumeButton);
        }

        private void BackToMainMenuState()
        {
            _stateManager.SetState(_gameRef.MainMenuState);
            _gameRef.IsPaused = false;
        }
        private void SwitchToPauseMenu()
        {
            _gameRef.UIManager.SetActiveMenu(_pauseMenu);
            _gameRef.IsPaused = true;
        }
        private void SwitchToGameMenu()
        {
            _gameRef.UIManager.SetActiveMenu(_gameMenu);
            _gameRef.IsPaused = false;
        }


        public void Update(GameTime gameTime)
        {
            if (!Started) return;
            _camera.Update(gameTime);
        }


        public void SetWorld(Guid worldId)
        {
            // Load the world here
            CurrentWorld = ContentLoader.LDtkFile.LoadWorld(worldId);
        }
        public void LoadLevel(string levelId, string levelTransitionId = null)
        {
            if (!Started) return;

            // Create the level state
            CurrentLevelState = new LevelState(_gameRef, _stateManager);

            _camera.Zoom = 1.0f;

            _stateManager.SetState(CurrentLevelState);
            _gameRef.UIManager.SetActiveMenu(_gameMenu);
            CurrentLevelState.StartLevel(CurrentWorld, levelId, levelTransitionId);
        }

        public bool AddGun(LDtkGun gunData)
        {
            if (!Started) return false;

            if (!HasGun(gunData))
            {
                UnlockedGuns.Add(gunData.Name, gunData);
                return true;
            }

            return false;
        }
        public bool HasGun(LDtkGun gunData)
        {
            if (!Started) return false;

            return UnlockedGuns.ContainsKey(gunData.Name);
        }
        public void EquipGun(LDtkGun gunData)
        {
            if (!Started) return;

            if (gunData == null)
            {
                EquippedGun = null;
            }
            else if (HasGun(gunData))
            {
                // EquippedGun = gunData;
            }
            else
            {
                EquippedGun = null;
            }
        }
    }
}
