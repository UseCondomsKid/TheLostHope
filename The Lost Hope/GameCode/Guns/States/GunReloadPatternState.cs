using TheLostHope.GameCode.ObjectStateMachine;
using TheLostHopeEngine.EngineCode.Assets;

namespace TheLostHope.GameCode.Guns.States
{
    public class GunReloadPatternState : GunState
    {
        private WeaponReloadPatternAction? _pattern;

        public GunReloadPatternState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _gun.OnCancelReload += CancelReload;
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

        protected override void AnimationFinished()
        {
            base.AnimationFinished();

            if (_pattern == null)
            {
                _gun.IsReloading = false;
                if (_gun.ShootInputPressed)
                {
                    _stateMachine.ChangeState(_gun.GunShootState);
                }
                else
                {
                    _stateMachine.ChangeState(_gun.GunIdleState);
                }
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _gun.ReloadWaitForInput(_pattern);
        }
    }
}
