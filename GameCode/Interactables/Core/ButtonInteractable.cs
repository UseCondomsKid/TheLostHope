using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Interactables.Core
{
    public abstract class ButtonInteractable : Interactable
    {
        protected ButtonInteractable(Game game, Rectangle interactionZone) : base(game, interactionZone)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            OnEnterInteractionZone += CharacterEnteredInteractionZone;
            OnExitInteractionZone += CharacterExitedInteractionZone;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                OnEnterInteractionZone -= CharacterEnteredInteractionZone;
                OnExitInteractionZone -= CharacterExitedInteractionZone;
            }
        }

        protected abstract void CharacterEnteredInteractionZone(CollisionTags tag);
        protected abstract void CharacterExitedInteractionZone(CollisionTags tag);
    }
}
