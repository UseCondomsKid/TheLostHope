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
            if (binding is ButtonInputBinding<KeyboardKey>)
            {
                var buttonBinding = (ButtonInputBinding<KeyboardKey>)binding;
                HandleButtonBinding(buttonBinding, runtimeInputAction);
            }
            else if (binding is AxisInputBinding<KeyboardKey>)
            {
                var axisBinding = (AxisInputBinding<KeyboardKey>)binding;
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

        private void HandleButtonBinding<TButton>(ButtonInputBinding<TButton> buttonBinding, RuntimeInputAction runtimeInputAction)
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

        private void HandleAxisBinding<TButton>(AxisInputBinding<TButton> axisBinding, RuntimeInputAction runtimeInputAction)
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

        private bool GetCurrentButtonState<TButton>(TButton button)
        {
            if (typeof(TButton) == typeof(KeyboardKey))
            {
                // Check the current state of the keyboard key
                return _currentKeyboardState.IsKeyDown(ConvertToXnaKeys((KeyboardKey)(object)button));
            }
            else if (typeof(TButton) == typeof(GamepadButton))
            {
                // Check the current state of the gamepad button
                return _currentGamePadState.IsButtonDown(ConvertToXnaButtons((GamepadButton)(object)button));
            }
            // Handle other input types if needed

            return false;
        }

        private bool GetPreviousButtonState<TButton>(TButton button)
        {
            if (typeof(TButton) == typeof(KeyboardKey))
            {
                // Check the previous state of the keyboard key
                return _previousKeyboardState.IsKeyDown(ConvertToXnaKeys((KeyboardKey)(object)button));
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

        private Keys ConvertToXnaKeys(KeyboardKey key)
        {
            switch (key)
            {
                case KeyboardKey.Back: return Keys.Back;
                case KeyboardKey.Tab: return Keys.Tab;
                case KeyboardKey.Enter: return Keys.Enter;
                case KeyboardKey.CapsLock: return Keys.CapsLock;
                case KeyboardKey.Escape: return Keys.Escape;
                case KeyboardKey.Space: return Keys.Space;
                case KeyboardKey.PageUp: return Keys.PageUp;
                case KeyboardKey.PageDown: return Keys.PageDown;
                case KeyboardKey.End: return Keys.End;
                case KeyboardKey.Home: return Keys.Home;
                case KeyboardKey.Left: return Keys.Left;
                case KeyboardKey.Up: return Keys.Up;
                case KeyboardKey.Right: return Keys.Right;
                case KeyboardKey.Down: return Keys.Down;
                case KeyboardKey.Select: return Keys.Select;
                case KeyboardKey.Print: return Keys.Print;
                case KeyboardKey.Execute: return Keys.Execute;
                case KeyboardKey.PrintScreen: return Keys.PrintScreen;
                case KeyboardKey.Insert: return Keys.Insert;
                case KeyboardKey.Delete: return Keys.Delete;
                case KeyboardKey.Help: return Keys.Help;
                case KeyboardKey.D0: return Keys.D0;
                case KeyboardKey.D1: return Keys.D1;
                case KeyboardKey.D2: return Keys.D2;
                case KeyboardKey.D3: return Keys.D3;
                case KeyboardKey.D4: return Keys.D4;
                case KeyboardKey.D5: return Keys.D5;
                case KeyboardKey.D6: return Keys.D6;
                case KeyboardKey.D7: return Keys.D7;
                case KeyboardKey.D8: return Keys.D8;
                case KeyboardKey.D9: return Keys.D9;
                case KeyboardKey.A: return Keys.A;
                case KeyboardKey.B: return Keys.B;
                case KeyboardKey.C: return Keys.C;
                case KeyboardKey.D: return Keys.D;
                case KeyboardKey.E: return Keys.E;
                case KeyboardKey.F: return Keys.F;
                case KeyboardKey.G: return Keys.G;
                case KeyboardKey.H: return Keys.H;
                case KeyboardKey.I: return Keys.I;
                case KeyboardKey.J: return Keys.J;
                case KeyboardKey.K: return Keys.K;
                case KeyboardKey.L: return Keys.L;
                case KeyboardKey.M: return Keys.M;
                case KeyboardKey.N: return Keys.N;
                case KeyboardKey.O: return Keys.O;
                case KeyboardKey.P: return Keys.P;
                case KeyboardKey.Q: return Keys.Q;
                case KeyboardKey.R: return Keys.R;
                case KeyboardKey.S: return Keys.S;
                case KeyboardKey.T: return Keys.T;
                case KeyboardKey.U: return Keys.U;
                case KeyboardKey.V: return Keys.V;
                case KeyboardKey.W: return Keys.W;
                case KeyboardKey.X: return Keys.X;
                case KeyboardKey.Y: return Keys.Y;
                case KeyboardKey.Z: return Keys.Z;
                case KeyboardKey.LeftWindows: return Keys.LeftWindows;
                case KeyboardKey.RightWindows: return Keys.RightWindows;
                case KeyboardKey.Apps: return Keys.Apps;
                case KeyboardKey.Sleep: return Keys.Sleep;
                case KeyboardKey.NumPad0: return Keys.NumPad0;
                case KeyboardKey.NumPad1: return Keys.NumPad1;
                case KeyboardKey.NumPad2: return Keys.NumPad2;
                case KeyboardKey.NumPad3: return Keys.NumPad3;
                case KeyboardKey.NumPad4: return Keys.NumPad4;
                case KeyboardKey.NumPad5: return Keys.NumPad5;
                case KeyboardKey.NumPad6: return Keys.NumPad6;
                case KeyboardKey.NumPad7: return Keys.NumPad7;
                case KeyboardKey.NumPad8: return Keys.NumPad8;
                case KeyboardKey.NumPad9: return Keys.NumPad9;
                case KeyboardKey.Multiply: return Keys.Multiply;
                case KeyboardKey.Add: return Keys.Add;
                case KeyboardKey.Separator: return Keys.Separator;
                case KeyboardKey.Subtract: return Keys.Subtract;
                case KeyboardKey.Decimal: return Keys.Decimal;
                case KeyboardKey.Divide: return Keys.Divide;
                case KeyboardKey.F1: return Keys.F1;
                case KeyboardKey.F2: return Keys.F2;
                case KeyboardKey.F3: return Keys.F3;
                case KeyboardKey.F4: return Keys.F4;
                case KeyboardKey.F5: return Keys.F5;
                case KeyboardKey.F6: return Keys.F6;
                case KeyboardKey.F7: return Keys.F7;
                case KeyboardKey.F8: return Keys.F8;
                case KeyboardKey.F9: return Keys.F9;
                case KeyboardKey.F10: return Keys.F10;
                case KeyboardKey.F11: return Keys.F11;
                case KeyboardKey.F12: return Keys.F12;
                case KeyboardKey.F13: return Keys.F13;
                case KeyboardKey.F14: return Keys.F14;
                case KeyboardKey.F15: return Keys.F15;
                case KeyboardKey.F16: return Keys.F16;
                case KeyboardKey.F17: return Keys.F17;
                case KeyboardKey.F18: return Keys.F18;
                case KeyboardKey.F19: return Keys.F19;
                case KeyboardKey.F20: return Keys.F20;
                case KeyboardKey.F21: return Keys.F21;
                case KeyboardKey.F22: return Keys.F22;
                case KeyboardKey.F23: return Keys.F23;
                case KeyboardKey.F24: return Keys.F24;
                case KeyboardKey.NumLock: return Keys.NumLock;
                case KeyboardKey.Scroll: return Keys.Scroll;
                case KeyboardKey.LeftShift: return Keys.LeftShift;
                case KeyboardKey.RightShift: return Keys.RightShift;
                case KeyboardKey.LeftControl: return Keys.LeftControl;
                case KeyboardKey.RightControl: return Keys.RightControl;
                case KeyboardKey.LeftAlt: return Keys.LeftAlt;
                case KeyboardKey.RightAlt: return Keys.RightAlt;
                case KeyboardKey.BrowserBack: return Keys.BrowserBack;
                case KeyboardKey.BrowserForward: return Keys.BrowserForward;
                case KeyboardKey.BrowserRefresh: return Keys.BrowserRefresh;
                case KeyboardKey.BrowserStop: return Keys.BrowserStop;
                case KeyboardKey.BrowserSearch: return Keys.BrowserSearch;
                case KeyboardKey.BrowserFavorites: return Keys.BrowserFavorites;
                case KeyboardKey.BrowserHome: return Keys.BrowserHome;
                case KeyboardKey.VolumeMute: return Keys.VolumeMute;
                case KeyboardKey.VolumeDown: return Keys.VolumeDown;
                case KeyboardKey.VolumeUp: return Keys.VolumeUp;
                case KeyboardKey.MediaNextTrack: return Keys.MediaNextTrack;
                case KeyboardKey.MediaPreviousTrack: return Keys.MediaPreviousTrack;
                case KeyboardKey.MediaStop: return Keys.MediaStop;
                case KeyboardKey.MediaPlayPause: return Keys.MediaPlayPause;
                case KeyboardKey.LaunchMail: return Keys.LaunchMail;
                case KeyboardKey.SelectMedia: return Keys.SelectMedia;
                case KeyboardKey.LaunchApplication1: return Keys.LaunchApplication1;
                case KeyboardKey.LaunchApplication2: return Keys.LaunchApplication2;
                case KeyboardKey.OemSemicolon: return Keys.OemSemicolon;
                case KeyboardKey.OemPlus: return Keys.OemPlus;
                case KeyboardKey.OemComma: return Keys.OemComma;
                case KeyboardKey.OemMinus: return Keys.OemMinus;
                case KeyboardKey.OemPeriod: return Keys.OemPeriod;
                case KeyboardKey.OemQuestion: return Keys.OemQuestion;
                case KeyboardKey.OemTilde: return Keys.OemTilde;
                case KeyboardKey.OemOpenBrackets: return Keys.OemOpenBrackets;
                case KeyboardKey.OemPipe: return Keys.OemPipe;
                case KeyboardKey.OemCloseBrackets: return Keys.OemCloseBrackets;
                case KeyboardKey.OemQuotes: return Keys.OemQuotes;
                case KeyboardKey.Oem8: return Keys.Oem8;
                case KeyboardKey.OemBackslash: return Keys.OemBackslash;
                case KeyboardKey.ProcessKey: return Keys.ProcessKey;
                case KeyboardKey.Attn: return Keys.Attn;
                case KeyboardKey.Crsel: return Keys.Crsel;
                case KeyboardKey.Exsel: return Keys.Exsel;
                case KeyboardKey.EraseEof: return Keys.EraseEof;
                case KeyboardKey.Play: return Keys.Play;
                case KeyboardKey.Zoom: return Keys.Zoom;
                case KeyboardKey.Pa1: return Keys.Pa1;
                case KeyboardKey.OemClear: return Keys.OemClear;
                case KeyboardKey.ChatPadGreen: return Keys.ChatPadGreen;
                case KeyboardKey.ChatPadOrange: return Keys.ChatPadOrange;
                case KeyboardKey.Pause: return Keys.Pause;
                case KeyboardKey.ImeConvert: return Keys.ImeConvert;
                case KeyboardKey.ImeNoConvert: return Keys.ImeNoConvert;
                case KeyboardKey.Kana: return Keys.Kana;
                case KeyboardKey.Kanji: return Keys.Kanji;
                case KeyboardKey.OemAuto: return Keys.OemAuto;
                case KeyboardKey.OemCopy: return Keys.OemCopy;
                case KeyboardKey.OemEnlW: return Keys.OemEnlW;
                default: return Keys.None;
            }
        }

    }

    public abstract class InputBinding
    {
        public abstract void Accept(IInputBindingVisitor visitor, RuntimeInputAction runtimeInputAction);
    }

    public class ButtonInputBinding<TButton> : InputBinding
    {
        public TButton Button { get; set; }

        public override void Accept(IInputBindingVisitor visitor, RuntimeInputAction runtimeInputAction)
        {
            visitor.Visit(this, runtimeInputAction);
        }
    }

    public class AxisInputBinding<TButton> : InputBinding
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


        // TODO: Implement these:
        //private InputDeviceType _currentInputDevice;
        //public event Action<InputDeviceType> OnInputDeviceChanged;

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
                throw new Exception("Input System was not initialized.");
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
                throw new Exception("Input System was not initialized.");
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

    public enum KeyboardKey
    {
        None,
        Back,
        Tab,
        Enter,
        CapsLock,
        Escape,
        Space,
        PageUp,
        PageDown,
        End,
        Home,
        Left,
        Up,
        Right,
        Down,
        Select,
        Print,
        Execute,
        PrintScreen,
        Insert,
        Delete,
        Help,
        D0,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        LeftWindows,
        RightWindows,
        Apps,
        Sleep,
        NumPad0,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,
        Multiply,
        Add,
        Separator,
        Subtract,
        Decimal,
        Divide,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F20,
        F21,
        F22,
        F23,
        F24,
        NumLock,
        Scroll,
        LeftShift,
        RightShift,
        LeftControl,
        RightControl,
        LeftAlt,
        RightAlt,
        BrowserBack,
        BrowserForward,
        BrowserRefresh,
        BrowserStop,
        BrowserSearch,
        BrowserFavorites,
        BrowserHome,
        VolumeMute,
        VolumeDown,
        VolumeUp,
        MediaNextTrack,
        MediaPreviousTrack,
        MediaStop,
        MediaPlayPause,
        LaunchMail,
        SelectMedia,
        LaunchApplication1,
        LaunchApplication2,
        OemSemicolon,
        OemPlus,
        OemComma,
        OemMinus,
        OemPeriod,
        OemQuestion,
        OemTilde,
        OemOpenBrackets,
        OemPipe,
        OemCloseBrackets,
        OemQuotes,
        Oem8,
        OemBackslash,
        ProcessKey,
        Attn,
        Crsel,
        Exsel,
        EraseEof,
        Play,
        Zoom,
        Pa1,
        OemClear,
        ChatPadGreen,
        ChatPadOrange,
        Pause,
        ImeConvert,
        ImeNoConvert,
        Kana,
        Kanji,
        OemAuto,
        OemCopy,
        OemEnlW
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
