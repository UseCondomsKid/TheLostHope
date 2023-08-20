using LostHope.Engine.Input;
using LostHope.GameCode.Characters.FSM;
using Humper.Responses;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LostHope.Engine.Animations;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
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

            _player.Move(delta);

            if (InputManager.KeyPressed(Keys.W))
            {
                _player.SetVelocityY(-_player.PlayerData.JumpForce);
            }

            if (InputManager.MousePressed(MouseButton.Right))
            {
                _stateMachine.ChangeState(_player.PlayerParryState);
                return;
            }
            if (InputManager.KeyPressed(Keys.LeftShift) && _player.IsMoving())
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
                _stateMachine.ChangeState(_player.PlayerJumpState);
            }
        }
    }
}
