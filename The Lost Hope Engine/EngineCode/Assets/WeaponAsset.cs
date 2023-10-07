using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.CustomAttributes;

namespace TheLostHopeEngine.EngineCode.Assets
{
    public enum WeaponReloadPatternAction
    {
        Up, Down, Left, Right
    }

    public class WeaponAsset : ScriptableObject
    {
        [Header("Weapon Properties")]
        public string AsepriteFileName { get; set; } = "";
        public int MagazineSize { get; set; }
        [Range(0.0f, 10.0f)]
        public float TimeBetweenShots { get; set; }
        public bool HoldToShoot { get; set; }
        public float PlayerKnockbackForceOnFire { get; set; }
        public List<WeaponReloadPatternAction> ReloadPattern { get; set; } = new List<WeaponReloadPatternAction>();
        public int ReloadFailedMagazineSize { get; set; }

        [Header("Bullet Properties")]
        public float Speed { get; set; }
        public int Damage { get; set; }
        public Vector2 FirePosition { get; set; }
        public Vector2 Size { get; set; }
        public float MaxRange { get; set; }
        public float MaxLifeTime { get; set; }
        public int Penetration { get; set; }
        public int Bouciness { get; set; }
        public float EnemyKnockbackForceOnHit { get; set; }
        public float GravityScale { get; set; }
    }
}
