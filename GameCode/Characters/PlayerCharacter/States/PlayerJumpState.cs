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
            //if (_player.HeldGun != null)
            //{
            //    _player.HeldGun.HandleGunShoot();
            //    _player.HeldGun.HandleGunReload();
            //}

            // Move player
            //_player.PlayerMove(delta);

            if (_player.IsGrounded(_player.currentMovement))
            {
                _player.Velocity.Y = 0f;

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
