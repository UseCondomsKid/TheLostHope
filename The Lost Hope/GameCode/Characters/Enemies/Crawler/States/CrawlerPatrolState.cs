using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.Characters.FSM;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Characters.Enemies.Crawler.States
{
    public class CrawlerPatrolState : CharacterState
    {
        private int _moveDirection;

        public CrawlerPatrolState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _moveDirection = -1;
        }

        public override void Update(float delta)
        {
            if (_character.IsTouchingWall())
            {
                _moveDirection *= -1;
            }

            base.Update(delta);
            _character.MoveX(delta, _moveDirection, 15f, 10f, 10f, 1.2f);

            if (!_character.IsGrounded())
            {
                _character.MoveY(delta, 1, 100, 10, 10, 1.2f);
            }
        }
    }
}
