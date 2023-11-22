using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheLostHopeEngine.EngineCode.Inputs;

namespace TheLostHopeEngine.EngineCode.UI
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
        private enum UINavigationDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        private Menu _activeMenu;

        public UIManager()
        {
            InputSystem.Instance.GetAction("UI_Up").OnChange += UI_UpPressed;

            InputSystem.Instance.GetAction("UI_Down").OnChange += UI_DownPressed;

            InputSystem.Instance.GetAction("UI_Left").OnChange += UI_LeftPressed;

            InputSystem.Instance.GetAction("UI_Right").OnChange += UI_RightPressed;

            InputSystem.Instance.GetAction("UI_Enter").OnChange += UI_EnterPressed;

            InputSystem.Instance.GetAction("UI_Back").OnChange += UI_BackPressed;

            InputSystem.Instance.GetAction("UI_Escape").OnChange += UI_EscapePressed;
        }

        private void UI_EscapePressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    _activeMenu.HandleEscape();
                    break;
            }
        }

        private void UI_BackPressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    _activeMenu.HandleBack();
                    break;
            }
        }

        private void UI_EnterPressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    _activeMenu.SelectedSelectable?.Enter();
                    break;
            }
        }

        private void UI_RightPressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    HandleNavigation(UINavigationDirection.Right);
                    break;
            }
        }

        private void UI_LeftPressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    HandleNavigation(UINavigationDirection.Left);
                    break;
            }
        }

        private void UI_DownPressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    HandleNavigation(UINavigationDirection.Down);
                    break;
            }
        }

        private void UI_UpPressed(InputActionContext context)
        {
            switch (context.Phase)
            {
                case InputActionPhase.Started:
                    HandleNavigation(UINavigationDirection.Up);
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_activeMenu == null) return;

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

        private void HandleNavigation(UINavigationDirection navigationDirection)
        {
            if (_activeMenu == null || _activeMenu.Children.Count == 0)
            {
                return;
            }

            int selectedIndex = _activeMenu.Children.IndexOf(_activeMenu.SelectedSelectable);
            int newIndex = FindSelectableInDirection(selectedIndex, navigationDirection);

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

        private int FindSelectableInDirection(int currentIndex, UINavigationDirection direction)
        {
            // Define how to calculate the distance between selectables in the specified direction
            Func<Vector2, Vector2, float> distanceFunction = null;

            switch (direction)
            {
                case UINavigationDirection.Up:
                    distanceFunction = (pos1, pos2) => pos1.Y - pos2.Y;
                    break;
                case UINavigationDirection.Down:
                    distanceFunction = (pos1, pos2) => pos2.Y - pos1.Y;
                    break;
                case UINavigationDirection.Left:
                    distanceFunction = (pos1, pos2) => pos1.X - pos2.X;
                    break;
                case UINavigationDirection.Right:
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
