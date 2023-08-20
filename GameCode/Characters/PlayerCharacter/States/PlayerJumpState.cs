using Humper.Responses;
using LostHope.Engine.Input;
using LostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerJumpState : PlayerState
    {
        public PlayerJumpState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _player.Move(delta);

            if (_player.IsTouchingCeiling())
            {
                _player.SetVelocityY(0f);
            }

            if (_player.IsGrounded())
            {
                if (_player.IsMoving())
                {
                    _stateMachine.ChangeState(_player.PlayerRunState);
                }
                else
                {
                    _stateMachine.ChangeState(_player.PlayerLandState);
                }
            }
        }
    }
}
