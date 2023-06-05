using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.Rendering
{
    // This class implement some static helper functions concerning rendering
    public static class SpriteBatchExtensions
    {
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { color });

            spriteBatch.Draw(pixel, rectangle, color);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, int x1, int y1, int x2, int y2, Color color)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { color });

            float angle = (float)Math.Atan2(y2 - y1, x2 - x1);
            float length = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));
            spriteBatch.Draw(pixel, new Vector2(x1, y1), null, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }
        public static void DrawEllipse(this SpriteBatch spriteBatch, int centerX, int centerY, int radiusX, int radiusY, int segments, Color color)
        {
            double angleIncrement = 2 * Math.PI / segments;

            for (int i = 0; i < segments; i++)
            {
                double angle = i * angleIncrement;
                int x1 = (int)(centerX + radiusX * Math.Cos(angle));
                int y1 = (int)(centerY + radiusY * Math.Sin(angle));
                angle += angleIncrement;
                int x2 = (int)(centerX + radiusX * Math.Cos(angle));
                int y2 = (int)(centerY + radiusY * Math.Sin(angle));

                spriteBatch.DrawLine(x1, y1, x2, y2, color);
            }
        }

        public static Vector2 GetTextureCenter(int width, int height, float scale)
        {
            return new Vector2((width * scale) / 2, (height * scale) / 2);
        }
        public static Vector2 GetTextureCenter(Texture2D texture, float scale = 1f)
        {
            return new Vector2((texture.Width * scale) / 2, (texture.Height * scale) / 2);
        }
        public static Vector2 GetTexturePosition(int width, int height, Vector2 position, float scale = 1f)
        {
            return position - GetTextureCenter(width , height, scale);
        }
        public static Vector2 GetTexturePosition(Texture2D texture, Vector2 position, float scale = 1f)
        {
            return position - GetTextureCenter(texture, scale);
        }

        public static Vector2 GetRectangleCenter(Rectangle rectangle)
        {
            return new Vector2(rectangle.Width/2, rectangle.Height/2);
        }
        public static Vector2 GetRectanglePosition(Rectangle rectangle)
        {
            return new Vector2(rectangle.X, rectangle.Y) - GetRectangleCenter(rectangle);
        }

        public static Vector2 GetTextCenter(SpriteFont spriteFont, string text, float scale = 1f)
        {
            var strLen = spriteFont.MeasureString(text);
            return new Vector2(strLen.X/2, strLen.Y/2) * scale;
        }
        public static Vector2 GetTextPosition(SpriteFont spriteFont, string text, Vector2 position, float scale = 1f)
        {
            return position - GetTextCenter(spriteFont, text, scale);
        }
    }
}
