using LDtkTypes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Weapons.Bullets
{
    public class HitscanBullet : Bullet
    {
        public HitscanBullet(Game game, Vector2 startPos, Vector2 endPos, float range, int damage, bool canPenetrate) : base(game, startPos, endPos, range, damage, canPenetrate)
        {
        }

        public override void Fire()
        {
        }
    }
}
