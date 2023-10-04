using TheLostHope.GameCode.Characters.FSM;
using Humper.Responses;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    // Base player grounded state, aka when the player is on the ground
    public class PlayerGroundedState : PlayerState
    {
        public PlayerGroundedState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _player.MoveGround(delta);

            //if (_player.JumpInput.Pressed() && _player.IsGrounded())
            //{
            //    _stateMachine.ChangeState(_player.PlayerJumpState);
            //}

            if (_player.PlayerLastGroundedTime > 0f && _player.PlayerLastJumpTime > 0f && !_player.PlayerJumping
                && !_player.IsTouchingCeiling())
            {
                _stateMachine.ChangeState(_player.PlayerJumpState);
            }

            if (_player.ParryInput.Pressed())
            {
                _stateMachine.ChangeState(_player.PlayerParryState);
                return;
            }
            if (_player.RollInput.Pressed() && _player.IsMoving())
            {
                _stateMachine.ChangeState(_player.PlayerRollState);
                return;
            }

            if (_player.IsMoving())
            {
                _stateMachine.ChangeState(_player.PlayerRunState);
            }
            else
            {
                _stateMachine.ChangeState(_player.PlayerIdleState);
            }

            if (!_player.IsGrounded())
            {
                _stateMachine.ChangeState(_player.PlayerInAirState);
            }
        }
    }
}
