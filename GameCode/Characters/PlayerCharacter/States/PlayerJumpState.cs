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

            _player.MoveX(delta, InputManager.IsKeyDown(Keys.D) ? 1 : InputManager.IsKeyDown(Keys.A) ? -1 : 0,
                _player.PlayerData.Speed, _player.PlayerData.Acceleration, _player.PlayerData.Deacceleration,
                1.2f);

            _player.SetVelocityY(_player.PlayerData.GravityScale);
            //if (_player.HeldGun != null)
            //{
            //    _player.HeldGun.HandleGunShoot();
            //    _player.HeldGun.HandleGunReload();
            //}

            if (_player.IsGrounded())
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
