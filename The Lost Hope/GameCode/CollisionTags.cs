using System;

namespace TheLostHope.GameCode
{
    // This is used to give tags to colliders, so that we can diffirentiate between them.
    [Flags]
    public enum CollisionTags
    {
        Player = 1 << 0, // Player Tag
        Ground = 1 << 1, // Platform/Solid Block Tag
        Damageable = 1 << 2,
        Bullet = 1 << 3,
    }
}
