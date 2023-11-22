using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Pooling;

namespace TheLostHopeEngine.EngineCode.Inputs
{
    public class InputAction
    {
        public string Name { get; set; }
        public List<InputBinding> InputBindings { get; set; } = new List<InputBinding>();
    }

    public interface IInputBindingVisitor
    {
        void Visit(InputBinding binding, RuntimeInputAction runtimeInputAction);
    }

    public class InputBindingVisitor : IInputBindingVisitor
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private GamePadState _currentGamePadState;
        private GamePadState _previousGamePadState;

        private ObjectPool<InputActionContext> _contextPool;
        private List<InputActionContext> _usedContexts;

        public InputBindingVisitor()
        {
            _usedContexts = new List<InputActionContext>();
            _contextPool = new ObjectPool<InputActionContext>
                (
                    objectCreate: () =>
                    {
                        var ictx = new InputActionContext();
                        _usedContexts.Add(ictx);
                        return ictx;
                    },
                    objectRelease: (ictx) =>
                    {
                        ictx.Set(InputActionPhase.None, -100f);
                    },
                    maxSize: 50
                );
        }

        public void UpdateStates(KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {
            // Update the current and previous states
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = currentKeyboardState;

            _previousGamePadState = _currentGamePadState;
            _currentGamePadState = currentGamePadState;

            foreach (var usedIctx in _usedContexts)
            {
                _contextPool.ReturnObject(usedIctx);
            }
            _usedContexts.Clear();
        }

        public void Visit(InputBinding binding, RuntimeInputAction runtimeInputAction)
        {
            if (binding is ButtonInputBinding<Keys>)
            {
                var buttonBinding = (ButtonInputBinding<Keys>)binding;
                HandleButtonBinding(buttonBinding, runtimeInputAction);
            }
            else if (binding is AxisInputBinding<Keys>)
            {
                var axisBinding = (AxisInputBinding<Keys>)binding;
                HandleAxisBinding(axisBinding, runtimeInputAction);
            }
            else if (binding is ButtonInputBinding<GamepadButton>)
            {
                var buttonBinding = (ButtonInputBinding<GamepadButton>)binding;
                HandleButtonBinding(buttonBinding, runtimeInputAction);
            }
            else if (binding is AxisInputBinding<GamepadButton>)
            {
                var axisBinding = (AxisInputBinding<GamepadButton>)binding;
                HandleAxisBinding(axisBinding, runtimeInputAction);
            }
        }

        private void HandleButtonBinding<TButton>(ButtonInputBinding<TButton> buttonBinding, RuntimeInputAction runtimeInputAction) where TButton : Enum
        {
            // Get the current and previous state of the button
            var currentButtonState = GetCurrentButtonState(buttonBinding.Button);
            var previousButtonState = GetPreviousButtonState(buttonBinding.Button);

            // Check if the button is pressed or released
            if (currentButtonState && !previousButtonState)
            {
                // Button pressed
                var ictx = _contextPool.GetObject();
                ictx.Set(InputActionPhase.Started, 1f);
                runtimeInputAction.InvokeOnChange(ictx);
            }
            else if (currentButtonState && previousButtonState)
            {
                // Button is held
                var ictx = _contextPool.GetObject();
                ictx.Set(InputActionPhase.Held, 1f);
                runtimeInputAction.InvokeOnChange(ictx);
            }
            else if (!currentButtonState && previousButtonState)
            {
                // Button released
                var ictx = _contextPool.GetObject();
                ictx.Set(InputActionPhase.Released, 0f);
                runtimeInputAction.InvokeOnChange(ictx);
            }
        }

        private void HandleAxisBinding<TButton>(AxisInputBinding<TButton> axisBinding, RuntimeInputAction runtimeInputAction) where TButton : Enum
        {
            // Get the current state of the positive and negative buttons
            var currentPositiveButtonState = GetCurrentButtonState(axisBinding.PositiveButton);
            var currentNegativeButtonState = GetCurrentButtonState(axisBinding.NegativeButton);

            // Get the previous state of the positive and negative buttons
            var previousPositiveButtonState = GetPreviousButtonState(axisBinding.PositiveButton);
            var previousNegativeButtonState = GetPreviousButtonState(axisBinding.NegativeButton);

            float value = (currentPositiveButtonState ? 1f : 0f) + (currentNegativeButtonState ? -1f : 0f);

            // Check if the axis is pressed or released
            if ((currentPositiveButtonState && !previousPositiveButtonState) ||
                (currentNegativeButtonState && !previousNegativeButtonState))
            {
                // Axis pressed
                var ictx = _contextPool.GetObject();
                ictx.Set(InputActionPhase.Started, value);
                runtimeInputAction.InvokeOnChange(ictx);
            }
            if ((currentPositiveButtonState && previousPositiveButtonState) ||
                (currentNegativeButtonState && previousNegativeButtonState))
            {
                // Axis held
                var ictx = _contextPool.GetObject();
                ictx.Set(InputActionPhase.Held, value);
                runtimeInputAction.InvokeOnChange(ictx);
            }
            else if ((!currentPositiveButtonState && previousPositiveButtonState) ||
                     (!currentNegativeButtonState && previousNegativeButtonState))
            {
                // Axis released
                var ictx = _contextPool.GetObject();
                ictx.Set(InputActionPhase.Released, value);
                runtimeInputAction.InvokeOnChange(ictx);
            }
        }

        private bool GetCurrentButtonState<TButton>(TButton button) where TButton : Enum
        {
            if (typeof(TButton) == typeof(Keys))
            {
                // Check the current state of the keyboard key
                return _currentKeyboardState.IsKeyDown((Keys)(object)button + 21);
            }
            else if (typeof(TButton) == typeof(GamepadButton))
            {
                // Check the current state of the gamepad button
                return _currentGamePadState.IsButtonDown(ConvertToXnaButtons((GamepadButton)(object)button));
            }
            // Handle other input types if needed

            return false;
        }

        private bool GetPreviousButtonState<TButton>(TButton button) where TButton : Enum
        {
            if (typeof(TButton) == typeof(Keys))
            {
                // Check the previous state of the keyboard key
                return _previousKeyboardState.IsKeyDown((Keys)(object)button + 21);
            }
            else if (typeof(TButton) == typeof(GamepadButton))
            {
                // Check the previous state of the gamepad button
                return _previousGamePadState.IsButtonDown(ConvertToXnaButtons((GamepadButton)(object)button));
            }
            // Handle other input types if needed

            return false;
        }


        private Buttons ConvertToXnaButtons(GamepadButton button)
        {
            switch (button)
            {
                case GamepadButton.ButtonEast:
                    return Buttons.B;
                case GamepadButton.ButtonWest:
                    return Buttons.X;
                case GamepadButton.ButtonNorth:
                    return Buttons.Y;
                case GamepadButton.ButtonSouth:
                    return Buttons.A;
                case GamepadButton.RightShoulder:
                    return Buttons.RightShoulder;
                case GamepadButton.LeftShoulder:
                    return Buttons.LeftShoulder;
                case GamepadButton.RightTrigger:
                    return Buttons.RightTrigger;
                case GamepadButton.LeftTrigger:
                    return Buttons.LeftTrigger;
                case GamepadButton.DpadRight:
                    return Buttons.DPadRight;
                case GamepadButton.DpadLeft:
                    return Buttons.DPadLeft;
                case GamepadButton.DpadUp:
                    return Buttons.DPadUp;
                case GamepadButton.DpadDown:
                    return Buttons.DPadDown;
                case GamepadButton.RightStickButton:
                    return Buttons.RightStick;
                case GamepadButton.LeftStickButton:
                    return Buttons.LeftStick;
                case GamepadButton.RightStickUp:
                    return Buttons.RightThumbstickUp;
                case GamepadButton.RightStickDown:
                    return Buttons.RightThumbstickDown;
                case GamepadButton.RightStickLeft:
                    return Buttons.RightThumbstickLeft;
                case GamepadButton.RightStickRight:
                    return Buttons.RightThumbstickRight;
                case GamepadButton.LeftStickUp:
                    return Buttons.LeftThumbstickUp;
                case GamepadButton.LeftStickDown:
                    return Buttons.LeftThumbstickDown;
                case GamepadButton.LeftStickLeft:
                    return Buttons.LeftThumbstickLeft;
                case GamepadButton.LeftStickRight:
                    return Buttons.LeftThumbstickRight;
                case GamepadButton.Start:
                    return Buttons.Start;
                case GamepadButton.Select:
                    return Buttons.Back;
                default:
                    return Buttons.None;
            }
        }
    }

