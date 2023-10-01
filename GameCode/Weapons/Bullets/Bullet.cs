using LDtkTypes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Weapons.Bullets
{
    public abstract class Bullet
    {
        protected Vector2 _firePosition;

        protected float _range;
        protected int _damage;
        protected bool _canPenetrate;

        protected Bullet(Vector2 startPos, float range, int damage, bool canPenetrate)
        {
            _firePosition = startPos;

            _range = range;
            _damage = damage;
            _canPenetrate = canPenetrate;
        }

        // Called to instantiate the bullet
        public abstract void Fire();
        // Used to release the bullet from the pool
        public abstract void Release();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
