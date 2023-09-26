using FontStashSharp;
using LDtk;
using LostHope.Engine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LostHope.Engine.ContentLoading
{
    public static class ContentLoader
    {
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

        private static Dictionary<string, AsepriteExportData> _asepriteAnimations;
        public static Dictionary<string, AsepriteExportData> AsepriteAnimations
        {
            get
            {
                if (_asepriteAnimations == null) _asepriteAnimations = new Dictionary<string, AsepriteExportData>();

                return _asepriteAnimations;
            }
        }

        private static Dictionary<string, SoundEffect> _sfx;
        public static Dictionary<string, SoundEffect> Sfx
        {
            get
            {
                if (_sfx == null) _sfx = new Dictionary<string, SoundEffect>();
                return _sfx;
            }
        }


        private static ContentManager _content;
        public static ContentManager Content { get { return _content; } }

        public static void Initialize(ContentManager content)
        {
            _textures = new Dictionary<string, Texture2D>();
            _asepriteAnimations = new Dictionary<string, AsepriteExportData>();
            _sfx = new Dictionary<string, SoundEffect>();

            FontSystem = new FontSystem();
            _content = content;
        }
        public static void Unload()
        {
            FontSystem = null;
            LDtkFile = null;

            UnloadTextures();
            UnloadAsepriteFiles();
            UnloadSfx();
        }

        public static void AddFont(string fullName)
        {
            FontSystem.AddFont(TitleContainer.OpenStream($"{Content.RootDirectory}/{fullName}"));
        }
        public static void LoadLDtkFile(string contentName)
        {
            LDtkFile = LDtkFile.FromFile(contentName, _content);
        }
        public static List<Localization.LocalizationStringData.Entry> LoadLocalizationFileAsEntries()
        {
            var csvTest = File.ReadAllText($"{ContentLoader.Content.RootDirectory}/Localization.csv");
            var rows = CSVSerializer.ParseCSV(csvTest);
            return CSVSerializer.Deserialize<Localization.LocalizationStringData.Entry>(rows).ToList();
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

        public static void LoadAsepriteFile(string name)
        {
            if (_asepriteAnimations.ContainsKey(name)) return;
            _asepriteAnimations.Add(name, new AsepriteExportData(name));
        }
        public static void UnloadAsepriteFiles()
        {
            _asepriteAnimations.Clear();
            _asepriteAnimations = null;
        }
        public static AsepriteExportData GetAsepriteFile(string name)
        {
            return _asepriteAnimations[name];
        }

        public static void LoadSfx(string name, string path)
        {
            if (_sfx.ContainsKey(name)) return;
            _sfx.Add(name, _content.Load<SoundEffect>(path));
        }
        public static void UnloadSfx()
        {
            _sfx.Clear();
            _sfx = null;
        }
        public static SoundEffect GetSfx(string name)
        {
            return _sfx[name];
        }
    }
}
