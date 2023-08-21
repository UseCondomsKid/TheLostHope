using LDtkTypes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Weapons.Bullets
{
    public abstract class Bullet : DrawableGameComponent
    {
        protected Vector2 _firePosition;
        protected Vector2 _endPosition;

        protected float _range;
        protected int _damage;
        protected bool _canPenetrate;

        protected Bullet(Game game, Vector2 startPos, Vector2 endPos, float range, int damage, bool canPenetrate) : base(game)
        {
            _firePosition = startPos;
            _endPosition = endPos;

            _range = range;
            _damage = damage;
            _canPenetrate = canPenetrate;
        }
        public abstract void Fire();

        public override void Update(GameTime gameTime)
        {
        }
        public override void Draw(GameTime gameTime)
        {
        }
    }
}
