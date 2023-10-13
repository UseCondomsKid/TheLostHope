using Apos.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEngine.EngineCode.Assets;

namespace TheLostHopeEngine.EngineCode.Inputs
{
    public class InputBinding
    {
        public string Id { get; set; }
        public bool Editabled { get; set; }
        public Keys KeyboadKey { get; set; }
        public GamePadButton GamepadButton { get; set; }
    }

    public static class InputManager
    {
        public static void Initialize(InputAsset inputAsset)
        {
        }
    }
}
