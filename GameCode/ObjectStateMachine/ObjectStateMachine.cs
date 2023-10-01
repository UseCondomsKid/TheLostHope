using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.ObjectStateMachine
{
    public class ObjectStateMachine
    {
        public ObjectState CurrentState { get; private set; }
        public void Initialize(ObjectState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(ObjectState newState)
        {
            if (newState == CurrentState) return;

            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
