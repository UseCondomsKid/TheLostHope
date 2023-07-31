using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Characters
{
    public class Object : DrawableGameComponent
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Object(Game game) : base(game)
        {
            Rotation = 0f;
            Scale = 1f;
        }
    }
}
