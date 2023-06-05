using LostHope.GameCode.Characters.FSM;
using Humper;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using LostHope.GameCode.Weapons;
using LostHope.GameCode.Characters.PlayerCharacter.States;
using Humper.Responses;

namespace LostHope.GameCode.Characters.PlayerCharacter
{
    public class Player : Character
    {
        public Gun HeldGun { get; set; }

        // ----- Player States ------
        public PlayerIdleState PlayerIdleState { get; private set; }
        public PlayerRunState PlayerRunState { get; private set; }
        public PlayerJumpState PlayerJumpState { get; private set; }
        public PlayerLandState PlayerLandState { get; private set; }
        public PlayerRollState PlayerRollState { get; private set; }
        public PlayerParryState PlayerParryState { get; private set; }
        // --------------------------

        public Player(Game game, World physicsWorld, AsepriteFile asepriteFile) : base(game, physicsWorld, asepriteFile)
        {
            // Initilize the states
            PlayerIdleState = new PlayerIdleState(this, "Idle");
            PlayerRunState = new PlayerRunState(this, "Run");
            PlayerJumpState = new PlayerJumpState(this, "Jump");
            PlayerLandState = new PlayerLandState(this, "Land");
            PlayerRollState = new PlayerRollState(this, "Roll");
            PlayerParryState = new PlayerParryState(this, "Parry");

            // Initialize state machine and Set active state
            StateMachine.Initialize(PlayerIdleState);
        }

        //public void SpawnPlayer(World physicsWorld, LDtkPlayer playerData, Vector2 spawnPos, bool newMap, LDtkLevel level)
        //{
        //    _physicsWorld = physicsWorld;

        //    Position = spawnPos;

        //    if (newMap)
        //    {
        //        LDtkGun gunData = level.GetEntityRef<LDtkGun>(playerData.StarterGun);
        //        HeldGun = new Gun(Game.GraphicsDevice, gunData, _physicsWorld);
        //    }

        //    // Call create body function
        //    CreateBody(Position);

        //    _currentHealth = GetMaxHealth();

        //    HeldGun.OnClipChanged += ClipSizeChanged;
        //}

        private void ClipSizeChanged(int clip)
        {
            _currentHealth = clip;
        }

        //public override void CreateBody(Vector2 position)
        //{
        //    _body = _physicsWorld.Create(position.X, position.Y, PlayerData.Size.X,
        //        PlayerData.Size.Y).AddTags(CollisionTags.Player);

        //    _body.Data = this;
        //}

        public override int GetMaxHealth()
        {
            if (HeldGun == null) return 666;
            return HeldGun.CurrentClip;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (HeldGun != null)
            {
                HeldGun.Update(gameTime);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (HeldGun != null)
            {
                HeldGun.Draw();
            }
        }

        //public IMovement PlayerMove(float delta)
        //{
        //    return Move(delta, PlayerData.Speed, PlayerData.GravityScale, (collision) =>
        //    {
        //        if (collision.Other.HasTag(CollisionTags.Enemy))
        //        {
        //            return CollisionResponses.Cross;
        //        }

        //        return CollisionResponses.Slide;
        //    });
        //}

        protected override void OnDeath()
        {
        }

        public override float GetIFrameTimer()
        {
            return 0.7f;
        }
    }
}
