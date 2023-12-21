using TheLostHope.GameCode.Characters.PlayerCharacter;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using TheLostHope.GameCode.ObjectStateMachine;
using TheLostHope.GameCode.Guns.States;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Pooling;
using Microsoft.Xna.Framework.Graphics;
using TheLostHopeEngine.EngineCode.Inputs;
using System.Collections.Generic;
using System;

namespace TheLostHope.GameCode.Guns
{
    public class Gun : StatefullObject
    {
        #region Gun Properties
        private WeaponAsset _weaponAsset;
        private Player _player;
        private int _facingDirection;
        // Should be used to get the next reload pattern state
        private int _currentReloadPatternStateIndex;
        private int _currentMagCount;
        private WeaponShootDirection _shootDirection;

        private List<Bullet> _activeBullets;
        private List<Bullet> _bulletsToRemove;

        public WeaponAsset WeaponAsset { get { return _weaponAsset; } }
        public ObjectPool<Bullet> BulletPool { get; private set; }
        public int CurrentMagCount { get { return _currentMagCount; } }
        #endregion

        #region States
        public GunIdleState GunIdleState { get; private set; }
        public GunShootState GunShootState { get; private set; }
        public GunReloadState GunReloadState { get; private set; }
        #endregion

        #region Input
        public bool ShootInputPressed;
        public bool ShootInputHeld;

        public bool ReloadInputPressed;

        public bool UpInputPressed;
        public bool UpInputHeld;

        public bool DownInputPressed;
        public bool DownInputHeld;

        public bool LeftInputPressed;
        public bool LeftInputHeld;

        public bool RightInputPressed;
        public bool RightInputHeld;
        #endregion

        public Gun(Game game, AsepriteFile asepriteFile, WeaponAsset weaponAsset) : base(game, asepriteFile)
        {
            // Get from constructor
            _weaponAsset = weaponAsset;
            _currentReloadPatternStateIndex = -1;

            BulletPool = new ObjectPool<Bullet>
                (
                    objectCreate: () =>
                    {
                        var bullet = new Bullet();
                        return bullet;
                    },
                    objectRelease: (bullet) =>
                    {
                        return;
                    },
                    maxSize: 100
                );
            _activeBullets = new List<Bullet>();
            _bulletsToRemove = new List<Bullet>();

            // Inputs
            InputSystem.Instance.GetAction("PlayerGun_Shoot").OnChange += GunShootInput;
            InputSystem.Instance.GetAction("PlayerGun_Reload").OnChange += GunReloadInput;
            InputSystem.Instance.GetAction("PlayerGun_Up").OnChange += GunUpInput;
            InputSystem.Instance.GetAction("PlayerGun_Down").OnChange += GunDownInput;
            InputSystem.Instance.GetAction("PlayerGun_Left").OnChange += GunLeftInput;
            InputSystem.Instance.GetAction("PlayerGun_Right").OnChange += GunRightInput;

            // Setup states
            GunIdleState = new GunIdleState(this, "Idle");
            GunShootState = new GunShootState(this, "Shoot");
            GunReloadState = new GunReloadState(this, "Reload");
        }

        public void FireBullet()
        {
            Bullet b = BulletPool.GetObject();
            if (b != null)
            {
                b.Fire(_weaponAsset, Position, _facingDirection, _shootDirection);
                b.OnBulletReleased += BulletReleased;
                _activeBullets.Add(b);
            }
        }

        private void BulletReleased(Bullet bullet)
        {
            _bulletsToRemove.Add(bullet);
            bullet.OnBulletReleased -= BulletReleased;
            BulletPool.ReturnObject(bullet);
        }

        public void EquipGun(Player player)
        {
            _player = player;
            _player.PlayerOnRoll += PlayerRoll;

            SpawnObject(_player.Position, Vector2.Zero);
            StateMachine.Initialize(GunIdleState);

            SetCurrentMagCount(_weaponAsset.MagazineSize);
        }
        public void UnequipGun()
        {
            _player.PlayerOnRoll -= PlayerRoll;
        }


        private void PlayerRoll()
        {
            // TODO: Figure this out
        }

        public void SetCurrentMagCount(int magCount)
        {
            if (magCount > _weaponAsset.MagazineSize)
            {
                _currentMagCount = _weaponAsset.MagazineSize;
            }
            else
            {
                _currentMagCount = magCount;
            }

            _player.SetCurrentHealth(_currentMagCount);
        }

        public bool CanShoot()
        {
            return _currentMagCount > _weaponAsset.ReloadFailedMagazineSize &&
                ((ShootInputPressed && !_weaponAsset.HoldToShoot) || (ShootInputHeld && _weaponAsset.HoldToShoot));
        }

