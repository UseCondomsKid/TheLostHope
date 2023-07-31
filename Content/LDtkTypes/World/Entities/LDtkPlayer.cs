// This file was automatically generated, any modifications will be lost!
#pragma warning disable
namespace LDtkTypes;

using Microsoft.Xna.Framework;
using LDtk;

public class LDtkPlayer : ILDtkEntity
{
    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public float GravityScale { get; set; }
    public float Speed { get; set; }
    public float Acceleration { get; set; }
    public float Deacceleration { get; set; }
    public float JumpForce { get; set; }
    public float RollVelocity { get; set; }
    public EntityRef StarterGun { get; set; }
}
#pragma warning restore
