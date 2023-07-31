using LostHope.Engine.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.GameStates
{
    public class GameplayState : GameState
    {
        public GameplayState(Game1 gameRef, GameStateManager stateManager) : base(gameRef, stateManager)
        {
        }
    }
}
