// This file was automatically generated, any modifications will be lost!
#pragma warning disable
namespace LDtkTypes;

using Microsoft.Xna.Framework;
using LDtk;

public class LDtkGun : ILDtkEntity
{
    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public string Name { get; set; }
    public string AsepriteFileName { get; set; }
    public int ClipSize { get; set; }
    public bool Auto { get; set; }
    public float TimeBetweenShots { get; set; }
    public float Range { get; set; }
    public int Damage { get; set; }
    public float KnockbackForce { get; set; }
    public int BulletWidth { get; set; }
    public bool CanPenetrate { get; set; }
}
#pragma warning restore
