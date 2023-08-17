using LostHope.GameCode.Characters.FSM;
using System;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerRunState : PlayerGroundedState
    {
        public PlayerRunState(Character character, object animKey) : base(character, animKey)
        {
        }
    }
}
