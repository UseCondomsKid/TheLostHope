using Humper;
using LDtk;
using LDtkTypes;
using LostHope.Engine.ContentLoading;
using LostHope.Engine.Rendering;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.Interactables;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LostHope.GameCode.GameStates
{
    public class LevelState : GameState
    {
        public class CharacterBox
        {
            public CollisionTags Tag { get; private set; }
            public IBox Box { get; private set; }

            public CharacterBox(CollisionTags tag, IBox box)
            {
                Tag = tag;
                Box = box;
            }
        }

        private GameplayManager _gameplayManager;

        // Room Data
        private LDtkLevel _levelData;
        private ModifiedLDtkRenderer _levelRenderer;
        private World _physicsWorld;

        private Player _player;
        private List<CharacterBox> _characterBoxes;

        public Player Player { get { return _player; } }
        public List<CharacterBox> CharacterBoxes { get { return _characterBoxes; } }

        public LevelState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager) : base(gameRef, stateManager, uiManager)
        {
            _gameplayManager = GameplayManager.Instance;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public void StartLevel(LDtkWorld world, string levelId, string levelTransitionId = null)
        {
            // Load the level
            _levelData = world.LoadLevel(levelId);

            // Create the renderer
            _levelRenderer = new ModifiedLDtkRenderer(Globals.SpriteBatch);
            // Pre-render the level
            _levelRenderer.PrerenderLevel(_levelData);

            // Load entities
            var playerData = _levelData.GetEntity<LDtkPlayer>();
            var enemyDataList = new List<LDtkEnemy>();
            foreach (var e in _levelData.GetEntities<LDtkEnemy>())
            {
                enemyDataList.Add(e);
            }
            var gunDataList = new List<LDtkGun>();
            foreach (var g in _levelData.GetEntities<LDtkGun>())
            {
                gunDataList.Add(g);
            }
            var levelTransitionDataList = new List<LDtkLevelTransition>();
            foreach (var t in _levelData.GetEntities<LDtkLevelTransition>())
            {
                levelTransitionDataList.Add(t);
            }

            // Physics
            LDtkIntGrid collisions = _levelData.GetIntGrid("Collisions");
            var tileSize = collisions.TileSize;
            _physicsWorld = new World(_levelData.Size.X, _levelData.Size.Y, collisions.TileSize);

            // Spawn all tiles
            for (int x = 0; x < (_levelData.Size.X / collisions.TileSize); x++)
            {
                for (int y = 0; y < (_levelData.Size.Y / collisions.TileSize); y++)
                {
                    long intGridValue = collisions.GetValueAt(x, y);
                    if (intGridValue is 1)
                    {
                        IBox tile = _physicsWorld.Create(x * collisions.TileSize, y * collisions.TileSize,
                            collisions.TileSize, collisions.TileSize);

                        tile.AddTags(CollisionTags.Ground);
                    }
                }
            }

            // Camera setup zoom
            float widthRatio = _levelData.Size.X /  _gameplayManager.GameCamera.Size.X;
            float heightRatio = _levelData.Size.Y / _gameplayManager.GameCamera.Size.Y;
            float minRatio = Math.Min(widthRatio, heightRatio);
            _gameplayManager.GameCamera.Zoom = minRatio > 0.8f ? 1.8f : 1f / minRatio;

            // Setup box list
            _characterBoxes = new List<CharacterBox>();

            // Load and Setup Player
            ContentLoader.LoadAsepriteFile("Player", "Player");
            _player = new Player(_gameRef, _physicsWorld, ContentLoader.GetAsepriteFile("Player"),
                playerData);

            _player.SpawnCharacter((levelTransitionId == null ?
                playerData.Position :
                levelTransitionDataList.Find(lt => lt.Id == levelTransitionId).SpawnPosition.ToVector2())
                - _levelData.Position.ToVector2()
                );

            _characterBoxes.Add(new CharacterBox(CollisionTags.Player, _player.Body));
            AddComponent(_player);

            // Load and Setup Enemies
            // TODO: Create enemies and other characters and add their boxes to the list

            // Final thing to do is load and setup interactables
            // Load and Setup Level transitions
            foreach (var levelTransition in levelTransitionDataList)
            {
                var l = new LevelTransition(_gameRef, new Rectangle(
                    levelTransition.Position.ToPoint() - _levelData.Position, levelTransition.Size.ToPoint()),
                    levelTransition);

                l.Initialize();

                AddComponent(l);
            }
            // Load and Setup Guns
            foreach (var gun in gunDataList)
            {
                if (!_gameplayManager.HasGun(gun))
                {
                    var g = new GunInteractable(_gameRef, new Rectangle(
                        gun.Position.ToPoint() - _levelData.Position, gun.Size.ToPoint()),
                        gun);

                    g.Initialize();

                    AddComponent(g);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Set the camera's position to be the player's position
            _gameplayManager.GameCamera.Position = _player.Position;

            // Make sure that the camera is still in the bounderies of the level
            // X Bounds
            if (_gameplayManager.GameCamera.Position.X - (_gameplayManager.GameCamera.Size.X / 2f) < 0)
            {
                _gameplayManager.GameCamera.Position = new Vector2(_gameplayManager.GameCamera.Size.X / 2f, _gameplayManager.GameCamera.Position.Y);
            }
            else if (_gameplayManager.GameCamera.Position.X + (_gameplayManager.GameCamera.Size.X / 2f) > _levelData.Size.X)
            {
                _gameplayManager.GameCamera.Position = new Vector2(_levelData.Size.X - (_gameplayManager.GameCamera.Size.X / 2f), _gameplayManager.GameCamera.Position.Y);
            }
            // Y Bounds
            if (_gameplayManager.GameCamera.Position.Y - (_gameplayManager.GameCamera.Size.Y / 2f) < 0)
            {
                _gameplayManager.GameCamera.Position = new Vector2(_gameplayManager.GameCamera.Position.X, _gameplayManager.GameCamera.Size.Y / 2f);
            }
            else if (_gameplayManager.GameCamera.Position.Y + (_gameplayManager.GameCamera.Size.Y / 2f) > _levelData.Size.Y)
            {
                _gameplayManager.GameCamera.Position = new Vector2(_gameplayManager.GameCamera.Position.X, _levelData.Size.Y - (_gameplayManager.GameCamera.Size.Y / 2f));
            }
        }

        protected override Matrix? GetGameplayTransformMatrix()
        {
            return GameplayManager.Instance.GameCamera.Transform;
        }
        protected override void DrawGameplay(GameTime gameTime)
        {
            // Draw Room
            _levelRenderer.RenderPrerenderedLevel(_levelData);

            base.DrawGameplay(gameTime);
        }
    }
}
