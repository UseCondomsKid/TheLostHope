﻿using TheLostHope.GameCode.Characters.FSM;
using TheLostHope.GameCode.Guns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Guns.States
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
