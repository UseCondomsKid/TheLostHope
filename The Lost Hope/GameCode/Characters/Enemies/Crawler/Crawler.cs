using Humper;
using Humper.Responses;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.Characters.Enemies.Crawler.States;
using TheLostHope.GameCode.Characters.FSM;

namespace TheLostHope.GameCode.Characters.Enemies.Crawler
{
    public class Crawler : Character
    {
        public CrawlerPatrolState CrawlerPatrolState { get; private set; }

        public Crawler(Game game, AsepriteFile asepriteFile) : base(game, asepriteFile)
        {
            CrawlerPatrolState = new CrawlerPatrolState(this, "Walk");

            // Initialize state machine and Set active state
            StateMachine.Initialize(CrawlerPatrolState);
        }

        public override Vector2 GetBoxSize()
        {
            return new Vector2(26, 15);
        }

        public override Func<ICollision, CollisionResponses> GetCollisionFilter()
        {
            return new Func<ICollision, CollisionResponses>((collision) =>
            {
                if (collision.Other.HasTag(CollisionTags.Player))
                {
                    return CollisionResponses.None;
                }
                else if (collision.Other.HasTag(CollisionTags.Bullet))
                {
                    return CollisionResponses.None;
                }
                return CollisionResponses.Slide;
            });
        }

        public override CollisionTags GetCollisionTag()
        {
            return CollisionTags.Damageable;
        }

        public override float GetIFrameTimer()
        {
            return 0.2f;
        }

        public override int GetMaxHealth()
        {
            return 13;
        }

        protected override void OnDeath()
        {
            DespawnCharacter();
        }

        protected override void OnHealthChanged()
        {
        }

        protected override void OnTakeDamage()
        {
        }
    }
}
