using TheLostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework.Input;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerLandState : PlayerState
    {
        public PlayerLandState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (_isAnimationFinished)
            {
                if (_player.IsMoving())
                {
                    _stateMachine.ChangeState(_player.PlayerRunState);
                }
                else
                {
                    _stateMachine.ChangeState(_player.PlayerIdleState);
                }
            }
            else if (_player.PlayerLastGroundedTime > 0f && _player.PlayerLastJumpTime > 0f && !_player.PlayerJumping &&
                !_player.IsEquippedGunReloading())
            {
                _stateMachine.ChangeState(_player.PlayerJumpState);
            }
        }
    }
}
