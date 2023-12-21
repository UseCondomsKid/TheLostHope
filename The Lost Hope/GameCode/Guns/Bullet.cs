using Humper;
using Humper.Responses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheLostHope.Engine.ContentManagement;
using TheLostHope.GameCode.GameStates;
using TheLostHopeEngine.EngineCode.Assets;

namespace TheLostHope.GameCode.Guns
{
    public class Bullet
    {
        private WeaponAsset _weaponAsset;
        private bool _enabled;

        private int _currentPenetration;
        private int _currentBounciness;

        private IBox _groundJustHit;
        private IBox _enemyJustHit;

        private float _timer;
        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private Vector2 _currentVelocity;
        private float _rotation;

        private IBox _box;

        public event Action<Bullet> OnBulletReleased;

        public Bullet()
        {
            _enabled = false;
        }

        private void ResetVelocity(Vector2 direction)
        {
            _currentVelocity = direction * _weaponAsset.Speed;
        }

        // Called to instantiate the bullet
        public void Fire(WeaponAsset weaponAsset, Vector2 gunPos, int facingDirection, WeaponShootDirection shootDir)
        {
            _weaponAsset = weaponAsset;
            Vector2 firePos = Vector2.Zero;
            switch(shootDir)
            {
                case WeaponShootDirection.Up:
                    ResetVelocity(new Vector2(0, -1));
                    firePos = _weaponAsset.UpFirePosition;
                    _rotation = _weaponAsset.UpDirectionRotation;
                    break;
                case WeaponShootDirection.Down:
                    ResetVelocity(new Vector2(0, 1));
                    firePos = _weaponAsset.DownFirePosition;
                    _rotation = _weaponAsset.DownDirectionRotation;
                    break;
                case WeaponShootDirection.Left:
                    ResetVelocity(new Vector2(-1, 0));
                    firePos = _weaponAsset.LeftFirePosition;
                    _rotation = _weaponAsset.LeftDirectionRotation;
                    break;
                case WeaponShootDirection.Right:
                    ResetVelocity(new Vector2(1, 0));
                    firePos = _weaponAsset.RightFirePosition;
                    _rotation = _weaponAsset.RightDirectionRotation;
                    break;
                default:
                    ResetVelocity(new Vector2(facingDirection, 0));
                    firePos = facingDirection == -1 ? _weaponAsset.LeftFirePosition : _weaponAsset.RightFirePosition;
                    _rotation = _weaponAsset.RightDirectionRotation;
                    break;
            }
            Vector2 fireWorldPos = gunPos + firePos;

            _startPosition = fireWorldPos;
            _currentPosition = _startPosition;
            _timer = 0.0f;
            _box = GameplayManager.Instance.PhysicsWorld
                .Create(_currentPosition.X, _currentPosition.Y, _weaponAsset.Size.X, _weaponAsset.Size.Y)
                .AddTags(CollisionTags.Bullet);

            _currentPenetration = _weaponAsset.Penetration;
            _currentBounciness = _weaponAsset.Bouciness;

            _groundJustHit = null;
            _enemyJustHit = null;

            _enabled = true;
        }

        // Used to release the bullet from the pool
        public void Release()
        {
            _enabled = false;
            GameplayManager.Instance.PhysicsWorld.Remove(_box);
            OnBulletReleased?.Invoke(this);
        }

        public void Update(GameTime gameTime)
        {
            if (!_enabled) return;
            float distanceFromGun = Vector2.Distance(_currentPosition, _startPosition);
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += delta;

            if (_timer >= _weaponAsset.MaxLifeTime || distanceFromGun >= _weaponAsset.MaxRange)
            {
                Release();
                return;
            }

            _currentVelocity.Y += _weaponAsset.GravityScale;
            _rotation = (float)Math.Atan2(_currentVelocity.Y, _currentVelocity.X);
            var movement = _box.Move(_box.X + delta * _currentVelocity.X,
            _box.Y + delta * _currentVelocity.Y, (collision) =>
            {
                //if (collision.Other.HasTag(CollisionTags.Damageable))
                //{
                //    return CollisionResponses.None;
                //}

                return CollisionResponses.None;
            });

            _currentPosition = new Vector2(_box.X, _box.Y);
            CheckAndResolveCollisions(movement);
        }

        public void Draw(GameTime gameTime)
        {
            if (!_enabled) return;

            Game1.SpriteBatch.Draw(ContentLoader.GetTexture("button"), _currentPosition, new Rectangle(_currentPosition.ToPoint(),
                _weaponAsset.Size.ToPoint()), Color.White, _rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }

        private void CheckAndResolveCollisions(IMovement movement)
        {
            foreach (var hit in movement.Hits)
            {
                if (hit.Box.HasTag(CollisionTags.Damageable))
                {
                    if(_enemyJustHit != hit.Box)
                    {
                        // TODO: Deal Damage
                    }

                    if (_currentPenetration < 0)
                    {
                        Release();
                        break;
                    }
                    else if (_enemyJustHit != hit.Box)
                    {
                        _currentPenetration--;
                    }

                    _enemyJustHit = hit.Box;
                }
                else if (hit.Box.HasTag(CollisionTags.Ground))
                {
                    if (_currentBounciness < 0)
                    {
                        Release();
                        break;
                    }
                    else if (_groundJustHit != hit.Box)
                    {
                        ResetVelocity(new Vector2(hit.Normal.X, hit.Normal.Y));
                        _currentBounciness--;
                    }

                    _groundJustHit = hit.Box;
                }
            }
        }
    }
}
