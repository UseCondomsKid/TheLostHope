using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Interactables
{
    public enum InteractionType
    {
        None,
        Trigger,
        ButtonPress,
    }

    public class Interactable : DrawableGameComponent
    {
        public Rectangle InteractionZone { get; set; }

        public event Action OnEnterInteractionZone;
        public event Action OnInteract;
        public event Action OnExitInteractionZone;

        protected InteractionType _interactionType;

        public Interactable(Game game, Rectangle interactionZone, InteractionType interactionType) : base(game)
        {
            InteractionZone = interactionZone;
            _interactionType = interactionType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
