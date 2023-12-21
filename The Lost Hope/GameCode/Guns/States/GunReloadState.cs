using System;
using TheLostHope.GameCode.ObjectStateMachine;
using TheLostHopeEngine.EngineCode.Assets;

namespace TheLostHope.GameCode.Guns.States
{
    public class GunReloadState : GunState
    {
        private WeaponReloadPatternAction? _pattern;

        public GunReloadState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _gun.OnCancelReload += CancelReload;
            _gun.IsReloading = true;
            _pattern = _gun.ReloadStep();
        }
        public override void Exit()
        {
            base.Exit();

            _gun.OnCancelReload -= CancelReload;
        }

        private void CancelReload()
        {
            _stateMachine.ChangeState(_gun.GunIdleState);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _gun.ReloadWaitForInput(_pattern);
        }
    }
}
