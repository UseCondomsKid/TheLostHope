using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.Input
{
    // Monogame handles mouse input in a bit of a weird way, so this enum will help with that
    // Right is the right mouse button
    // Left is the left mouse buttom
    // Middle in the scroll whell button
    public enum MouseButton { Right, Left, Middle };

    // This class implements a wrapper around monogame's input system. And makes it easier to work with.
    public class InputManager : GameComponent
    {
        #region Fields
        // The current keyboard state
        private static KeyboardState keyboardState;
        // The previous keyboard state
        private static KeyboardState previousKeyboardState;

        // The current mouse state
        private static MouseState mouseState;
        // The previous mouse state
        private static MouseState previousMouseState;

        // Public fields to access the keyboard states
        public static KeyboardState KeyboardState { get { return keyboardState; } }
        public static KeyboardState PreviousKeyboardState { get { return previousKeyboardState; } }

        // Public fields to access the mouse states
        public static MouseState MouseState { get { return mouseState; } }
        public static MouseState PreviousMouseState { get { return previousMouseState; } }

        // Public events that trigger when a key is pressed
        public static event Action<Keys> OnKeyPressed;
        #endregion

        #region Constructor
        // Constructor
        public InputManager(Game game) : base(game)
        {
            // Get the current keyboard state
            keyboardState = Keyboard.GetState();
            // Get the current mouse state
            mouseState = Mouse.GetState();
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            // Set the previous states to be the states from the previous update tick
            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;

            // Set the current states to the current states
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // Loop through all that keys that are being pressed this frame,
            // and check if it was pressed the last frame. If it wasn't then we trigger the
            // OnKeyPressed event and pass in the key.
            foreach (var key in keyboardState.GetPressedKeys())
            {
                if (KeyPressed(key))
                {
                    OnKeyPressed?.Invoke(key);
                    break;
                }
            }

            base.Update(gameTime);
        }

        // Returns true as long as a key is being held down in the current frame, false otherwise
        public static bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        // Returns true as long as a key was being held down in the previous frame, false otherwise
        public static bool WasKeyDown(Keys key)
        {
            return previousKeyboardState.IsKeyDown(key);
        }
        // Returns true if a key is pressed in the current frame, false otherwise
        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }
        // Returns true if a key is released in the current frame, false otherwise
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key);
        }

        // Returns true as long as a mouse button is being held down in the current frame, false otherwise
        public static bool IsMouseDown(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => mouseState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed,
                _ => false,
            };
        }

        // Returns true as long as a mouse button was being held down in the previous frame, false otherwise
        public static bool WasMouseDown(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => previousMouseState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => previousMouseState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => previousMouseState.MiddleButton == ButtonState.Pressed,
                _ => false,
            };
        }

        // Returns true if a mouse button is pressed in the current frame, false otherwise
        public static bool MousePressed(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed &&
               previousMouseState.LeftButton == ButtonState.Released,
                MouseButton.Right => mouseState.RightButton == ButtonState.Pressed
               && previousMouseState.RightButton == ButtonState.Released,
                MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed
               && previousMouseState.MiddleButton == ButtonState.Released,
                _ => false,
            };
        }

        // Returns true if a mouse button is released in the current frame, false otherwise
        public static bool MouseReleased(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => mouseState.LeftButton == ButtonState.Released &&
               previousMouseState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => mouseState.RightButton == ButtonState.Released
               && previousMouseState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => mouseState.MiddleButton == ButtonState.Released
               && previousMouseState.MiddleButton == ButtonState.Pressed,
                _ => false,
            };
        }

        #endregion
    }
}
