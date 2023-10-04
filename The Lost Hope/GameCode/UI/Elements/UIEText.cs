using FontStashSharp;
using TheLostHope.Engine.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEngine.EngineCode.UI;

namespace TheLostHope.GameCode.UI.Elements
{
    public class UIEText : UIElement
    {
        private string _text = string.Empty;
        private DynamicSpriteFont _font;
        private Color _fontColor;

        public UIEText(GraphicsDevice graphicsDevice, UIAnchor anchor, float xPosPercent, float yPosPercent, UIElement parent, string text,
            float fontSize = 24f, Color fontColor = default) : base(graphicsDevice, anchor, xPosPercent, yPosPercent, parent)
        {
            SetText(text, fontSize);
            _fontColor = fontColor;
        }

        public override Vector2 GetSize()
        {
            if (_text == string.Empty || _font == null) return Vector2.Zero;

            var size = _font.MeasureString(_text);
            return size;
        }

        public void SetText(string text, float fontSize = 24f)
        {
            _text = text;
            _font = ContentLoader.FontSystem.GetFont(fontSize);
        }

        public override void UpdateSelf(GameTime gameTime)
        {
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_text == string.Empty || _font == null) return;

            spriteBatch.DrawString(_font, _text, GetCenteredDrawPosition(), _fontColor);
        }
    }
}
