using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Weapons.States
{
    public class GunIdleState : GunState
    {
        public GunIdleState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Update(float delta)
        {
            // TODO: Check if shoot input pressed, if so switch to shoot state if we have bullets left in mag.
        }
    }
}
