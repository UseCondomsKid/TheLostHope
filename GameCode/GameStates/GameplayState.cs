using LostHope.Engine.StateManagement;
using LostHope.Engine.UI;
using LostHope.GameCode.Characters.PlayerCharacter;
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
        private AsepriteFile _playerAseprite;
        private Player _player;

        public GameplayState(Game1 gameRef, GameStateManager stateManager, UIManager uiManager) : base(gameRef, stateManager, uiManager)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }
        protected override void LoadContent()
        {
            _playerAseprite = _gameRef.Content.Load<AsepriteFile>("Player");

            _player = new Player(_gameRef, null, _playerAseprite);
        }
    }
}
