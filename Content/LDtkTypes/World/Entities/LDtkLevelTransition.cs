// This file was automatically generated, any modifications will be lost!
#pragma warning disable
namespace LDtkTypes;

using Microsoft.Xna.Framework;
using LDtk;

public class LDtkLevelTransition : ILDtkEntity
{
    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public string LevelToTransitionTo { get; set; }
    public string Id { get; set; }
    public Point SpawnPos { get; set; }
}
#pragma warning restore
