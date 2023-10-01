using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheLostHope.Engine.UI
{
    public enum UIAnchor
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    public class UIManager
    {
        private enum NavigationDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        private Menu _activeMenu;

        // Input
        private ICondition _up;
        private ICondition _down;
        private ICondition _left;
        private ICondition _right;
        private ICondition _enter;
        private ICondition _back;
        private ICondition _escape;

        public UIManager()
        {
            _up = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Up),
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.W),
                new GamePadCondition(GamePadButton.Up, 0)
                );

            _down = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Down),
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.S),
                new GamePadCondition(GamePadButton.Down, 0)
                );

            _left = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Left),
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.A),
                new GamePadCondition(GamePadButton.Left, 0)
                );

            _right = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Right),
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.D),
                new GamePadCondition(GamePadButton.Right, 0)
                );

            _enter = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Enter),
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Space),
                new GamePadCondition(GamePadButton.A, 0)
                );

            _back = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Escape),
                new GamePadCondition(GamePadButton.B, 0)
                );

            _escape = new AnyCondition(
                new KeyboardCondition(Microsoft.Xna.Framework.Input.Keys.Escape),
                new GamePadCondition(GamePadButton.Start, 0)
                );
        }

        public void Update(GameTime gameTime)
        {
            if (_activeMenu == null) return;

            HandleNavigation();
            _activeMenu?.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_activeMenu == null) return;

            spriteBatch.Begin();
            _activeMenu?.Draw(spriteBatch);
            spriteBatch.End();
        }


        public void SetActiveMenu(Menu menu)
        {
            _activeMenu = menu;
            if (_activeMenu != null && _activeMenu.Children.Count > 0)
            {

                if (_activeMenu.SelectedSelectable != null)
                {
                    _activeMenu.SelectedSelectable.Select();
                }
            }
        }

        public void HandleNavigation()
        {
            if (_activeMenu == null || _activeMenu.Children.Count == 0)
            {
                return;
            }

            int selectedIndex = _activeMenu.Children.IndexOf(_activeMenu.SelectedSelectable);
            int newIndex = selectedIndex;

            if (_activeMenu.SelectedSelectable != null)
            {
                if (_up.Pressed())
                {
                    newIndex = FindSelectableInDirection(selectedIndex, NavigationDirection.Up);
                }
                else if (_down.Pressed())
                {
                    newIndex = FindSelectableInDirection(selectedIndex, NavigationDirection.Down);
                }
                else if (_left.Pressed())
                {
                    newIndex = FindSelectableInDirection(selectedIndex, NavigationDirection.Left);
                }
                else if (_right.Pressed())
                {
                    newIndex = FindSelectableInDirection(selectedIndex, NavigationDirection.Right);
                }
            }

            if (_enter.Pressed())
            {
                // Call Enter function on the selectedSelectable
                _activeMenu.SelectedSelectable?.Enter();
            }

            if (_escape.Pressed())
            {
                // Handle Escape (e.g., go back to the previous menu or perform menu-specific action)
                _activeMenu.HandleEscape();
            }
            else if (_back.Pressed())
            {
                // Handle Back (e.g., go back to the previous menu or perform menu-specific action)
                _activeMenu.HandleBack();
            }

            if (newIndex != selectedIndex)
            {
                _activeMenu.SelectedSelectable.Deselect();
                _activeMenu.SetSelected(_activeMenu.Children[newIndex] as Selectable);
                if (_activeMenu.SelectedSelectable != null)
                {
                    _activeMenu.SelectedSelectable.Select();
                }
            }
        }
        private int FindSelectableInDirection(int currentIndex, NavigationDirection direction)
        {
            // Define how to calculate the distance between selectables in the specified direction
            Func<Vector2, Vector2, float> distanceFunction = null;

            switch (direction)
            {
                case NavigationDirection.Up:
                    distanceFunction = (pos1, pos2) => pos1.Y - pos2.Y;
                    break;
                case NavigationDirection.Down:
                    distanceFunction = (pos1, pos2) => pos2.Y - pos1.Y;
                    break;
                case NavigationDirection.Left:
                    distanceFunction = (pos1, pos2) => pos1.X - pos2.X;
                    break;
                case NavigationDirection.Right:
                    distanceFunction = (pos1, pos2) => pos2.X - pos1.X;
                    break;
            }

            float minDistance = float.MaxValue;
            int newIndex = currentIndex;

            for (int i = 0; i < _activeMenu.Children.Count; i++)
            {
                if (i != currentIndex)
                {
                    Selectable selectable = _activeMenu.Children[i] as Selectable;
                    if (selectable != null)
                    {
                        float distance = distanceFunction(_activeMenu.SelectedSelectable.GetScreenPosition(),
                            selectable.GetScreenPosition());

                        if (distance > 0 && distance < minDistance)
                        {
                            minDistance = distance;
                            newIndex = i;
                        }
                    }
                }
            }

            return newIndex;
        }
    }
}
