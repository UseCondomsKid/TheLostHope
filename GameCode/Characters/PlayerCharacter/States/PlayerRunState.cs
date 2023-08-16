using LostHope.GameCode.Characters.FSM;
using System;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerRunState : PlayerGroundedState
    {
        public PlayerRunState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Exit()
        {
            base.Exit();

            _player.Animator.AnimationSpeedMultiplier = 1f;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _player.Animator.AnimationSpeedMultiplier = Math.Min(_player.Velocity.X, 1f);
        }
    }
}
