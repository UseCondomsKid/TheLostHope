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

            _player.MoveX(delta, InputManager.IsKeyDown(Keys.D) ? 1 : InputManager.IsKeyDown(Keys.A) ? -1 : 0,
                _player.PlayerData.Speed, _player.PlayerData.Acceleration, _player.PlayerData.Deacceleration,
                1.2f);

            // Apply friction
            // _player.Velocity.X -= _player.Velocity.X * _player.PlayerData.Deacceleration;

            // Input
            //if (InputManager.IsKeyDown(Keys.D))
            //{
            //    _player.Velocity.X += _player.PlayerData.Acceleration;
            //}
            //else if (InputManager.IsKeyDown(Keys.A))
            //{
            //    _player.Velocity.X -= _player.PlayerData.Acceleration;
            //}
            //if (InputManager.KeyPressed(Keys.W))
            //{
            //    _player.Velocity.Y -= _player.PlayerData.JumpForce;
            //    _player.IsGrounded = false;
            //}

            //if (_player.HeldGun != null)
            //{
            //    _player.HeldGun.HandleGunShoot();
            //    _player.HeldGun.HandleGunReload();
            //}

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

            //if (!_player.IsGrounded())
            //{
            //    _stateMachine.ChangeState(_player.PlayerJumpState);
            //}
        }
    }
}
