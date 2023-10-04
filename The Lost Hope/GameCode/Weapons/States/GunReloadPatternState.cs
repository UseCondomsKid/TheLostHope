using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Weapons.States
{
    public class GunReloadPatternState : GunState
    {
        public GunReloadPatternState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        protected override void AnimationFinished()
        {
            base.AnimationFinished();

            // TODO: Switch to next reload pattern state or return to idle (or some other state) if pattern is done
        }
    }
}
