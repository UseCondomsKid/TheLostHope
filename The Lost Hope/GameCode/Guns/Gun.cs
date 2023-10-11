using LDtkTypes;
using TheLostHope.GameCode.Characters.PlayerCharacter;
using TheLostHope.GameCode.GameStates;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System.Collections.Generic;
using TheLostHope.GameCode.ObjectStateMachine;
using TheLostHope.GameCode.Guns.States;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Pooling;
using TheLostHope.GameCode.Guns.Bullets;
using Apos.Input;

namespace TheLostHope.GameCode.Guns
{
    public class Gun : StatefullObject
    {
        private WeaponAsset _weaponAsset;
        private GameplayManager _gameplayManager;
        private Player _player;

        public WeaponAsset WeaponAsset { get { return _weaponAsset; } }
        public ObjectPool<Bullet> BulletPool { get; private set; }

        #region States
        public GunIdleState GunIdleState { get; private set; }
        public GunShootState GunShootState { get; private set; }
        public GunReloadState GunReloadState { get; private set; }
        public List<GunReloadPatternState> GunReloadPatternStates { get; private set; }

        // Should be used to get the next reload pattern state
        private GunReloadPatternState _currentReloadPatternState;
        #endregion

        #region Inputs
        public ICondition ShootInput { get; private set; }
        public ICondition ReloadInput { get; private set; }
        public ICondition UpReloadInput { get; private set; }
        public ICondition DownReloadInput { get; private set; }
        public ICondition LeftReloadInput { get; private set; }
        public ICondition RightReloadInput { get; private set; }
        #endregion

        public Gun(Game game, AsepriteFile asepriteFile, WeaponAsset weaponAsset, GameplayManager gameplayManager) : base(game, asepriteFile)
        {
            // Get from constructor
            _weaponAsset = weaponAsset;
            _gameplayManager = gameplayManager;
            _player = _gameplayManager.Player;

            // Setup states
            GunIdleState = new GunIdleState(this, "Idle");
            GunShootState = new GunShootState(this, "Shoot");
            GunReloadState = new GunReloadState(this, "Reload");

            // Set the reload pattern states
            GunReloadPatternStates = new List<GunReloadPatternState>();
            foreach (var reloadPattern in _weaponAsset.ReloadPattern)
            {
                GunReloadPatternStates.Add(new GunReloadPatternState(this, reloadPattern.ToString()));
            }

            _currentReloadPatternState = null;
        }

        public void SpawnGun()
        {
            // SpawnObject(gunSpawnPos, (0, 0))
            StateMachine.Initialize(GunIdleState);
        }
        public override void Update(GameTime gameTime)
        {
        }
        public override void Draw(GameTime gameTime)
        {
            // TODO: Draw gun using animator
        }
    }
}
