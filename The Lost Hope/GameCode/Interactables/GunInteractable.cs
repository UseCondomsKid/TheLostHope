﻿using LDtkTypes;
using TheLostHope.Engine.ContentManagement;
using TheLostHope.GameCode.GameStates;
using TheLostHope.GameCode.Interactables.Core;
using TheLostHope.GameCode.Guns;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using TheLostHopeEngine.EngineCode.Animations;

namespace TheLostHope.GameCode.Interactables
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

                    GameplayManager.Instance.EquipGun(_gunData);
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

            Game1.SpriteBatch.Draw(_animator.SpriteSheetTexture, _interactionZone.Location.ToVector2(), _animator.GetSourceRectangle(), Color.White);
        }
    }
}
