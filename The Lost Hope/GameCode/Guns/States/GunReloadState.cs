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

            _pattern = _gun.ReloadStep();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _gun.ReloadWaitForInput(_pattern);
        }
    }
}
