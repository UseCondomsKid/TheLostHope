using LostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerRollState : PlayerState
    {
        public PlayerRollState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _player.IFrame = true;
            // _player.Velocity = new Vector2(_player.FacingDirection * _player.PlayerData.RollVelocity,
                // 0);
        }
        public override void Exit()
        {
            base.Exit();

            _player.Velocity = Vector2.Zero;
            _player.IFrame = false;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (_player.HeldGun != null)
            {
                // _player.HeldGun.HandleGunReload();
            }

            if (_isAnimationFinished)
            {
                if (!_player.IsGrounded)
                {
                    _stateMachine.ChangeState(_player.PlayerJumpState);
                }
                else
                {
                    _stateMachine.ChangeState(_player.PlayerIdleState);
                }
            }
            else
            {
                // _player.PlayerMove(delta);
            }
        }
    }
}
