using FontStashSharp;
using LDtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheLostHopeEngine.EngineCode.ContentManagement.CSV;
using TheLostHopeEngine.EngineCode.ContentManagement.Json;
using TheLostHopeEngine.EngineCode.Localization;

namespace TheLostHope.Engine.ContentManagement
{
    public static class ContentLoader
    {
        public static JsonAssetManager AssetManager { get; private set; }
        public static FontSystem FontSystem { get; private set; }
        public static LDtkFile LDtkFile { get; private set; }

        private static Dictionary<string, Texture2D> _textures;
        public static Dictionary<string, Texture2D> Textures
        {
            get
            {
                if (_textures == null) _textures = new Dictionary<string, Texture2D>();

                return _textures;
            }
        }

        private static Dictionary<string, AsepriteFile> _asepriteFiles;
        public static Dictionary<string, AsepriteFile> AsepriteFiles
        {
            get
            {
                if (_asepriteFiles == null) _asepriteFiles = new Dictionary<string, AsepriteFile>();

                return _asepriteFiles;
            }
        }


        private static ContentManager _content;
        public static ContentManager Content { get { return _content; } }

        public static void Initialize(ContentManager content)
        {
            _textures = new Dictionary<string, Texture2D>();
            _asepriteFiles = new Dictionary<string, AsepriteFile>();

            AssetManager = new JsonAssetManager();
            FontSystem = new FontSystem();
            _content = content;
        }
        public static void Unload()
        {
            FontSystem = null;
            LDtkFile = null;

            UnloadTextures();
            UnloadAsepriteFiles();
        }

        public static void AddFont(string fullName)
        {
            FontSystem.AddFont(TitleContainer.OpenStream($"{Content.RootDirectory}/{fullName}"));
        }
        public static void LoadLDtkFile(string contentName)
        {
            LDtkFile = LDtkFile.FromFile(contentName, _content);
        }
        public static List<LocalizationStringData.Entry> LoadLocalizationFileAsEntries()
        {
            var csvTest = File.ReadAllText($"{ContentLoader.Content.RootDirectory}/Localization.csv");
            var rows = CSVSerializer.ParseCSV(csvTest);
            return CSVSerializer.Deserialize<LocalizationStringData.Entry>(rows).ToList();
        }

        public static void LoadTexture(string name, string contentName)
        {
            if (_textures.ContainsKey(name)) return;
            _textures.Add(name, _content.Load<Texture2D>(contentName));
        }
        public static void UnloadTextures()
        {
            _textures.Clear();
            _textures = null;
        }
        public static Texture2D GetTexture(string name)
        {
            return _textures[name];
        }

        public static void LoadAsepriteFile(string name, string contentName)
        {
            if (_asepriteFiles.ContainsKey(name)) return;
            _asepriteFiles.Add(name, _content.Load<AsepriteFile>(contentName));
        }
        public static void UnloadAsepriteFiles()
        {
            _asepriteFiles.Clear();
            _asepriteFiles = null;
        }
        public static AsepriteFile GetAsepriteFile(string name)
        {
            return _asepriteFiles[name];
        }
    }
}
