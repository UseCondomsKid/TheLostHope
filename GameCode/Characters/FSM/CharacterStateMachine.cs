using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Characters.FSM
{
    public class CharacterStateMachine
    {
        public CharacterState CurrentState { get; private set; }
        public void Initialize(CharacterState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(CharacterState newState)
        {
            if (newState == CurrentState) return;

            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
