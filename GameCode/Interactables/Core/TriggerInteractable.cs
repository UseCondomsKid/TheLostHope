using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Interactables.Core
{
    public abstract class TriggerInteractable : Interactable
    {
        protected TriggerInteractable(Game game, Rectangle interactionZone) : base(game, interactionZone)
        {
        }
        public override void Initialize()
        {
            base.Initialize();

            OnEnterInteractionZone += Interact;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                OnEnterInteractionZone -= Interact;
            }
        }
    }
}
