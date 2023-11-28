﻿using TheLostHope.GameCode.Characters.FSM;
using Humper;
using Microsoft.Xna.Framework;
using TheLostHope.GameCode.Characters.PlayerCharacter.States;
using Humper.Responses;
using System;
using LDtkTypes;
using MonoGame.Aseprite;
using TheLostHope.GameCode.Guns;
using TheLostHopeEngine.EngineCode.Inputs;
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

        public Gun EquippedGun { get; private set; }


        // ---- Player Movement Variables -----
        private float _movementDirection;
        public float PlayerLastGroundedTime { get; private set; }
        public float PlayerLastJumpTime { get; private set; }
        public float PlayerLastRollTime { get; private set; }
        public bool PlayerJumping { get; private set; }
        public bool PlayerJumpInputReleased { get; private set; }
        public bool PlayerParryInputPressed { get; private set; }
        // ------------------------------------

        public Player(Game game, AsepriteFile asepriteFile, LDtkPlayer playerData) : base(game, asepriteFile)
        {
            // Set the player data
            PlayerData = playerData;

            // Inputs
            InputSystem.Instance.GetAction("Player_Move").OnChange += PlayerMoveInput;
            InputSystem.Instance.GetAction("Player_Jump").OnChange += PlayerJumpInput;
            InputSystem.Instance.GetAction("Player_Roll").OnChange += PlayerRollInput;
            InputSystem.Instance.GetAction("Player_Parry").OnChange += PlayerParryInput;

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

        private void PlayerMoveInput(InputActionContext context)
        {
            _movementDirection = context.Value;
        }

        private void PlayerJumpInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    PlayerLastJumpTime = PlayerData.JumpBufferTime;
                    break;
                case InputActionPhase.Released:
                    PlayerJumpInputReleased = true;
                    break;

            }
        }
        private void PlayerRollInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    PlayerLastRollTime = PlayerData.RollBufferTime;
                    break;
            }
        }
        private void PlayerParryInput(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    PlayerParryInputPressed = true;
                    break;
            }
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

            if (IsGrounded())
            {
                PlayerJumping = false;
                PlayerLastGroundedTime = PlayerData.JumpCayoteTime;
            }

            base.Update(gameTime);


            PlayerJumpInputReleased = false;
            PlayerParryInputPressed = false;
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
            MoveX(delta, (int)_movementDirection, PlayerData.GroundSpeed, PlayerData.Acceleration,
                PlayerData.Deacceleration, 1.2f);
        }
        public void MoveAir(float delta)
        {
            MoveX(delta, (int)_movementDirection, PlayerData.AirSpeed, PlayerData.Acceleration,
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
