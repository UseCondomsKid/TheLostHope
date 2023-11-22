using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEditor.EditorCode.StateManagement;
using TheLostHopeEditor.EditorCode.States.SuperStates;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.Inputs;

namespace TheLostHopeEditor.EditorCode.States.SubStates
{
    public class InputsEditorState : AssetEditorState
    {
        private bool _newInputActionWindow = false;

        private string _inputActionName = "";
        private InputDeviceType _inputDeviceType = InputDeviceType.None;
        private InputActionType _inputActionType = InputActionType.Button;

        public InputsEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
            _newInputActionWindow = false;
            _inputDeviceType = InputDeviceType.None;
            _inputActionType = InputActionType.Button;
            _inputActionName = "";
        }

        public override void DrawGame(GameTime gameTime)
        {
        }
        protected override void DrawImGuiTool()
        {
            if (_editorAssetManager.Asset != null)
            {
                string windowStatus = _newInputActionWindow ? "Open" : "Closed";
                if (ImGui.Button($"Add New Input Action ({windowStatus})"))
                {
                    _newInputActionWindow = !_newInputActionWindow;
                }

                if (_newInputActionWindow)
                {
                    ImGui.Indent();

                    ImGui.InputText("Name of the Input Action", ref _inputActionName, 255);

                    var inputAsset = (InputAsset)_editorAssetManager.Asset;
                    var action = inputAsset.Actions.Find(a => a.Name == _inputActionName);

                    if (action == null)
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(1f, 0f, 0f, 1f),
                            $"Can't find an action with the name: {_inputActionName}");
                    }
                    else
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(0f, 1f, 0f, 1f),
                            "Action Found!");
                    }

                    int selectedDeviceTypeIndex = Convert.ToInt32(_inputDeviceType);
                    string[] deviceTypeNames = Enum.GetNames(typeof(InputDeviceType));
                    if (ImGui.Combo("Input Device Type", ref selectedDeviceTypeIndex, deviceTypeNames, deviceTypeNames.Length))
                    {
                        InputDeviceType newDeviceTypeValue = (InputDeviceType)Enum.ToObject(typeof(InputDeviceType), selectedDeviceTypeIndex);
                        _inputDeviceType = newDeviceTypeValue;
                    }

                    int selectedActionTypeIndex = Convert.ToInt32(_inputActionType);
                    string[] actionTypeNames = Enum.GetNames(typeof(InputActionType));
                    if (ImGui.Combo("Input Action Type", ref selectedActionTypeIndex, actionTypeNames, actionTypeNames.Length))
                    {
                        InputActionType newActionTypeValue = (InputActionType)Enum.ToObject(typeof(InputActionType), selectedActionTypeIndex);
                        _inputActionType = newActionTypeValue;
                    }

                    if (action != null)
                    {
                        if (ImGui.Button("Add"))
                        {
                            switch (_inputActionType)
                            {
                                case InputActionType.Button:
                                    switch (_inputDeviceType)
                                    {
                                        case InputDeviceType.Gamepad:
                                            action.InputBindings.Add(new ButtonInputBinding<GamepadButton>());
                                            break;
                                        case InputDeviceType.Keyboard:
                                            action.InputBindings.Add(new ButtonInputBinding<Keys>());
                                            break;
                                    }
                                    break;
                                case InputActionType.Axis:
                                    switch (_inputDeviceType)
                                    {
                                        case InputDeviceType.Gamepad:
                                            action.InputBindings.Add(new AxisInputBinding<GamepadButton>());
                                            break;
                                        case InputDeviceType.Keyboard:
                                            action.InputBindings.Add(new AxisInputBinding<Keys>());
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                    ImGui.Unindent();
                }
            }
        }

        protected override ScriptableObject CreateAsset(string path)
        {
            return new InputAsset();
        }
        protected override ScriptableObject LoadAsset(string path)
        {
            return ContentLoader.AssetManager.LoadAsset<InputAsset>(path);
        }
        protected override string GetAssetCustomFolder()
        {
            return "Input";
        }

    }
}
