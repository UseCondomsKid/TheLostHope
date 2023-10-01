using LDtkTypes;
using LostHope.Engine.Animations;
using LostHope.Engine.ContentLoading;
using LostHope.GameCode.GameStates;
using LostHope.GameCode.Interactables.Core;
using LostHope.GameCode.Weapons;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace LostHope.GameCode.Interactables
{
    public class GunInteractable : TriggerInteractable
    {
        private LDtkGun _gunData;
        private Animator _animator;

        public GunInteractable(Game game, Rectangle interactionZone, LDtkGun gunData) : base(game, interactionZone)
        {
            _gunData = gunData;
        }

        public override void Initialize()
        {
            base.Initialize();

            ContentLoader.LoadAsepriteFile(_gunData.Name, _gunData.AsepriteFileName);
            _animator = new Animator(ContentLoader.GetAsepriteFile(_gunData.Name), GraphicsDevice);
            _animator.SetActiveAnimation("Idle");
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _animator = null;
        }

        public override void Interact(CollisionTags tag)
        {
            if (tag == CollisionTags.Player)
            {
                if (GameplayManager.Instance.AddGun(_gunData))
                {
                    Debug.WriteLine("Gun Added to inventory");
                }

                Enabled = false;
                Visible = false;
                UnloadContent();
                Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            _animator.Update(gameTime);

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Visible) return;

            Globals.SpriteBatch.Draw(_animator.SpriteSheetTexture, _interactionZone.Location.ToVector2(), _animator.GetSourceRectangle(), Color.White);
        }
    }
}
