using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TheLostHope.Engine.Camera;
using TheLostHope.GameCode.Characters.PlayerCharacter;
using TheLostHope.GameCode.GameStates;
using TheLostHope.GameCode.GameSettings;

namespace TheLostHope.GameCode
{
    public static class Globals
    {
        public static int CurrentScreenWidth { get; set; }
        public static int CurrentScreenHeight { get; set; }

        public static Settings Settings { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static GameTime GameTime { get; set; }
    }
}
