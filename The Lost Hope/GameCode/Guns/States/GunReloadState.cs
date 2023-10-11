using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Guns.States
{
    public class GunReloadState : GunState
    {
        public GunReloadState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        protected override void AnimationFinished()
        {
            base.AnimationFinished();

            // TODO: Switch to the reload pattern states somehow?
        }
    }
}
