using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TheLostHopeEditor.EditorCode.Camera;

namespace TheLostHopeEditor.EditorCode
{
    public static class Globals
    {
        public static int CurrentScreenWidth { get; set; }
        public static int CurrentScreenHeight { get; set; }
        public static OrthographicCamera Camera { get; set; }
    }
}
