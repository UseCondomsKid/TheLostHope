using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LostHope.Engine.Camera;
using LostHope.GameCode.Characters.PlayerCharacter;
using LostHope.GameCode.GameStates;
using LostHope.GameCode.GameSettings;

namespace LostHope.GameCode
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
