using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.Objects
{
    public class GameObject : DrawableGameComponent
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public GameObject(Game game) : base(game)
        {
            Rotation = 0f;
            Scale = 1f;
        }

        public virtual void SpawnObject(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}
