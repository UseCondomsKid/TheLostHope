using Microsoft.Xna.Framework;

namespace LostHope.Engine.UI
{
    public abstract class UIElement
    {
        // The anchor. This allows UI elements to be anchored to different locations to the screen.
        // Check the UIManager.cs script for more info.
        protected UIAnchor _anchor;
        // Reference to the UI manager.
        protected UIManager _manager;
        // Public property that references the selectable's anchor
        public UIAnchor Anchor { get { return _anchor; }  set { _anchor = value; } }

        public UIElement(UIManager uiManager, UIAnchor anchor = UIAnchor.Center)
        {
            _manager = uiManager;
            _anchor = anchor;

            _manager.RegisterUIElement(this);
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        public Matrix GetMatrix() => _manager.GetMatrixFromAnchor(_anchor);
    }
}
