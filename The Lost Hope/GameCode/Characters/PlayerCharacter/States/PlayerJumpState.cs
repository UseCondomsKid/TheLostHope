using Humper.Responses;
using TheLostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerJumpState : PlayerState
    {
        public PlayerJumpState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.SetPlayerJumping(true);
            _player.SetVelocityY(-_player.PlayerData.JumpForce);
            _stateMachine.ChangeState(_player.PlayerInAirState);
        }
    }
}
