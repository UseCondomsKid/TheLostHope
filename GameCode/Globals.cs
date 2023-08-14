using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LostHope.Engine.Camera;
using LostHope.GameCode.Characters.PlayerCharacter;

namespace LostHope.GameCode
{
    public static class Globals
    {
        public static OrthographicCamera GameCamera { get; set; }

        public static Player Player { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static GameTime GameTime { get; set; }
    }
}
