using LostHope.GameCode.Characters.FSM;
using Humper;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using LostHope.GameCode.Weapons;
using LostHope.GameCode.Characters.PlayerCharacter.States;
using Humper.Responses;
using System;
using Microsoft.Xna.Framework.Input;
using LDtkTypes;
using Apos.Input;
using System.Collections.Generic;
using System.Linq;

namespace LostHope.GameCode.Characters.PlayerCharacter
{
    public class Player : Character
    {
        // -----  Player Data  ------
        public LDtkPlayer PlayerData { get; private set; }
        // --------------------------

        // ----- Player States ------
        public PlayerIdleState PlayerIdleState { get; private set; }
        public PlayerRunState PlayerRunState { get; private set; }
        public PlayerJumpState PlayerJumpState { get; private set; }
        public PlayerLandState PlayerLandState { get; private set; }
        public PlayerRollState PlayerRollState { get; private set; }
        public PlayerParryState PlayerParryState { get; private set; }
        // --------------------------


        // ------ Player Inputs -----
        public ICondition MoveRightInput { get; private set; }
        public ICondition MoveLeftInput { get; private set; }
        public ICondition JumpInput { get; private set; }
        public ICondition RollInput { get; private set; }
        public ICondition ParryInput { get; private set; }
        public ICondition ShootInput { get; private set; }
        public ICondition InteractInput { get; private set; }
        // --------------------------

        public Player(Game game, World physicsWorld, AsepriteFile asepriteFile, LDtkPlayer playerData) : base(game, physicsWorld, asepriteFile)
        {
            // Set the player data
            PlayerData = playerData;

            // Create the input conditions
            List<ICondition> conditions;
            int k, m, g;

            // Move Right
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerMoveRightBinding;
            m = Globals.Settings.MPlayerMoveRightBinding;
            g = Globals.Settings.GPlayerMoveRightBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            MoveRightInput = new AnyCondition(conditions.ToArray());
            // Move Left
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerMoveLeftBinding;
            m = Globals.Settings.MPlayerMoveLeftBinding;
            g = Globals.Settings.GPlayerMoveLeftBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            MoveLeftInput = new AnyCondition(conditions.ToArray());
            // Jump
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerJumpBinding;
            m = Globals.Settings.MPlayerJumpBinding;
            g = Globals.Settings.GPlayerJumpBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            JumpInput = new AnyCondition(conditions.ToArray());
            // Roll
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerRollBinding;
            m = Globals.Settings.MPlayerRollBinding;
            g = Globals.Settings.GPlayerRollBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            RollInput = new AnyCondition(conditions.ToArray());
            // Parry
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerParryBinding;
            m = Globals.Settings.MPlayerParryBinding;
            g = Globals.Settings.GPlayerParryBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            ParryInput = new AnyCondition(conditions.ToArray());
            // Shoot
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerShootBinding;
            m = Globals.Settings.MPlayerShootBinding;
            g = Globals.Settings.GPlayerShootBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            ShootInput = new AnyCondition(conditions.ToArray());
            // Interact
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerInteractBinding;
            m = Globals.Settings.MPlayerInteractBinding;
            g = Globals.Settings.GPlayerInteractBinding;
            if (k != -1) conditions.Add(new KeyboardCondition((Keys)k));
            if (m != -1) conditions.Add(new MouseCondition((MouseButton)m));
            if (g != -1) conditions.Add(new GamePadCondition((GamePadButton)g, 0));
            InteractInput = new AnyCondition(conditions.ToArray());


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
        public override IBox CreateCharacterBox(float xPos, float yPos)
        {
            return _physicsWorld.Create(xPos, yPos, PlayerData.Size.X,
                PlayerData.Size.Y).AddTags(CollisionTags.Player);
        }

        public override int GetMaxHealth()
        {
            // if (HeldGun == null) return 666;
            // return HeldGun.CurrentClip;

            return 10;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void OnDeath()
        {
        }

        public override float GetIFrameTimer()
        {
            return 0.7f;
        }

        protected override void OnTakeDamage()
        {
        }

        public override Func<ICollision, CollisionResponses> GetCollisionFilter()
        {
            return new Func<ICollision, CollisionResponses>((collision) =>
            {
                if (collision.Other.HasTag(CollisionTags.Enemy))
                {
                    return CollisionResponses.Cross;
                }
                return CollisionResponses.Slide;
            });
        }


        public void Move(float delta)
        {
            int movement = (MoveRightInput.Held() ? 1 : 0) - (MoveLeftInput.Held() ? 1 : 0);

            MoveX(delta, movement, PlayerData.Speed, PlayerData.Acceleration,
                PlayerData.Deacceleration, 1.2f);
        }
        public void ApplyGravity()
        {
            if (!IsGrounded())
            {
                SetVelocityY(Velocity.Y >= PlayerData.MaxGravityVelocity ? PlayerData.MaxGravityVelocity : Velocity.Y + PlayerData.GravityScale);
            }
        }
    }
}