    public abstract class InputBinding
    {
        public abstract void Accept(IInputBindingVisitor visitor, RuntimeInputAction runtimeInputAction);
    }

    public class ButtonInputBinding<TButton> : InputBinding where TButton : Enum
    {
        public TButton Button { get; set; }

        public override void Accept(IInputBindingVisitor visitor, RuntimeInputAction runtimeInputAction)
        {
            visitor.Visit(this, runtimeInputAction);
        }
    }

    public class AxisInputBinding<TButton> : InputBinding where TButton : Enum
    {
        public TButton PositiveButton { get; set; }
        public TButton NegativeButton { get; set; }
        public float AxisThreshold { get; set; } = 0.1f;

        public override void Accept(IInputBindingVisitor visitor, RuntimeInputAction runtimeInputAction)
        {
            visitor.Visit(this, runtimeInputAction);
        }
    }

    public class InputActionContext
    {
        private InputActionPhase _phase;
        private float _value;

        public InputActionPhase Phase { get { return _phase; } }
        public float Value { get { return _value; } }

        public InputActionContext()
        {
            _phase = InputActionPhase.None;
            _value = -100f;
        }

        public void Set(InputActionPhase phase, float value)
        {
            _phase = phase;
            _value = value;
        }
    }

    public class RuntimeInputAction
    {
        public event Action<InputActionContext> OnChange;

