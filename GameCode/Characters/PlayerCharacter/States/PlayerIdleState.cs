using LostHope.GameCode.Characters.FSM;

namespace LostHope.GameCode.Characters.PlayerCharacter.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Character character, object animKey) : base(character, animKey)
        {
        }
    }
}
