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

            if (_isAnimationFinished)
            {
                if (_player.IsGrounded())
                {
                    _stateMachine.ChangeState(_player.PlayerIdleState);
                }
                else
                {
                    _stateMachine.ChangeState(_player.PlayerJumpState);
                }

            }
            else
            {
                _player.MoveX(delta, _player.FacingDirection, _player.PlayerData.RollVelocity,
                    _player.PlayerData.Acceleration, _player.PlayerData.Deacceleration, 1.2f);
            }
        }
    }
}
