using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.ContentLoading
{
    public static class ContentLoader
    {
        public static SpriteFont Font { get; private set; }

        private static Dictionary<string, Texture2D> _textures;

        public static Dictionary<string, Texture2D> Textures
        {
            get
            {
                if (_textures == null) _textures = new Dictionary<string, Texture2D>();

                return _textures;
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

        public static void Initialize(ContentManager content)
        {
            _textures = new Dictionary<string, Texture2D>();
            _sfx = new Dictionary<string, SoundEffect>();

            _content = content;
        }

        public static void LoadSpriteFont(string contentName)
        {
            Font =_content.Load<SpriteFont>(contentName);
        }

        public static void LoadTexture(string name, string contentName)
        {
            _textures.Add(name, _content.Load<Texture2D>(contentName));
        }
        public static Texture2D GetTexture(string name)
        {
            return _textures[name];
        }

        public static void LoadSfx(string name, string path)
        {
            Sfx.Add(name, _content.Load<SoundEffect>(path));
        }
        public static void PlaySoundEffect(string name)
        {
            Sfx[name].Play();
        }
    }
}
