using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheLostHopeEngine.EngineCode.UI
{
    public class Menu : UIElement
    {
        public Selectable SelectedSelectable { get; private set; }

        private event Action _onBack;
        private event Action _onEscape;


        public Menu(GraphicsDevice graphicsDevice, Action onBack = null, Action onEscape = null) : base(graphicsDevice, UIAnchor.Center, 0f, 0f, null)
        {
            _onBack = onBack;
            _onEscape = onEscape;
        }


        public void HandleBack()
        {
            _onBack?.Invoke();
        }
        public void HandleEscape()
        {
            _onEscape?.Invoke();
        }

        public void SetSelected(Selectable selectable)
        {
            SelectedSelectable = selectable;
        }


        public override Vector2 GetSize()
        {
            return new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        }

        public override void UpdateSelf(GameTime gameTime)
        {
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
        }
    }
}
