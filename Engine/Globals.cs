using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LostHope.Engine.Camera;
using LostHope.GameCode.Characters.PlayerCharacter;
using Humper;

namespace LostHope.Engine
{
    public static class Globals
    {
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

        // Game Camera
        public static OrthographicCamera GameCamera { get; set; }

        public static Player Player { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static ContentManager Content { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static GameTime GameTime { get; set; }
        public static SpriteFont Font { get; set; }

        public static void LoadTexture(string name, string path)
        {
            Textures.Add(name, Content.Load<Texture2D>(path));
        }
        public static void LoadSfx(string name, string path)
        {
            Sfx.Add(name, Content.Load<SoundEffect>(path));
        }

        public static Texture2D GetTexture(string name)
        {
            return Textures[name];
        }
        public static void PlaySoundEffect(string name)
        {
            Sfx[name].Play();
        }
    }
}
