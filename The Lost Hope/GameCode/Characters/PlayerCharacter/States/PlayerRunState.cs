using TheLostHope.GameCode.Characters.FSM;
using System;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerRunState : PlayerGroundedState
    {
        public PlayerRunState(Character character, object animKey) : base(character, animKey)
        {
        }
    }
}
