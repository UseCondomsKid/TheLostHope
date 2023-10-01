using LDtkTypes;
using TheLostHope.GameCode.GameStates;
using TheLostHope.GameCode.Interactables.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Interactables
{
    public class LevelTransition : TriggerInteractable
    {
        LDtkLevelTransition _levelTransitionData;

        public LevelTransition(Game game, Rectangle interactionZone, LDtkLevelTransition levelTransitionData)
            : base(game, interactionZone)
        {
            _levelTransitionData = levelTransitionData;
        }

        public override void Interact(CollisionTags tag)
        {
            if (tag == CollisionTags.Player)
            {
                // Change level
                GameplayManager.Instance.LoadLevel(_levelTransitionData.LevelToTransitionTo, _levelTransitionData.Id);
            }
        }
    }
}
