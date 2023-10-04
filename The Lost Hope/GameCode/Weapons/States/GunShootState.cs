using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Weapons.States
{
    public class GunShootState : GunState
    {
        public GunShootState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void Exit()
        {
            base.Exit();
        }
        protected override void AnimationFrameEventTriggered()
        {
            FireBullet();
        }
        protected override void AnimationFinished()
        {
            base.AnimationFinished();

            // Return to idle state?
        }

        private void FireBullet()
        {
            // TODO: Fire the bullet
        }
    }
}
