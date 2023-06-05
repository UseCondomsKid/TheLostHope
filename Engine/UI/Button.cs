using LostHope.Engine.Input;
using LostHope.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LostHope.Engine.UI
{
    // UI Button Class
    public class Button : Selectable
    {
        // Rectangle that represents the position and size of the button
        private Rectangle _buttonRect;
        // Save rectangle above, but centered. Used for rendering
        private Rectangle _buttonRectAligned;

        // Buttons can have textures
        private Texture2D _buttonTexture;
        // Button font that will be used to render the button text
        private SpriteFont _buttonTextFont;
        // Text
        private string _buttonText;

        // Colors for both the texture and text
        private Color _textureColor;
        private Color _textColor;

        // The position of the button
        private Vector2 _position;


        // Public properties to access the button's private variables
        public Rectangle ButtonRect { get { return _buttonRect; } set { _buttonRect = value; } }
        public Color TextureColor { get { return _textureColor; } set { _textureColor = value; } }
        public Color TextColor { get { return _textColor; } set { _textColor = value; } }

        // Constructor
        public Button(Game1 game, UIManager uiManager, Rectangle buttonRect, UIAnchor anchor = UIAnchor.Center, bool selectOnRegister = true) : base(game, uiManager, anchor, selectOnRegister)
        {
            _buttonRect = buttonRect;

            // Get the position
            _position = SpriteBatchExtensions.GetRectanglePosition(_buttonRect);
            // Set the aligned rect
            _buttonRectAligned = new Rectangle(_position.ToPoint(), _buttonRect.Size);

            // Set texture, font and text to null
            _buttonTexture = null;
            _buttonTextFont = null;
            _buttonText = string.Empty;

            // Initialize colors to white
            _textureColor = Color.White;
            _textColor = Color.White;
        }

        // Sets the texture of the button
        public void SetTexture(Texture2D texture)
        {
            _buttonTexture = texture;
        }
        // Sets the text of the button
        public void SetText(SpriteFont spriteFont, string text)
        {
            _buttonTextFont = spriteFont;
            _buttonText = text;
        }

        // Return true if the mouse if on top of the button
        public override bool IsMouseOnSelectable()
        {
            return _mousePosition.X > _position.X && _mousePosition.X < (_position.X + _buttonRect.Width) &&
                -_mousePosition.Y > _position.Y && -_mousePosition.Y < (_position.Y + _buttonRect.Height);
        }

        public override void Draw(GameTime gameTime)
        {
            // Selectable's BeginSpriteBatch(). Calls SpriteBatch.Begin() with the correct transformation matrix
            // that corresponds to the anchor. Implemetation can be found in Selectable.cs
            BeginSpriteBatch();

            // If a texture was specified, we render it.
            if (_buttonTexture != null)
            {
                Globals.SpriteBatch.Draw(_buttonTexture, _buttonRectAligned, _textureColor);
            }
            // If a font and the text were specified, we render the text.
            if (_buttonTextFont != null && _buttonText != string.Empty)
            {
                Globals.SpriteBatch.DrawString(_buttonTextFont, _buttonText, SpriteBatchExtensions.GetTextPosition(_buttonTextFont, _buttonText, new Vector2(_buttonRect.X, _buttonRect.Y)), TextColor);
            }

            Globals.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
