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
using MonoGame.Aseprite;
using TheLostHope.GameCode.Guns;
using TheLostHopeEngine.EngineCode.Inputs.Interfaces;
using TheLostHopeEngine.EngineCode.Inputs;

namespace TheLostHope.GameCode.Characters.PlayerCharacter
{
    public class Player : Character, IInputBindingUser
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

        public Gun EquippedGun { get; private set; }


        // ------ Player Inputs -----
        // Movement and abilities
        public ICondition MoveRightInput { get; private set; }
        public ICondition MoveLeftInput { get; private set; }
        public ICondition JumpInput { get; private set; }
        public ICondition RollInput { get; private set; }
        public ICondition ParryInput { get; private set; }
        public ICondition InteractInput { get; private set; }
        // --------------------------

        // ---- Player Movement Variables -----
        public float PlayerLastGroundedTime { get; private set; }
        public float PlayerLastJumpTime { get; private set; }
        public float PlayerLastRollTime { get; private set; }
        public bool PlayerJumping { get; private set; }
        // ------------------------------------

        public Player(Game game, AsepriteFile asepriteFile, LDtkPlayer playerData) : base(game, asepriteFile)
        {
            // Set the player data
            PlayerData = playerData;

            // Inputs
            RegisterInputBindingsUser();
            SetupInputBindings();

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
            PlayerLastRollTime = -1f;
            PlayerJumping = false;

            // Initialize state machine and Set active state
            StateMachine.Initialize(PlayerIdleState);
        }

        public void RegisterInputBindingsUser()
        {
            InputBindingManager.RegisterUser(this);
        }
        public void SetupInputBindings()
        {
            InputBinding b = InputBindingManager.GetInputBinding("PlayerMoveRight");
            MoveRightInput = new AnyCondition(
                new KeyboardCondition(b.KeyboadKey), new GamePadCondition(b.GamepadButton, 0));

            b = InputBindingManager.GetInputBinding("PlayerMoveLeft");
            MoveLeftInput = new AnyCondition(
                new KeyboardCondition(b.KeyboadKey), new GamePadCondition(b.GamepadButton, 0));

            b = InputBindingManager.GetInputBinding("PlayerJump");
            JumpInput = new AnyCondition(
                new KeyboardCondition(b.KeyboadKey), new GamePadCondition(b.GamepadButton, 0));

            b = InputBindingManager.GetInputBinding("PlayerJump");
            JumpInput = new AnyCondition(
                new KeyboardCondition(b.KeyboadKey), new GamePadCondition(b.GamepadButton, 0));
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
            PlayerLastRollTime -= delta;

            if (JumpInput.Pressed())
            {
                PlayerLastJumpTime = PlayerData.JumpBufferTime;
            }
            if (RollInput.Pressed())
            {
                PlayerLastRollTime = PlayerData.RollBufferTime;
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
        public void ApplyGravity(float delta)
        {
            if (!IsGrounded())
            {
                MoveY(delta, 1, PlayerData.MaxGravityVelocity, PlayerData.GravityScale, PlayerData.GravityScale, 1.2f);
            }
        }
    }
}