        public WeaponReloadPatternAction? ReloadStep()
        {
            if (_currentReloadPatternStateIndex == _weaponAsset.ReloadPattern.Count - 1)
            {
                // Reload in complete
                _currentReloadPatternStateIndex = -1;
                SetCurrentMagCount(_weaponAsset.MagazineSize);
                return null;
            }
            else
            {
                _currentReloadPatternStateIndex++;
                return _weaponAsset.ReloadPattern[_currentReloadPatternStateIndex];
            }
        }
        public void ReloadWaitForInput(WeaponReloadPatternAction? reloadPattern)
        {
            switch (reloadPattern)
            {
                case WeaponReloadPatternAction.Up:
                    if (UpInputPressed)
                    {
                        StateMachine.ChangeState(new GunReloadPatternState(this, "Up"));
                    }
                    break;
                case WeaponReloadPatternAction.Down:
                    if (DownInputPressed)
                    {
                        StateMachine.ChangeState(new GunReloadPatternState(this, "Down"));
                    }
                    break;
                case WeaponReloadPatternAction.Left:
                    if (LeftInputPressed)
                    {
                        StateMachine.ChangeState(new GunReloadPatternState(this, "Left"));
                    }
                    break;
                case WeaponReloadPatternAction.Right:
                    if (RightInputPressed)
                    {
                        StateMachine.ChangeState(new GunReloadPatternState(this, "Right"));
                    }
                    break;
            }
        }


        #region Input
        private void GunRightInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    RightInputPressed = true;
                    break;
                case InputActionPhase.Held:
                    RightInputHeld = true;
                    break;
                case InputActionPhase.Released:
                    RightInputHeld = false;
                    break;
            }
        }
        private void GunLeftInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    LeftInputPressed = true;
                    break;
                case InputActionPhase.Held:
                    LeftInputHeld = true;
                    break;
                case InputActionPhase.Released:
                    LeftInputHeld = false;
                    break;
            }
        }
        private void GunDownInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    DownInputPressed = true;
                    break;
                case InputActionPhase.Held:
                    DownInputHeld = true;
                    break;
                case InputActionPhase.Released:
                    DownInputHeld = false;
                    break;
            }
        }
        private void GunUpInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    UpInputPressed = true;
                    break;
                case InputActionPhase.Held:
                    UpInputHeld = true;
                    break;
                case InputActionPhase.Released:
                    UpInputHeld = false;
                    break;
            }
        }
        private void GunReloadInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    ReloadInputPressed = true;
                    break;
            }
        }
        private void GunShootInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    ShootInputPressed = true;
                    break;
                case InputActionPhase.Held:
                    ShootInputHeld = true;
                    break;
                case InputActionPhase.Released:
                    ShootInputHeld = false;
                    break;
            }
        }
        #endregion


        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position = _player.Position;
            _facingDirection = _player.FacingDirection;

            // Update the state machine
            StateMachine.CurrentState.Update(delta);

            // Update active bullets
            foreach (var bullet in _activeBullets)
            {
                bullet.Update(gameTime);
            }

            Animator.Update(gameTime);

            if (_currentReloadPatternStateIndex == -1)
            {
                if (DownInputHeld && !_player.IsGrounded())
                {
                    _shootDirection = WeaponShootDirection.Down;
                }
                else if (UpInputHeld && !_player.IsTouchingCeiling())
                {
                    _shootDirection = WeaponShootDirection.Up;
                }
                else if (RightInputHeld)
                {
                    _shootDirection = WeaponShootDirection.Right;
                }
                else if (LeftInputHeld)
                {
                    _shootDirection = WeaponShootDirection.Left;
                }
                else
                {
                    _shootDirection = WeaponShootDirection.None;
                }
            }
            
            ShootInputPressed = false;
            ReloadInputPressed = false;
            UpInputPressed = false;
            DownInputPressed = false;
            LeftInputPressed = false;
            RightInputPressed = false;

            // Remove any bullets that need to be removed
            foreach (var bullet in _bulletsToRemove)
            {
                _activeBullets.Remove(bullet);
            }
            _bulletsToRemove.Clear();
        }
        public override void Draw(GameTime gameTime)
        {
            if (Animator.SpriteSheetTexture == null) return;

            SpriteEffects se = SpriteEffects.None;

            switch (_shootDirection)
            {
                case WeaponShootDirection.Up:
                    Rotation = _weaponAsset.UpDirectionRotation;
                    break;
                case WeaponShootDirection.Down:
                    Rotation = _weaponAsset.DownDirectionRotation;
                    break;
                case WeaponShootDirection.Left:
                    se = SpriteEffects.FlipHorizontally;
                    Rotation = _weaponAsset.LeftDirectionRotation;
                    break;
                case WeaponShootDirection.Right:
                    se = SpriteEffects.None;
                    Rotation = _weaponAsset.RightDirectionRotation;
                    break;
                default:
                    se = _facingDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Rotation = _weaponAsset.RightDirectionRotation;
                    break;
            }


            // Draw active bullets
            foreach (var bullet in _activeBullets)
            {
                bullet.Draw(gameTime);
            }

            // Draw using the position, scale, rotation, texture and animation
            Game1.SpriteBatch.Draw(Animator.SpriteSheetTexture, Position, Animator.GetSourceRectangle(),
                Color.White, Rotation,
                _weaponAsset.SpriteHalfSize, Scale, se, 0);
        }
    }
}
