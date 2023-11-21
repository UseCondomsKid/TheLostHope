using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.CustomAttributes;

namespace TheLostHopeEngine.EngineCode.Inputs
{
    public class InputAction
    {
        public string Name { get; set; }
        public List<InputBinding> InputBindings { get; set; } = new List<InputBinding>();
    }

    public abstract class InputBinding
    {
        // This function bellow might never be needed. I'm just testing at this point.
        // Just know that the InputBinding is abstract, and we can do a bunch of stuff with that!
        public abstract InputDeviceType GetDeviceType();
    }

    public class ButtonInputBinding<TButton> : InputBinding where TButton : Enum
    {
        public TButton Button { get; set; }

        public override InputDeviceType GetDeviceType()
        {
            if (typeof(TButton) == typeof(GamepadButton))
            {
                return InputDeviceType.Gamepad;
            }
            else if (typeof(TButton) == typeof(Keys))
            {
                return InputDeviceType.Keyboard;
            }
            else
            {
                return InputDeviceType.None;
            }
        }
    }

    public class AxisInputBinding<TButton> : InputBinding where TButton : Enum
    {
        public TButton PositiveButton { get; set; }
        public TButton NegativeButton { get; set; }
        public float AxisThreshold { get; set; } = 0.5f;

        public override InputDeviceType GetDeviceType()
        {
            if (typeof(TButton) == typeof(GamepadButton))
            {
                return InputDeviceType.Gamepad;
            }
            else if (typeof(TButton) == typeof(Keys))
            {
                return InputDeviceType.Keyboard;
            }
            else
            {
                return InputDeviceType.None;
            }
        }
    }

    public enum InputActionType
    {
        Button,
        Axis
    }
    public enum InputDeviceType
    {
        None,
        Keyboard,
        Gamepad,
    }



    //public class InputAction
    //{
    //    public string Name { get; set; }
    //    public InputActionType Type { get; set; }

    //    public List<KeyboardInputBinding> KeyboardInputBindings { get; set; } = new List<KeyboardInputBinding>();
    //    public List<GamepadInputBinding> GamepadInputBindings { get; set; } = new List<GamepadInputBinding>();
    //}


    //public class KeyboardInputBinding
    //{
    //    // Default Key
    //    public Keys Key { get; set; } = Keys.None;

    //    // Override Key
    //    private Keys _overrideKey = Keys.None;
    //    // Functions to set and get override key
    //    public Keys GetOverrideKey() { return _overrideKey; }
    //    public void SetOverrideKey(Keys overrideKey) { _overrideKey = overrideKey; }
    //}
    //public class GamepadInputBinding
    //{
    //    // Default Key
    //    public GamepadButton Button { get; set; } = GamepadButton.None;

    //    // Override Key
    //    private GamepadButton _overrideKey = GamepadButton.None;
    //    // Functions to set and get override key
    //    public GamepadButton GetOverrideKey() { return _overrideKey; }
    //    public void SetOverrideKey(GamepadButton overrideKey) { _overrideKey = overrideKey; }
    //}

    public enum GamepadButton
    {
        None,
        ButtonEast,
        ButtonWest,
        ButtonNorth,
        ButtonSouth,
        DpadRight,
        DpadLeft,
        DpadUp,
        DpadDown,
        LeftStickButton,
        RightStickButton,
        LeftStickRight,
        LeftStickLeft,
        LeftStickUp,
        LeftStickDown,
        RightStickRight,
        RightStickLeft,
        RightStickUp,
        RightStickDown,
        LeftTrigger,
        LeftShoulder,
        RightTrigger,
        RightShoulder,
        Start,
        Select,
    }


    public class InputActionContext
    {
        private InputAction _action;

        public InputActionContext(InputAction action)
        {
            _action = action;
        }

        public event Action<InputActionContext> OnChange;

        public InputActionPhase Phase { get; private set; }

        public float GetValue()
        {
            // Logic to get the input value based on the action type and bindings
            // This is where you would check if the value changed and trigger OnChange event
            return 0f; // Placeholder value, replace with actual logic
        }
    }
    public enum InputActionPhase
    {
        Started,
        Pressed,
        Stopped
    }


    public class InputSystem
    {
        private static InputSystem instance;

        public static InputSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputSystem();
                }
                return instance;
            }
        }

        private InputAsset _inputAsset;
        //private InputDevice _currentInputDevice;

        //public event Action<InputDevice> OnInputDeviceChanged;

        private bool _isInitialized = false;

        private InputSystem()
        {
            // Private constructor to enforce singleton pattern
            _isInitialized = false;
        }


        public void Initialize(InputAsset inputAsset)
        {
            _inputAsset = inputAsset;
            _isInitialized = _inputAsset != null;
        }

        public void Update()
        {
            // TODO: How to update states?
        }

        public InputActionContext GetAction(string actionName)
        {
            // TODO: Check if isInitialized

            var action = _inputAsset.Actions.FirstOrDefault(a => a.Name == actionName);

            // TODO: Figure out how to create the InputActionContext
            return new InputActionContext(action);
        }

        public void AddBindingOverride(string actionName/*, ???*/)
        {
            // TODO: Check if isInitialized

            // TODO: Implement this later
        }

        public void RemoveBindingOverride(string actionName)
        {
            // TODO: Check if isInitialized

            // TODO: Implement this later
        }
    }
}
