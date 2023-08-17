using LostHope.Engine.ContentLoading;
using LostHope.Engine.Rendering;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.Rooms;
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

            ContentLoader.LoadAsepriteFile("Player", "Player");
            _player = new Player(_gameRef, _roomData.PhysicsWorld, ContentLoader.GetAsepriteFile("Player"),
                _roomData.PlayerData);
            _player.SpawnCharacter(_roomData.PlayerData.Position - _roomData.LevelData.Position.ToVector2());
            AddComponent(_player);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _gameRef.GameCamera.Position = _player.Position;
        }

        protected override void DrawGameplay(GameTime gameTime)
        {
            // Draw Room
            _roomData.LevelRenderer.RenderPrerenderedLevel(_roomData.LevelData);

            base.DrawGameplay(gameTime);
        }
    }
}
