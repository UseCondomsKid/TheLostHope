using Humper;
using LDtkTypes;
using LostHope.Engine.Animations;
using LostHope.Engine.Input;
using LostHope.GameCode.Characters.Enemies;
using LostHope.GameCode.Weapons.Bullets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostHope.GameCode.Weapons
{
    public class Gun : DrawableGameComponent
    {
        public int CurrentClip { get { return _currentClip; } }

        private LDtkGun _data;

        private bool _equipped;

        public event Action<float> OnShoot;
        public event Action<int> OnClipChanged;

        protected World _physicsWorld;
        protected Animator _animator;

        private Animation _idleAnimation;
        private Animation _shootAnimation;
        private Animation _reloadAnimation;

        private float _shootTimer;
        private int _currentClip;
        private bool _reloading;

        private List<Bullet> _bulletsActive;

        // TODO: Figure out offset for position
        //private Vector2 _position => Globals.Player.Position + new Vector2(
        //    Globals.Player.FacingDirection == -1 ? -_animator.AnimationCanvasSize.X : Globals.Player.Body.Width,
        //    Globals.Player.Body.Height / 2f - _animator.AnimationCanvasSize.Y / 2f);

        //private Vector2 _shootPosition => Globals.Player.Position + new Vector2(
        //    Globals.Player.FacingDirection == -1 ? -_animator.AnimationCanvasSize.X :
        //    Globals.Player.Body.Width + _animator.AnimationCanvasSize.X,
        //    Globals.Player.Body.Height / 2f + _animator.AnimationCanvasSize.Y / 3f);

        public Gun(Game game, LDtkGun data, World physicsWorld, AsepriteFile asepriteFile, bool equipped = false) : base(game)
        {
            _data = data;
            _physicsWorld = physicsWorld;
            _equipped = equipped;

            _shootTimer = 0f;
            _currentClip = _data.ClipSize;
            _reloading = false;

            _animator = new Animator(asepriteFile, GraphicsDevice);
            _shootAnimation = _animator.Animations["Shoot"];
            _reloadAnimation = _animator.Animations["Reload"];
            _animator.SetActiveAnimation("Idle");

            _shootAnimation.OnAnimationFinished += ShootAnimationFinished;
            _reloadAnimation.OnAnimationFinished += ReloadAnimationFinished;

            _bulletsActive = new List<Bullet>();
        }

        private void ReloadAnimationFinished()
        {
            _reloading = false;
            _currentClip = _data.ClipSize;
            OnClipChanged?.Invoke(_currentClip);
            _animator.SetActiveAnimation("Idle");
        }

        private void ShootAnimationFinished()
        {
            _animator.SetActiveAnimation("Idle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _animator.Update(gameTime);

            if (_reloading || !_equipped) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _shootTimer -= delta;
        }

        public void HandleGunShoot()
        {
            switch(_data.FireType)
            {
                case GunFireType.Auto:
                    if (InputManager.IsMouseDown(MouseButton.Left) && _shootTimer <= 0)
                    {
                        Shoot();
                    }
                    break;
                case GunFireType.Single:
                    if (InputManager.MousePressed(MouseButton.Left) && _shootTimer <= 0)
                    {
                        Shoot();
                    }
                    break;
                default:
                    break;
            }
        }
        public void HandleGunReload()
        {
            if (InputManager.KeyPressed(Keys.R))
            {
                Reload();
            }
        }

        private void Reload()
        {
            _animator.SetActiveAnimation("Reload");
            _reloading = true;
        }
        private void Shoot()
        {
            if (_currentClip <= 0)
            {
                _currentClip = 0;
            }
            else
            {
                _animator.SetActiveAnimation("Shoot");
                _shootTimer = _data.TimeBetweenShots;
                _currentClip--;

                OnClipChanged?.Invoke(_currentClip);
                OnShoot?.Invoke(_data.KnockbackForce);

                // Spawn Bullet
                //switch(_data.BulletType)
                //{
                //    case GunBulletType.Hitscan:
                //        _bulletsActive.Add(new HitscanBullet(Game, _shootPosition, 
                //            new Vector2(_shootPosition.X + (_data.Range * Globals.Player.FacingDirection), _shootPosition.Y), _data.Range,
                //            _data.Damage, _data.CanPenetrate));
                //        break;
                //}

                //Rectangle shootRect = new Rectangle(Globals.Player.FacingDirection == 1 ?
                //    _shootPosition.ToPoint() : new Point((int)(_shootPosition.X - _data.Range),
                //    (int)_shootPosition.Y), new Point((int)_data.Range, 1));

                //var enemies = new List<Enemy>();
                //foreach (var e in Globals.Enemies)
                //{
                //    if (e.Enabled && e.Visible)
                //    {
                //        enemies.Add(e);
                //    }
                //}
                //enemies.Sort(EnemySortFunction);

                //foreach (var enemy in enemies)
                //{
                //    var enemyRect = new Rectangle((int)enemy.Body.X, (int)enemy.Body.Y, (int)enemy.Body.Width,
                //        (int)enemy.Body.Height);

                //    if (shootRect.Intersects(enemyRect))
                //    {
                //        enemy.TakeDamage(_data.Damage);

                //        if (!_data.CanPenetrate) return;
                //    }
                //}
            }
        }

        //private int EnemySortFunction(Enemy x, Enemy y)
        //{
        //    return (int)Vector2.Distance(Globals.Player.Position, x.Position) -
        //        (int)Vector2.Distance(Globals.Player.Position, y.Position);
        //}



        public override void Draw(GameTime gameTime)
        {
            //Globals.SpriteBatch.Draw(_animator.SpriteSheetTexture, _position,
            //    _animator.GetSourceRectangle(), Color.White, 0f, Vector2.Zero, 1f,
            //    Globals.Player.FacingDirection == 1 ? SpriteEffects.None :
            //    SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
