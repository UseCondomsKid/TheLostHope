using System.Collections.Generic;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.CustomAttributes;
using TheLostHopeEngine.EngineCode.Inputs;

namespace TheLostHopeEngine.EngineCode.Assets
{
    public class InputAsset : ScriptableObject
    {
        [Header("Inputs")]
        public List<InputAction> Actions { get; set; } = new List<InputAction>();
    }
}
