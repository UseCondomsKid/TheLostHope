using Humper;
using LostHope.Engine;
using LostHope.Engine.Animations;
using LostHope.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostHope.GameCode.Weapons
{
    public class Gun
    {
        public int CurrentClip { get { return _currentClip; } }

        public event Action<float> OnShoot;
        public event Action<int> OnClipChanged;

        protected World _physicsWorld;
        protected Animator _animator;

        private Animation _shootAnimation;
        private Animation _reloadAnimation;

        private float _shootTimer;
        private int _currentClip;
        private bool _reloading;

        // TODO: Figure out offset for position
        private Vector2 _position => Globals.Player.Position + new Vector2(
            Globals.Player.FacingDirection == -1 ? -_animator.AnimationCanvasSize.X : Globals.Player.Body.Width,
            Globals.Player.Body.Height / 2f - _animator.AnimationCanvasSize.Y / 2f);

        private Vector2 _shootPosition => Globals.Player.Position + new Vector2(
            Globals.Player.FacingDirection == -1 ? -_animator.AnimationCanvasSize.X :
            Globals.Player.Body.Width + _animator.AnimationCanvasSize.X,
            Globals.Player.Body.Height / 2f + _animator.AnimationCanvasSize.Y / 3f);

        //    public Gun(GraphicsDevice graphicsDevice, LDtkGun data, World physicsWorld)
        //    {
        //        _data = data;
        //        _physicsWorld = physicsWorld;

        //        _shootTimer = 0f;
        //        _currentClip = _data.ClipSize;
        //        _reloading = false;

        //        var asepriteFile = Globals.Content.Load<AsepriteFile>(_data.AsepriteFileName);

        //        _animator = new Animator(asepriteFile, graphicsDevice);
        //        _shootAnimation = _animator.Animations["Shoot"];
        //        _reloadAnimation = _animator.Animations["Reload"];
        //        _animator.SetActiveAnimation("Idle");

        //        _shootAnimation.OnAnimationFinished += ShootAnimationFinished;
        //        _reloadAnimation.OnAnimationFinished += ReloadAnimationFinished;
        //    }

        //    private void ReloadAnimationFinished()
        //    {
        //        _reloading = false;
        //        _currentClip = _data.ClipSize;
        //        OnClipChanged?.Invoke(_currentClip);
        //        _animator.SetActiveAnimation("Idle");
        //    }

        //    private void ShootAnimationFinished()
        //    {
        //        _animator.SetActiveAnimation("Idle");
        //    }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);

            if (_reloading) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _shootTimer -= delta;
        }

        //    public void HandleGunShoot()
        //    {
        //        if (_data.Auto)
        //        {
        //            if (InputManager.IsMouseDown(MouseButton.Left) && _shootTimer <= 0)
        //            {
        //                Shoot();
        //            }
        //        }
        //        else
        //        {
        //            if (InputManager.MousePressed(MouseButton.Left) && _shootTimer <= 0)
        //            {
        //                Shoot();
        //            }
        //        }
        //    }
        //    public void HandleGunReload()
        //    {
        //        if (InputManager.KeyPressed(Keys.R))
        //        {
        //            Reload();
        //        }
        //    }

        //    // TODO: Implement this
        //    private void Shoot()
        //    {
        //        if (_currentClip <= 0)
        //        {
        //            _currentClip = 0;
        //        }
        //        else
        //        {
        //            _animator.SetActiveAnimation("Shoot");
        //            _shootTimer = _data.TimeBetweenShots;
        //            _currentClip--;

        //            OnClipChanged?.Invoke(_currentClip);
        //            OnShoot?.Invoke(_data.KnockbackForce);

        //            Rectangle shootRect = new Rectangle( Globals.Player.FacingDirection == 1 ?
        //                _shootPosition.ToPoint() : new Point((int)(_shootPosition.X - _data.Range),
        //                (int)_shootPosition.Y), new Point((int)_data.Range, _data.BulletWidth));

        //            var enemies = new List<Enemy>();
        //            foreach (var e in Globals.Enemies)
        //            {
        //                if (e.Enabled && e.Visible)
        //                {
        //                    enemies.Add(e);
        //                }
        //            }
        //            enemies.Sort(EnemySortFunction);

        //            foreach (var enemy in enemies)
        //            {
        //                var enemyRect = new Rectangle((int)enemy.Body.X, (int)enemy.Body.Y, (int)enemy.Body.Width,
        //                    (int)enemy.Body.Height);

        //                if (shootRect.Intersects(enemyRect))
        //                {
        //                    enemy.TakeDamage(_data.Damage);

        //                    if (!_data.CanPenetrate) return;
        //                }
        //            }
        //        }
        //    }

        //    private int EnemySortFunction(Enemy x, Enemy y)
        //    {
        //        return (int)Vector2.Distance(Globals.Player.Position, x.Position) -
        //            (int)Vector2.Distance(Globals.Player.Position, y.Position);
        //    }



        //    // TODO: Implement this
        //    private void Reload()
        //    {
        //        _animator.SetActiveAnimation("Reload");
        //        _reloading = true;
        //    }

        public void Draw()
        {
            var spriteBatch = Globals.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Globals.GameCamera.Transform);

            spriteBatch.Draw(_animator.SpriteSheetTexture, _position,
                _animator.GetSourceRectangle(), Color.White, 0f, Vector2.Zero, 1f,
                Globals.Player.FacingDirection == 1 ? SpriteEffects.None :
                SpriteEffects.FlipHorizontally, 0f);

            //TODO: Draw Trail when firing
            //if (_shootTimer - _shootTimer / 1.5f > 0)
            //    DrawLineBetween(spriteBatch, _shootPosition, new Vector2(_shootPosition.X + (Globals.Player.FacingDirection * _data.Range),
            //            _shootPosition.Y), _data.BulletWidth, Color.Red);

            spriteBatch.End();
        }

        //    private void DrawLineBetween(
        //SpriteBatch spriteBatch,
        //Vector2 startPos,
        //Vector2 endPos,
        //int thickness,
        //Color color)
        //    {
        //        // Create a texture as wide as the distance between two points and as high as
        //        // the desired thickness of the line.
        //        var distance = (int)Vector2.Distance(startPos, endPos);
        //        var texture = new Texture2D(spriteBatch.GraphicsDevice, distance, thickness);

        //        // Fill texture with given color.
        //        var data = new Color[distance * thickness];
        //        for (int i = 0; i < data.Length; i++)
        //        {
        //            data[i] = color;
        //        }
        //        texture.SetData(data);

        //        // Rotate about the beginning middle of the line.
        //        var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
        //        var origin = new Vector2(0, thickness / 2);

        //        spriteBatch.Draw(
        //            texture,
        //            startPos,
        //            null,
        //            Color.White,
        //            rotation,
        //            origin,
        //            1.0f,
        //            SpriteEffects.None,
        //            0f);
        //    }
    }
}
