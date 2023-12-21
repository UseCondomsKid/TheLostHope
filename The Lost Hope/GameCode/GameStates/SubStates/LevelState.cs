using Humper;
using LDtk;
using LDtkTypes;
using TheLostHope.Engine.ContentManagement;
using TheLostHope.GameCode.Characters.PlayerCharacter;
using TheLostHope.GameCode.Interactables;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TheLostHope.GameCode.LDtkExtensions;
using TheLostHopeEngine.EngineCode.StateManagement;
using Microsoft.Xna.Framework.Graphics;
using TheLostHope.GameCode.GameStates.Core;

namespace TheLostHope.GameCode.GameStates.SubStates
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

        // Camera
        private Vector2 _cameraLastFramePosition;
        private readonly Vector2 _cameraFollowPlayerSpeed = new Vector2(0.1f, 0.2f);

        // Room Data
        private LDtkLevel _levelData;
        private ModifiedLDtkRenderer _levelRenderer;

        private List<CharacterBox> _characterBoxes;

        public List<CharacterBox> CharacterBoxes { get { return _characterBoxes; } }

        public LevelState(Game1 gameRef, StateManager stateManager) : base(gameRef, stateManager)
        {
            _gameplayManager = GameplayManager.Instance;
        }

        public void StartLevel(LDtkWorld world, string levelId, string levelTransitionId = null)
        {
            // Load the level
            _levelData = world.LoadLevel(levelId);

            // Create the renderer
            _levelRenderer = new ModifiedLDtkRenderer(_spriteBatch);
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
            _gameplayManager.PhysicsWorld = new World(_levelData.Size.X, _levelData.Size.Y, collisions.TileSize);

            // Initialize lists for rectangles
            List<Rectangle> rectangles = new List<Rectangle>();
            var tileSize = collisions.TileSize;
            var roomWidth = _levelData.Size.X / tileSize;
            var roomHeight = _levelData.Size.Y / tileSize;

            // Create a 2D boolean array to keep track of processed tiles
            bool[,] processed = new bool[roomWidth, roomHeight];

            for (int y = 0; y < roomHeight; y++)
            {
                for (int x = 0; x < roomWidth; x++)
                {
                    if (collisions.GetValueAt(x, y) == 1 && !processed[x, y])
                    {
                        // Determine the width and height of the rectangle
                        int width = 1;
                        int height = 1;

                        // Expand horizontally
                        while (x + width < roomWidth && collisions.GetValueAt(x + width, y) == 1)
                        {
                            for (int h = 0; h < height; h++)
                            {
                                if (y + h >= roomHeight || collisions.GetValueAt(x + width, y + h) != 1)
                                {
                                    // Stop expanding vertically if there's an empty space
                                    break;
                                }
                            }

                            if (y + height < roomHeight)
                            {
                                // Check if there's a tile in the row above that matches the current width
                                bool canExtend = true;
                                for (int w = 0; w < width; w++)
                                {
                                    if (collisions.GetValueAt(x + w, y + height) != 1)
                                    {
                                        canExtend = false;
                                        break;
                                    }
                                }

                                if (canExtend)
                                {
                                    height++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            width++;
                        }

                        // Mark processed tiles as true
                        for (int i = x; i < x + width; i++)
                        {
                            for (int j = y; j < y + height; j++)
                            {
                                processed[i, j] = true;
                            }
                        }

                        // Create a rectangle
                        rectangles.Add(new Rectangle(x * tileSize, y * tileSize, width * tileSize, height * tileSize));
                    }
                }
            }

            // Create the optimized rectangles and add them to the physics world
            foreach (var rect in rectangles)
            {
                int x = rect.Left / tileSize;
                int y = rect.Top / tileSize;
                int width = rect.Width / tileSize;
                int height = rect.Height / tileSize;

                IBox tile = _gameplayManager.PhysicsWorld.Create(x * tileSize, y * tileSize,
                    width * tileSize, height * tileSize);

                tile.AddTags(CollisionTags.Ground);
            }

            // Setup box list
            _characterBoxes = new List<CharacterBox>();

            // Load and Setup Player
            if (_gameplayManager.Player == null)
            {
                ContentLoader.LoadAsepriteFile(playerData.AsepriteFileName, playerData.AsepriteFileName);
                _gameplayManager.Player = new Player(_gameRef, ContentLoader.GetAsepriteFile(playerData.AsepriteFileName),
                    playerData);
            }

            Vector2 playerSpawnPos = (levelTransitionId == null ? playerData.Position :
                levelTransitionDataList.Find(lt => lt.Id == levelTransitionId).SpawnPosition.ToVector2()) - _levelData.Position.ToVector2();

            _gameplayManager.Player.SpawnCharacter(playerSpawnPos, _gameplayManager.PhysicsWorld);

            _characterBoxes.Add(new CharacterBox(CollisionTags.Player, _gameplayManager.Player.Body));
            AddComponent(_gameplayManager.Player);

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

            // Camera setup zoom
            float widthRatio = _levelData.Size.X / _gameplayManager.GameCamera.Size.X;
            float heightRatio = _levelData.Size.Y / _gameplayManager.GameCamera.Size.Y;
            float minRatio = Math.Min(widthRatio, heightRatio);
            _gameplayManager.GameCamera.Zoom = minRatio > 0.8f ? 1.8f : 1f / minRatio;
            // Set the starter camera's position
            SetCameraPosition(instant: false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SetCameraPosition(instant: false);

            _levelRenderer.UpdateParallaxBackgrounds(gameTime, _gameplayManager.GameCamera.Position, _cameraLastFramePosition);
        }

        protected override Matrix? GetGameplayTransformMatrix()
        {
            return GameplayManager.Instance.GameCamera.Transform;
        }
        protected override void DrawGameplay(GameTime gameTime)
        {
            // Draw Backgrounds
            _levelRenderer.RenderBackgrounds();

            // Draw Room
            _levelRenderer.RenderPrerenderedLevel(_levelData);

            base.DrawGameplay(gameTime);

            // Draw Foregrounds
            _levelRenderer.RenderForegrounds();
        }

        protected void SetCameraPosition(bool instant = false)
        {
            // Set the camera's position to be the player's position
            if (instant)
            {
                _cameraLastFramePosition = _gameplayManager.Player.Position;
                _gameplayManager.GameCamera.SetPosition(_gameplayManager.Player.Position);
            }
            else
            {
                _cameraLastFramePosition = _gameplayManager.GameCamera.Position;
                _gameplayManager.GameCamera.SetPosition(MathHelper.Lerp(_gameplayManager.GameCamera.Position.X,
                    _gameplayManager.Player.Position.X, _cameraFollowPlayerSpeed.X), MathHelper.Lerp(_gameplayManager.GameCamera.Position.Y,
                    _gameplayManager.Player.Position.Y, _cameraFollowPlayerSpeed.Y));
            }

            // Make sure that the camera is still in the bounderies of the level
            // X Bounds
            if (_gameplayManager.GameCamera.Position.X - _gameplayManager.GameCamera.Size.X / 2f < 0)
            {
                _gameplayManager.GameCamera.SetPosition(_gameplayManager.GameCamera.Size.X / 2f, _gameplayManager.GameCamera.Position.Y);
            }
            else if (_gameplayManager.GameCamera.Position.X + _gameplayManager.GameCamera.Size.X / 2f > _levelData.Size.X)
            {
                _gameplayManager.GameCamera.SetPosition(_levelData.Size.X - _gameplayManager.GameCamera.Size.X / 2f, _gameplayManager.GameCamera.Position.Y);
            }
            // Y Bounds
            if (_gameplayManager.GameCamera.Position.Y - _gameplayManager.GameCamera.Size.Y / 2f < 0)
            {
                _gameplayManager.GameCamera.SetPosition(_gameplayManager.GameCamera.Position.X, _gameplayManager.GameCamera.Size.Y / 2f);
            }
            else if (_gameplayManager.GameCamera.Position.Y + _gameplayManager.GameCamera.Size.Y / 2f > _levelData.Size.Y)
            {
                _gameplayManager.GameCamera.SetPosition(_gameplayManager.GameCamera.Position.X, _levelData.Size.Y - _gameplayManager.GameCamera.Size.Y / 2f);
            }

        }
    }
}
