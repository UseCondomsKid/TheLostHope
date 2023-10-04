using LDtkTypes;
using TheLostHope.GameCode.Characters.PlayerCharacter;
using TheLostHope.GameCode.GameStates;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System.Collections.Generic;
using TheLostHope.GameCode.ObjectStateMachine;
using TheLostHope.GameCode.Weapons.States;

namespace TheLostHope.GameCode.Weapons
{
    public class Gun : StatefullObject
    {
        private LDtkGun _gunData;
        private GameplayManager _gameplayManager;
        private Player _player;

        #region States
        public GunIdleState GunIdleState { get; private set; }
        public GunShootState GunShootState { get; private set; }
        public GunReloadState GunReloadState { get; private set; }
        public List<GunReloadPatternState> GunReloadPatternStates { get; private set; }

        // Should be used to get the next reload pattern state
        private GunReloadPatternState _currentReloadPatternState;
        #endregion

        public Gun(Game game, AsepriteFile asepriteFile, LDtkGun gunData, GameplayManager gameplayManager) : base(game, asepriteFile)
        {
            // Get from constructor
            _gunData = gunData;
            _gameplayManager = gameplayManager;
            _player = _gameplayManager.Player;

            // Setup states
            GunIdleState = new GunIdleState(this, "Idle");
            GunShootState = new GunShootState(this, "Shoot");
            GunReloadState = new GunReloadState(this, "Reload");

            // TODO: Figure out how to actually do this line bellow???
            GunReloadPatternStates = new List<GunReloadPatternState>();

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
