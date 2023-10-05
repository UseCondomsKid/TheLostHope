using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.CustomAttributes;

namespace TheLostHopeEngine.EngineCode.Assets
{
    public class WeaponAsset : ScriptableObject
    {
        public string AsepriteFileContentPath { get; set; } = "";
        public int MagazineSize { get; set; }
        [Range(0.0f, 10.0f)]
        public float TimeBetweenShots { get; set; }
    }
}
