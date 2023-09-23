﻿using LostHope.Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.UI.Elements
{
    public class UIEButton : Selectable
    {
        private Vector2 _size;
        private Texture2D _texture;

        public UIEButton(UIAnchor anchor, float xPosPercent, float yPosPercent, UIElement parent, Vector2 size,
            Texture2D texture = null, Action onEnter = null, Action onSelect = null, Action onDeselct = null) : base(anchor, xPosPercent, yPosPercent, parent, onEnter, onSelect, onDeselct)
        {
            _size = size;
            SetTexture(texture);
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }

        public override Vector2 GetSize()
        {
            return _size;
        }

        public override void UpdateSelf(GameTime gameTime)
        {
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                var pos = GetCenteredDrawPosition();
                spriteBatch.Draw(_texture, pos, new Rectangle(pos.ToPoint(), _size.ToPoint()), Color.White);
            }
        }
    }
}
