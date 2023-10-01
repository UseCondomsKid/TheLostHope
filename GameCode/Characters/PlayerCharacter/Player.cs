using TheLostHope.GameCode.Characters.FSM;
using Humper;
using Microsoft.Xna.Framework;
using TheLostHope.GameCode.Characters.PlayerCharacter.States;
using Humper.Responses;
using System;
using Microsoft.Xna.Framework.Input;
using LDtkTypes;
using Apos.Input;
using Track = Apos.Input.Track;
using System.Collections.Generic;
using TheLostHope.Engine.Animations;
using MonoGame.Aseprite;
using TheLostHope.GameCode.Characters.PlayerCharacter.States;
using System.Diagnostics;

namespace TheLostHope.GameCode.Characters.PlayerCharacter
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
        public PlayerInAirState PlayerInAirState { get; private set; }
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
        public ICondition ReloadInput { get; private set; }
        public ICondition InteractInput { get; private set; }
        // --------------------------

        // ---- Player Movement Variables -----
        public float PlayerLastGroundedTime { get; private set; }
        public float PlayerLastJumpTime { get; private set; }
        public bool PlayerJumping { get; private set; }
        // ------------------------------------

        public Player(Game game, AsepriteFile asepriteFile, LDtkPlayer playerData) : base(game, asepriteFile)
        {
            // Set the player data
            PlayerData = playerData;

            // Create the input conditions
            List<ICondition> conditions;
            int k, g;

            // Move Right
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerMoveRightBinding;
            g = Globals.Settings.GPlayerMoveRightBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            MoveRightInput = new AnyCondition(conditions.ToArray());
            // Move Left
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerMoveLeftBinding;
            g = Globals.Settings.GPlayerMoveLeftBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            MoveLeftInput = new AnyCondition(conditions.ToArray());
            // Jump
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerJumpBinding;
            g = Globals.Settings.GPlayerJumpBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            JumpInput = new AnyCondition(conditions.ToArray());
            // Roll
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerRollBinding;
            g = Globals.Settings.GPlayerRollBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            RollInput = new AnyCondition(conditions.ToArray());
            // Parry
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerParryBinding;
            g = Globals.Settings.GPlayerParryBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            ParryInput = new AnyCondition(conditions.ToArray());
            // Shoot
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerShootBinding;
            g = Globals.Settings.GPlayerShootBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            ShootInput = new AnyCondition(conditions.ToArray());
            // Reload
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerReloadBinding;
            g = Globals.Settings.GPlayerReloadBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            ReloadInput = new AnyCondition(conditions.ToArray());
            // Interact
            conditions = new List<ICondition>();
            k = Globals.Settings.KPlayerInteractBinding;
            g = Globals.Settings.GPlayerInteractBinding;
            if (k != -1) conditions.Add(new Track.KeyboardCondition((Keys)k));
            if (g != -1) conditions.Add(new Track.GamePadCondition((GamePadButton)g, 0));
            InteractInput = new AnyCondition(conditions.ToArray());


            // Initilize the states
            PlayerIdleState = new PlayerIdleState(this, "Idle");
            PlayerRunState = new PlayerRunState(this, "Run");
            PlayerJumpState = new PlayerJumpState(this, "Jump");
            PlayerInAirState = new PlayerInAirState(this, "Jump");
            PlayerLandState = new PlayerLandState(this, "Land");
            PlayerRollState = new PlayerRollState(this, "Roll");
            PlayerParryState = new PlayerParryState(this, "Parry");

            // Initialize movement variables
            PlayerLastGroundedTime = -1f;
            PlayerLastJumpTime = -1f;
            PlayerJumping = false;

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

        public void SetPlayerJumping(bool jumping)
        {
            PlayerJumping = jumping;
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PlayerLastGroundedTime -= delta;
            PlayerLastJumpTime -= delta;

            if (JumpInput.Pressed())
            {
                PlayerLastJumpTime = PlayerData.JumpBufferTime;
            }
            if (IsGrounded())
            {
                PlayerJumping = false;
                PlayerLastGroundedTime = PlayerData.JumpCayoteTime;
            }

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


        public void MoveGround(float delta)
        {
            int movement = (MoveRightInput.Held() ? 1 : 0) - (MoveLeftInput.Held() ? 1 : 0);

            MoveX(delta, movement, PlayerData.GroundSpeed, PlayerData.Acceleration,
                PlayerData.Deacceleration, 1.2f);
        }
        public void MoveAir(float delta)
        {
            int movement = (MoveRightInput.Held() ? 1 : 0) - (MoveLeftInput.Held() ? 1 : 0);

            MoveX(delta, movement, PlayerData.AirSpeed, PlayerData.Acceleration,
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
