using TheLostHope.GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHope.Engine.UI
{
    public abstract class UIElement
    {
        public UIAnchor Anchor { get; set; }
        // The Position percentage between -1 and 1.
        public float XPositionPercent { get; set; }
        public float YPositionPercent { get; set; }

        public UIElement Parent { get; set; }
        public List<UIElement> Children { get; set; }

        public UIElement(UIAnchor anchor, float xPosPercent, float yPosPercent, UIElement parent)
        {
            Anchor = anchor;
            XPositionPercent = xPosPercent;
            YPositionPercent = yPosPercent;

            Parent = parent;
            Children = new List<UIElement>();
        }

        public abstract Vector2 GetSize();

        public abstract void UpdateSelf(GameTime gameTime);
        public virtual void Update(GameTime gameTime)
        {
            UpdateSelf(gameTime);

            foreach (var child in Children)
            {
                if (child != null)
                {
                    child.Update(gameTime);
                }
            }
        }
        public abstract void DrawSelf(SpriteBatch spriteBatch);
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawSelf(spriteBatch);

            foreach (var child in Children)
            {
                if (child != null)
                {
                    child.Draw(spriteBatch);
                }
            }
        }

        public Vector2 GetScreenPosition()
        {
            Vector2 parentPos = Parent == null ? Vector2.Zero : Parent.GetScreenPosition();
            Vector2 parentSize = Parent == null ? Vector2.Zero : Parent.GetSize();

            float maxX = Parent == null ? Globals.CurrentScreenWidth  : parentSize.X;
            float maxY = Parent == null ? Globals.CurrentScreenHeight : parentSize.Y;

            Vector2 pos = new Vector2(XPositionPercent * maxX, YPositionPercent * maxY);

            switch (Anchor)
            {
                case UIAnchor.Center:
                    pos += new Vector2(maxX / 2f, maxY / 2f);
                    break;
                case UIAnchor.Left:
                    pos += new Vector2(0, maxY / 2f);
                    break;
                case UIAnchor.Right:
                    pos += new Vector2(maxX, maxY / 2f);
                    break;
                case UIAnchor.Top:
                    pos += new Vector2(maxX / 2f, 0);
                    break;
                case UIAnchor.Bottom:
                    pos += new Vector2(maxX / 2f, maxY);
                    break;
                case UIAnchor.TopRight:
                    pos += new Vector2(maxX, 0);
                    break;
                case UIAnchor.BottomLeft:
                    pos += new Vector2(0, maxY);
                    break;
                case UIAnchor.BottomRight:
                    pos += new Vector2(maxX, maxY);
                    break;
                default: // Top Left
                    break;
            }

            return pos + (parentPos - (parentSize / 2f));
        }

        protected virtual Vector2 GetCenteredDrawPosition()
        {
            return GetScreenPosition() - (GetSize() / 2f);
        }
    }
}
