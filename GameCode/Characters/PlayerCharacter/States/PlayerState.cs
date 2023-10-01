using TheLostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    // Base player State
    public class PlayerState : CharacterState
    {
        protected Player _player;
        public PlayerState(Character character, object animKey) : base(character, animKey)
        {
            _player = (Player)character;
        }

        public override void Update(float delta)
        {
            _player.ApplyGravity();
        }
    }
}
