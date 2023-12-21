using TheLostHope.GameCode.Characters.FSM;

namespace TheLostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Character character, object animKey) : base(character, animKey)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }
    }
}
