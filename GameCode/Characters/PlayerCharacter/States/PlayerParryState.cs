using LostHope.GameCode.Characters.FSM;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerParryState : PlayerState
    {
        public PlayerParryState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (_isAnimationFinished)
            {
                if (!_player.IsGrounded)
                {
                    _stateMachine.ChangeState(_player.PlayerJumpState);
                }
                else
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
            }
        }
    }
}
