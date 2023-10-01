using LostHope.Engine.Animations;
using System;
using System.Diagnostics;
using TheLostHope.GameCode.ObjectStateMachine;

namespace LostHope.GameCode.Characters.FSM
{
    // Represents a state that the character can be in.
    public class CharacterState : ObjectState
    {
        // A reference to the character
        protected Character _character;

        public CharacterState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
            _character = (Character)statefullObject;
        }
    }
}
