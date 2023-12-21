using TheLostHope.GameCode.ObjectStateMachine;

namespace TheLostHope.GameCode.Guns.States
{
    public class GunIdleState : GunState
    {
        public GunIdleState(StatefullObject statefullObject, object animKey) : base(statefullObject, animKey)
        {
        }

        public override void Update(float delta)
        {
            if (_gun.CanShoot())
            {
                _stateMachine.ChangeState(_gun.GunShootState);
            }
            else if (_gun.ReloadInputPressed)
            {
                _stateMachine.ChangeState(_gun.GunReloadState);
            }
        }
    }
}
