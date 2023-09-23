using LDtk;
using LDtkTypes;
using LostHope.Engine.Camera;
using LostHope.Engine.ContentLoading;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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


        // Player specific
        public Dictionary<string, LDtkGun> UnlockedGuns { get; private set; }


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
            _camera.Position = new Vector2(0, 0);

            // Set starter world
            SetWorld(startWorldId);

            // Player specific
            UnlockedGuns = new Dictionary<string, LDtkGun>();

            // Set flag
            Started = true;
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
            return UnlockedGuns.ContainsKey(gunData.Name);
        }

    }
}
