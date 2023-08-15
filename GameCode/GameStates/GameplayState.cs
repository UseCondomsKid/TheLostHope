using LostHope.Engine.ContentLoading;
using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.Rooms;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.GameStates
{
    public class GameplayState : GameState
    {
        private RoomData _roomData;

        public GameplayState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager) : base(gameRef, stateManager, uiManager)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Room room = new Room();
            _roomData = room.Initialize("Level_0");

            ContentLoader.LoadAsepriteFile("Player", "Player");
            Player player = new Player(_gameRef, _roomData.PhysicsWorld, ContentLoader.GetAsepriteFile("Player"),
                _roomData.PlayerData);
            AddComponent(player);
        }

        protected override void DrawGameplay(GameTime gameTime)
        {
            // Draw Room
            _roomData.LevelRenderer.RenderPrerenderedLevel(_roomData.LevelData);

            base.DrawGameplay(gameTime);
        }
    }
}
