using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEngine.EngineCode.Assets;

namespace TheLostHope.GameCode.ContentLoading
{
    // The role of this call is to load all resources that need be loaded at the start of the game
    public static class GameAssetsLoader
    {
        private static bool _initialized = false;

        private static Dictionary<string, WeaponAsset> _weaponAssets;
        private static InputAsset _inputAsset;


        // Public getters
        public static InputAsset InputAsset
        {
            get
            {
                if (!_initialized) return null;
                return _inputAsset; 
            }
        }

        public static void Initialize()
        {
            // Load input asset
            _inputAsset = ContentLoader.AssetManager.LoadAsset<InputAsset>(
                ContentLoader.GetApplicationRelativePath("Assets/Input/InputAsset.asset"));

            // Load Weapon Assets
            _weaponAssets = new Dictionary<string, WeaponAsset>();
            string[] weaponFiles = Directory.GetFiles(ContentLoader.GetApplicationRelativePath("Assets/Guns"), "*.asset");

            for (int i = 0; i < weaponFiles.Length; i++)
            {
                var asset = ContentLoader.AssetManager.LoadAsset<WeaponAsset>(weaponFiles[i]);
                _weaponAssets.Add(asset.Name, asset);
            }

            _initialized = true;
        }

        public static WeaponAsset GetWeaponAsset(string name)
        {
            if (!_initialized) return null;

            WeaponAsset asset = null;
            _weaponAssets.TryGetValue(name, out asset);
            return asset;
        }
    }
}
