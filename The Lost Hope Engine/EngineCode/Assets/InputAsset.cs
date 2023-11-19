using Apos.Input;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.CustomAttributes;

namespace TheLostHopeEngine.EngineCode.Assets
{
    public class InputAsset : ScriptableObject
    {
        [Header("Player")]
        public Keys K_PlayerMoveRightInput { get; set; }
        public GamePadButton G_PlayerMoveRightInput { get; set; }
        public Keys K_PlayerMoveLeftInput { get; set; }
        public GamePadButton G_PlayerMoveLeftInput { get; set; }
        public Keys K_PlayerJumpInput { get; set; }
        public GamePadButton G_PlayerJumpInput { get; set; }
        public Keys K_PlayerRollInput { get; set; }
        public GamePadButton G_PlayerRollInput { get; set; }
        public Keys K_PlayerParryInput { get; set; }
        public GamePadButton G_PlayerParryInput { get; set; }
        public Keys K_PlayerInteractInput { get; set; }
        public GamePadButton G_PlayerInteractInput { get; set; }

        [Header("Gun")]
        public Keys K_GunShootInput { get; set; }
        public GamePadButton G_GunShootInput { get; set; }
        public Keys K_GunInitializeReloadInput { get; set; }
        public GamePadButton G_GunInitializeReloadInput { get; set; }
        public Keys K_GunReloadUpInput { get; set; }
        public GamePadButton G_GunReloadUpInput { get; set; }
        public Keys K_GunReloadDownInput { get; set; }
        public GamePadButton G_GunReloadDownInput { get; set; }
        public Keys K_GunReloadLeftInput { get; set; }
        public GamePadButton G_GunReloadLeftInput { get; set; }
        public Keys K_GunReloadRightInput { get; set; }
        public GamePadButton G_GunReloadRightInput { get; set; }
    }
}
