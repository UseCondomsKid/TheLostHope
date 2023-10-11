using TheLostHope.GameCode.GameStates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.GameStates.SubStates;

namespace TheLostHope.GameCode.Interactables.Core
{
    public abstract class Interactable : DrawableGameComponent
    {
        private class InteractableAgainstCharacter
        {
            public bool Inside { get; set; }
            public LevelState.CharacterBox CharacterBox { get; private set; }

            public InteractableAgainstCharacter(LevelState.CharacterBox characterBox)
            {
                Inside = false;
                CharacterBox = characterBox;
            }
        }

        protected Rectangle _interactionZone { get; set; }

        public event Action<CollisionTags> OnEnterInteractionZone;
        // public event Action<CollisionTags> OnStayInInteractionZone;
        public event Action<CollisionTags> OnExitInteractionZone;

        private List<InteractableAgainstCharacter> _characterBoxes;

        public Interactable(Game game, Rectangle interactionZone) : base(game)
        {
            _interactionZone = interactionZone;
            _characterBoxes = new List<InteractableAgainstCharacter>();
            foreach (var characterBox in GameplayManager.Instance.CurrentLevelState.CharacterBoxes)
            {
                _characterBoxes.Add(new InteractableAgainstCharacter(characterBox));
            }
        }

        public abstract void Interact(CollisionTags tag);

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            foreach (var character in _characterBoxes)
            {
                if (_interactionZone.Intersects(character.CharacterBox.Box.BoundsRect))
                {
                    if (!character.Inside)
                    {
                        OnEnterInteractionZone?.Invoke(character.CharacterBox.Tag);
                    }
                    else
                    {
                        // OnStayInInteractionZone?.Invoke(character.CharacterBox.Tag);
                    }

                    character.Inside = true;
                }
                else
                {
                    if (character.Inside)
                    {
                        OnExitInteractionZone?.Invoke(character.CharacterBox.Tag);
                    }
                    character.Inside = false;
                }
            }
        }
    }
}