        public RuntimeInputAction()
        {
        }

        // Invoke this method when the input state changes
        public void InvokeOnChange(InputActionContext context)
        {
            OnChange?.Invoke(context);
        }
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

        private Dictionary<string, List<InputBinding>> _actionBindingsMap = new Dictionary<string, List<InputBinding>>();
        private Dictionary<string, RuntimeInputAction> _runtimeActionsMap = new Dictionary<string, RuntimeInputAction>();
        private InputBindingVisitor _inputBindingVisitor;

        private InputDeviceType _currentInputDevice;
        public event Action<InputDeviceType> OnInputDeviceChanged;

        private bool _isInitialized = false;

        private KeyboardState _currentKeyboardState;

        private GamePadCapabilities _currentGamePadCapabilities;
        private GamePadCapabilities _previousGamePadCapabilities;
        private GamePadState _currentGamePadState;

        private InputSystem()
        {
            // Private constructor to enforce singleton pattern
            _isInitialized = false;
        }


        public void Initialize(InputAsset inputAsset)
        {
            _inputAsset = inputAsset;
            _inputBindingVisitor = new InputBindingVisitor();

            // Build the actionBindingsMap
            foreach (var action in _inputAsset.Actions)
            {
                var bindings = action.InputBindings;
                _actionBindingsMap.Add(action.Name, bindings);
                _runtimeActionsMap.Add(action.Name, new RuntimeInputAction());
            }

            _isInitialized = _inputAsset != null;
        }

        public RuntimeInputAction GetAction(string actionName)
        {
            if (!_isInitialized)
            {
                // Handle uninitialized state
                return null;
            }

            if (_runtimeActionsMap.TryGetValue(actionName, out var action))
            {
                return action;
            }

            return null;
        }

        public void Update()
        {
            if (!_isInitialized)
            {
                // Handle uninitialized state
                return;
            }

            // Get States
            _currentKeyboardState = Keyboard.GetState();
            _previousGamePadCapabilities = _currentGamePadCapabilities;
            _currentGamePadCapabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);

            // If there is a gamepad attached, handle it
            if (_currentGamePadCapabilities.IsConnected)
            {
                if (!_previousGamePadCapabilities.IsConnected)
                {
                    // THe gamepad was just connected
                    Debug.WriteLine("Gamepad Connected");
                }

                _currentGamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            }
            else if (_previousGamePadCapabilities.IsConnected)
            {
                Debug.WriteLine("Gamepad Disconnected");
                // Here the gamepad was connected the previous frame, but is not this frame.
                // Aka the gamepad was just disconnected.
            }

            // Update states in the InputBindingVisitor
            _inputBindingVisitor.UpdateStates(_currentKeyboardState, _currentGamePadState);

            // Loop through the bindings
            foreach (var actionName in _actionBindingsMap.Keys)
            {
                var bindings = _actionBindingsMap[actionName];
                var runtimeAction = _runtimeActionsMap[actionName];

                foreach (var binding in bindings)
                {
                    binding.Accept(_inputBindingVisitor, runtimeAction);
                }
            }
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

    public enum InputActionPhase
    {
        None,
        Started,
        Held,
        Released
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
}
