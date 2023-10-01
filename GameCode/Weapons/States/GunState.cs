using LostHope.GameCode.Characters.FSM;
using LostHope.GameCode.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Weapons.States
{
    public class GunState : ObjectState
    {
        protected Gun _gun;

        public GunState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
            _gun = (Gun)statefullObject;
        }
    }
}
