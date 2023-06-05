using System;

namespace LostHope.GameCode
{
    // This is used to give tags to colliders, so that we can diffirentiate between them.
    [Flags]
    public enum CollisionTags
    {
        Player = 1 << 0, // Player Tag
        Ground = 1 << 1, // Platform/Solid Block Tag
        Enemy = 1 << 2,
    }
}
