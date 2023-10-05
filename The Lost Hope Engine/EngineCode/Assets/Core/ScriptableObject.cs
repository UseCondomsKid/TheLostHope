using System;

namespace TheLostHopeEngine.EngineCode.Assets.Core
{
    public abstract class ScriptableObject
    {
        public Guid AssetId { get; private set; } // Unique identifier for the asset
        public string Name { get; set; } // Name of the asset

        public ScriptableObject()
        {
            AssetId = Guid.NewGuid(); // Generate a new unique identifier
            Name = ""; // Set the name to be an empty string
        }
    }
}
