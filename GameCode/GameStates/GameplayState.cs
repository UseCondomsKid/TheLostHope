using LostHope.Engine.ContentLoading;
using LostHope.Engine.Rendering;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.Rooms;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.GameStates
{
    public class GameplayState : GameState
    {
        private RoomData _roomData;

        private Player _player;

        public GameplayState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager) : base(gameRef, stateManager, uiManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Room room = new Room();
            _roomData = room.Initialize("Level_0");

            // Camera setup zoom
            float widthRatio = _roomData.LevelData.Size.X / _gameRef.GameCamera.Size.X;
            float heightRatio = _roomData.LevelData.Size.Y / _gameRef.GameCamera.Size.Y;
            float minRatio = Math.Min(widthRatio, heightRatio);
            _gameRef.GameCamera.Zoom = minRatio > 0.8f ? 1.8f : 1f / minRatio;

            // Load and Setup Player
            ContentLoader.LoadAsepriteFile("Player", "Player");
            _player = new Player(_gameRef, _roomData.PhysicsWorld, ContentLoader.GetAsepriteFile("Player"),
                _roomData.PlayerData);
            _player.SpawnCharacter(_roomData.PlayerData.Position - _roomData.LevelData.Position.ToVector2());
            AddComponent(_player);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Set the camera's position to be the player's position
            _gameRef.GameCamera.Position = _player.Position;

            // Make sure that the camera is still in the bounderies of the level
            // X Bounds
            if (_gameRef.GameCamera.Position.X - (_gameRef.GameCamera.Size.X / 2f) < 0)
            {
                _gameRef.GameCamera.Position = new Vector2(_gameRef.GameCamera.Size.X / 2f, _gameRef.GameCamera.Position.Y);
            }
            else if (_gameRef.GameCamera.Position.X + (_gameRef.GameCamera.Size.X / 2f) > _roomData.LevelData.Size.X)
            {
                _gameRef.GameCamera.Position = new Vector2(_roomData.LevelData.Size.X - (_gameRef.GameCamera.Size.X / 2f), _gameRef.GameCamera.Position.Y);
            }
            // Y Bounds
            if (_gameRef.GameCamera.Position.Y - (_gameRef.GameCamera.Size.Y / 2f) < 0)
            {
                _gameRef.GameCamera.Position = new Vector2(_gameRef.GameCamera.Position.X, _gameRef.GameCamera.Size.Y / 2f);
            }
            else if (_gameRef.GameCamera.Position.Y + (_gameRef.GameCamera.Size.Y / 2f) > _roomData.LevelData.Size.Y)
            {
                _gameRef.GameCamera.Position = new Vector2(_gameRef.GameCamera.Position.X, _roomData.LevelData.Size.Y - (_gameRef.GameCamera.Size.Y / 2f));
            }
        }

        protected override void DrawGameplay(GameTime gameTime)
        {
            // Draw Room
            _roomData.LevelRenderer.RenderPrerenderedLevel(_roomData.LevelData);

            base.DrawGameplay(gameTime);
        }
    }
}
