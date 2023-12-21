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

    public enum WeaponShootDirection
    {
        None, Right, Left, Up, Down
    }

    public class WeaponAsset : ScriptableObject
    {
        [Header("Weapon Properties")]
        public string AsepriteFileName { get; set; }
        public Vector2 SpriteHalfSize { get; set; }
        public int MagazineSize { get; set; }
        public bool HoldToShoot { get; set; }
        public float PlayerKnockbackForceOnFire { get; set; }
        public float RightDirectionRotation { get; set; }
        public float LeftDirectionRotation { get; set; }
        public float UpDirectionRotation { get; set; }
        public float DownDirectionRotation { get; set; }
        public List<WeaponReloadPatternAction> ReloadPattern { get; set; } = new List<WeaponReloadPatternAction>();
        public int ReloadFailedMagazineSize { get; set; }

        [Header("Bullet Properties")]
        public float Speed { get; set; }
        public int Damage { get; set; }

        [Range(-50f, 50f)]
        public Vector2 RightFirePosition { get; set; }
        [Range(-50f, 50f)]
        public Vector2 LeftFirePosition { get; set; }
        [Range(-50f, 50f)]
        public Vector2 UpFirePosition { get; set; }
        [Range(-50f, 50f)]
        public Vector2 DownFirePosition { get; set; }

        public Vector2 Size { get; set; }
        public float MaxRange { get; set; }
        public float MaxLifeTime { get; set; }
        public int Penetration { get; set; }
        public int Bouciness { get; set; }
        public float EnemyKnockbackForceOnHit { get; set; }
        public float GravityScale { get; set; }
    }
}
