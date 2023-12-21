using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Guns.States
{
    public class GunShootState : GunState
    {
        public GunShootState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void Exit()
        {
            base.Exit();
        }
        protected override void AnimationFrameEventTriggered()
        {
            FireBullet();
        }
        protected override void AnimationFinished()
        {
            base.AnimationFinished();

            _stateMachine.ChangeState(_gun.GunIdleState);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (_gun.ReloadInputPressed)
            {
                _stateMachine.ChangeState(_gun.GunReloadState);
                return;
            }
        }

        private void FireBullet()
        {
            _gun.FireBullet();
            _gun.SetCurrentMagCount(_gun.CurrentMagCount - 1);
        }
    }
}
