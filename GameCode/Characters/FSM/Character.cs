using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LostHope.Engine.Animations;
using Humper;
using Humper.Responses;
using MonoGame.Aseprite;
using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LostHope.GameCode.Characters.FSM
{
    // Base character class for all the game character aka player, enemies, npcs
    public abstract class Character : Object
    {
        // State machine
        public CharacterStateMachine StateMachine { get; private set; }
        // Animator
        public Animator Animator { get; private set; }
        // public getter for the collider
        public IBox Body { get { return _body; }}
        public int FacingDirection { get { return _facingDirection; } }
        public bool IFrame { get; set; }
        public IMovement CurrentMovement { get; private protected set; }

        // The collider
        protected IBox _body;
        // The physics world
        protected World _physicsWorld;
        protected int _facingDirection;
        protected int _currentHealth;

        private float _iFrameTimer;

        private Vector2 velWorkspace;

        // Required funtion that creates the collider, when inheriting from this class,
        // every child has to implement these functions
        public abstract Func<ICollision, CollisionResponses> GetCollisionFilter();
        public abstract int GetMaxHealth();
        public abstract float GetIFrameTimer();
        protected abstract void OnTakeDamage();
        protected abstract void OnDeath();

        // Constructor
        public Character(Game game, AsepriteFile asepriteFile) : base(game)
        {
            // Initializations
            StateMachine = new CharacterStateMachine();
            Animator = new Animator(asepriteFile, GraphicsDevice);
            IFrame = false;

            _facingDirection = 1; // Right

            _iFrameTimer = -1f;

            _currentHealth = GetMaxHealth();
        }

        public abstract IBox CreateCharacterBox(float xPos, float yPos);

        public virtual void SpawnCharacter(Vector2 position, World physicsWorld)
        {
            _physicsWorld = physicsWorld;

            _body = CreateCharacterBox(position.X, position.Y);
            _body.Data = this;
            Position = position;
            Velocity = Vector2.Zero;

            _currentHealth = GetMaxHealth();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Timers
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _iFrameTimer -= delta;

            // Update the animator
            Animator.Update(gameTime);

            // Update the state machine
            StateMachine.CurrentState.Update(delta);

            // Update Physics
            CurrentMovement = MovementStep(delta);

            // Set facing direction
            if (Math.Abs(Velocity.X) > 0f && Math.Sign(Velocity.X) != Math.Sign(_facingDirection))
                _facingDirection *= -1;
        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Animator.SpriteSheetTexture == null) return;

            // Draw using the position, scale, rotation, texture and animation
            Globals.SpriteBatch.Draw(Animator.SpriteSheetTexture, Position, Animator.GetSourceRectangle(),
                IFrame ? Color.DodgerBlue : _iFrameTimer > 0 ? Color.Black : Color.White, Rotation,
                Vector2.Zero, Scale, _facingDirection == 1 ? SpriteEffects.None :
                SpriteEffects.FlipHorizontally, 0);
        }

        #region Health
        public virtual void TakeDamage(int damage)
        {
            if (_iFrameTimer > 0 || IFrame) return;

            _iFrameTimer = GetIFrameTimer();
            _currentHealth -= damage;
            OnTakeDamage();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDeath();
            }
        }
        #endregion

        #region Movement
        public bool IsMoving()
        {
            return Math.Abs(Velocity.X) > 10f;
        }
        public void SetVelocity(float velocity, Vector2 angle, int direction)
        {
            angle.Normalize();
            velWorkspace = new Vector2(angle.X * velocity * direction, angle.Y * velocity);
            SetFinalVelocity();
        }
        public void SetVelocity(float velocity, Vector2 direction)
        {
            velWorkspace = direction * velocity;
            SetFinalVelocity();
        }
        public void SetVelocity(Vector2 velocity)
        {
            velWorkspace = velocity;
            SetFinalVelocity();
        }
        public virtual void SetVelocityX(float velocity)
        {
            velWorkspace = new Vector2(velocity, Velocity.Y);
            SetFinalVelocity();
        }
        public virtual void SetVelocityY(float velocity)
        {
            velWorkspace = new Vector2(Velocity.X, velocity);
            SetFinalVelocity();
        }
        private void SetFinalVelocity()
        {
            Velocity = velWorkspace;
        }
        public void MoveX(float delta, int xInput, float speed, float accel, float deaccel, float velocityPower)
        {
            float targetSpeedX = xInput * MathHelper.Max(speed, 0f);

            float speedDifX = targetSpeedX - Velocity.X;

            float accelRateX = (Math.Abs(targetSpeedX) > 0.01f) ? accel : deaccel;

            float movementX = (float)Math.Pow(Math.Abs(speedDifX) * accelRateX, velocityPower) * Math.Sign(speedDifX);

            Velocity = new Vector2(Velocity.X + (delta * movementX),
                        Velocity.Y);
        }
        public void MoveY(float delta, int yInput, float speed, float accel, float deaccel, float velocityPower)
        {
            float targetSpeedY = yInput * MathHelper.Max(speed, 0f);

            float speedDifY = targetSpeedY - Velocity.Y;

            float accelRateY = (Math.Abs(targetSpeedY) > 0.01f) ? accel : deaccel;

            float movementY = (float)Math.Pow(Math.Abs(speedDifY) * accelRateY, velocityPower) * Math.Sign(speedDifY);

            Velocity = new Vector2(Velocity.X,
                        Velocity.Y + (delta * movementY));
        }

        /// <summary>
        /// This moves the character body and sets it's position, should be called after all velocity operations are done.
        /// </summary>
        public IMovement MovementStep(float delta)
        {
            var movement = _body.Move(_body.X + delta * Velocity.X,
                _body.Y + delta * Velocity.Y, GetCollisionFilter());

            Position = new Vector2(_body.X, _body.Y);

            return movement;
        }
        #endregion

        #region Checks
        public bool IsGrounded()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.Y < 0));
        }
        public bool IsTouchingCeiling()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.Y > 0));
        }
        public bool IsTouchingRightWall()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.X > 0));
        }
        public bool IsTouchingLeftWall()
        {
            if (CurrentMovement == null) return false;

            return CurrentMovement.Hits.Any((c) => c.Box.HasTag(CollisionTags.Ground) &&
                                 (c.Normal.X < 0));
        }
        public bool IsTouchingWall()
        {
            return IsTouchingRightWall() || IsTouchingLeftWall();
        }
        #endregion
    }
}
