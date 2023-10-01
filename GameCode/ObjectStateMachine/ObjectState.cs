using TheLostHope.Engine.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.ObjectStateMachine
{
    public class ObjectState
    {
        // A reference to the character
        protected StatefullObject _statefullObject;
        // A reference to the state machine
        protected ObjectStateMachine _stateMachine;

        // Is the animation associated with this state finished
        protected bool _isAnimationFinished;
        protected Animation _animation;
        // The key for the animation associated with this state
        private object _animKey;

        public ObjectState(StatefullObject statefullObject, object animKey)
        {
            this._statefullObject = statefullObject;
            _stateMachine = _statefullObject.StateMachine;
            this._animKey = animKey;
        }

        // Called when we enter this state
        public virtual void Enter()
        {
            // Debug.WriteLine("Entering character state: " + _animKey.ToString());

            _animation = _statefullObject.Animator.SetActiveAnimation(_animKey);
            _animation.OnAnimationFrameEvent += AnimationFrameEventTriggered;
            _animation.OnAnimationFinished += AnimationFinished;

            _isAnimationFinished = false;
        }
        // Called when we exit this state
        public virtual void Exit()
        {
            // Debug.WriteLine("Exiting character state: " + _animKey.ToString());

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
