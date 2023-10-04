using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEngine.EngineCode.StateManagement;

namespace TheLostHope.GameCode.GameStates.Core
{
    public abstract class GameState : State
    {
        protected new Game1 _gameRef;

        public GameState(Game1 gameRef, StateManager stateManager) : base(gameRef, Game1.SpriteBatch, stateManager)
        {
            _gameRef = gameRef;
        }
    }
}
