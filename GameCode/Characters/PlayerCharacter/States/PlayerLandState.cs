﻿using LostHope.Engine.Input;
using LostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework.Input;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerLandState : PlayerState
    {
        public PlayerLandState(Character character, object animKey) : base(character, animKey)
        {
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
        }
    }
}
