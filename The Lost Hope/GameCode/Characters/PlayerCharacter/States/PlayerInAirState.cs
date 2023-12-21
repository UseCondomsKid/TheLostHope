using TheLostHope.GameCode.Characters.FSM;
using TheLostHope.GameCode.Characters.PlayerCharacter.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerInAirState : PlayerState
    {
        public PlayerInAirState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // Make sure we catch all released inputs
            if (_player.PlayerJumpInputReleased)
            {
                _player.SetVelocityY(_player.Velocity.Y * _player.PlayerData.JumpForceCutOnJumpRelease);
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _player.MoveAir(delta);

            if (_player.PlayerLastGroundedTime > 0f && _player.PlayerLastJumpTime > 0f && !_player.PlayerJumping &&
                !_player.IsEquippedGunReloading())
            {
                _stateMachine.ChangeState(_player.PlayerJumpState);
            }
            else if (_player.PlayerLastGroundedTime > 0f && _player.PlayerLastRollTime > 0f && _player.IsMoving())
            {
                _stateMachine.ChangeState(_player.PlayerRollState);
            }
            else
            {
                if (_player.PlayerJumpInputReleased && _player.PlayerJumping)
                {
                    _player.SetVelocityY(_player.Velocity.Y * _player.PlayerData.JumpForceCutOnJumpRelease);
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
}
