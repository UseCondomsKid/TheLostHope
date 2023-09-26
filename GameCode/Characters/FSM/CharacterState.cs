using LostHope.Engine.Animations;
using System;
using System.Diagnostics;

namespace LostHope.GameCode.Characters.FSM
{
    // Represents a state that the character can be in.
    public class CharacterState
    {
        // A reference to the character
        protected Character _character;
        // A reference to the state machine
        protected CharacterStateMachine _stateMachine;

        // Is the animation associated with this state finished
        protected bool _isAnimationFinished;
        protected Animation _animation;
        // The key for the animation associated with this state
        private object _animKey;

        public CharacterState(Character character, object animKey)
        {
            this._character = character;
            _stateMachine = character.StateMachine;
            this._animKey = animKey;
        }

        // Called when we enter this state
        public virtual void Enter()
        {
            Debug.WriteLine("Entering character state: " + _animKey.ToString());

            _animation = _character.Animator.SetActiveAnimation(_animKey);
            _animation.OnAnimationFrameEvent += AnimationFrameEventTriggered;
            _animation.OnAnimationFinished += AnimationFinished;

            _isAnimationFinished = false;
        }
        // Called when we exit this state
        public virtual void Exit()
        {
            Debug.WriteLine("Exiting character state: " + _animKey.ToString());

            _animation.OnAnimationFinished -= AnimationFinished;
            _animation.OnAnimationFrameEvent -= AnimationFrameEventTriggered;
        }

        protected virtual void AnimationFrameEventTriggered()
        {
        }
        protected virtual void AnimationFinished()
        {
            _isAnimationFinished = true;
        }
        public virtual void Update(float delta)
        {
        }
    }
}
