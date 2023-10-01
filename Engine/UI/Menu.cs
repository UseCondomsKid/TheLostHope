using TheLostHope.GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheLostHope.Engine.UI
{
    public class Menu : UIElement
    {
        public Selectable SelectedSelectable { get; private set; }
        private event Action _onBack;
        private event Action _onEscape;


        public Menu(Action onBack = null, Action onEscape = null) : base(UIAnchor.Center, 0f, 0f, null)
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
            return new Vector2(Globals.CurrentScreenWidth, Globals.CurrentScreenHeight);
        }

        public override void UpdateSelf(GameTime gameTime)
        {
        }
        public override void DrawSelf(SpriteBatch spriteBatch)
        {
        }
    }
}
