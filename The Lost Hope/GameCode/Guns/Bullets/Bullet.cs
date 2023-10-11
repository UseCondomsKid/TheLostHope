using LDtkTypes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Guns.Bullets
{
    public abstract class Bullet
    {
        protected Vector2 _firePosition;
        protected int _fireDirection;
        protected float _range;
        protected int _damage;
        protected bool _canPenetrate;
        protected float _enemyKnockbackForce;

        protected Bullet(Vector2 startPos, int fireDirection, float range, int damage, bool canPenetrate, float enemyKnockbackForce)
        {
            _firePosition = startPos;
            _fireDirection = fireDirection;

            _range = range;
            _damage = damage;
            _canPenetrate = canPenetrate;
            _enemyKnockbackForce = enemyKnockbackForce;
        }

        // Called to instantiate the bullet
        public abstract void Fire();
        // Used to release the bullet from the pool
        public abstract void Release();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
