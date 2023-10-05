using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGui.Extensions;
using System;
using System.IO;
using System.Reflection;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEditor.EditorCode.Utils;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.CustomAttributes;

namespace TheLostHopeEditor.EditorCode.Assets
{
    public class EditorAssetManager
    {
        public const string PathToAssetsFolder = "C:\\000\\Programming\\The Lost Hope\\The Lost Hope\\bin\\Debug\\net6.0\\Assets";

        private string _assetPath;
        private ScriptableObject _asset;
        private PropertyInfo[] _assetProperties;

        // Function that returns the name of the asset
        private Func<string, ScriptableObject> _createNewAssetFunction;
        private Func<string, ScriptableObject> _loadAssetFunction;

        private bool _loaded;

        private int _scriptableObjectMainPropCount;


        // Imgui Editor Stuff
        private bool _createNewAssetWindow = false;
        private string _newAssetName = "";
        private float _assetSavedMessageShowTime = 1.5f;
        private float _assetSavedTimer = -1.0f;

        public EditorAssetManager(Func<string, ScriptableObject> createNewAssetFunction, Func<string, ScriptableObject> loadAssetFunction)
        {
            _loaded = false;
            _assetPath = "";

            _createNewAssetFunction = createNewAssetFunction;
            _loadAssetFunction = loadAssetFunction;

            Type soType = typeof(ScriptableObject);
            _scriptableObjectMainPropCount = soType.GetProperties().Length;

            _createNewAssetWindow = false;
            _newAssetName = "";
            _assetSavedTimer = -1.0f;
        }

        public void OpenLoadAssetFileDialog()
        {
            FileDialog.OpenFileDialog("", "Asset Files *.asset|*.asset", LoadAsset);
        }

        private void LoadAsset(string path)
        {
            _assetPath = path;
            _asset = _loadAssetFunction?.Invoke(path);
            _assetProperties = _asset.GetType().GetProperties();
            _loaded = true;
        }

        public void SaveAsset()
        {
            if (!_loaded) return;

            ContentLoader.AssetManager.SaveAsset(_assetPath, _asset);

            _assetSavedTimer = _assetSavedMessageShowTime;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _assetSavedTimer -= delta;
        }

        public void RenderAsset()
        {
            ImGui.BeginChild("Scrolling");
            ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Base");
            string createNewAssetWindowStatus = _createNewAssetWindow ? "Opened" : "Closed";
            if (ImGui.Button($"Create And Load New Asset ({createNewAssetWindowStatus})"))
            {
                _createNewAssetWindow = !_createNewAssetWindow;
            }

            if (_createNewAssetWindow)
            {
                if (ImGui.Begin("Create New Asset"))
                {
                    ImGui.InputText("Name", ref _newAssetName, 100);
                    if (ImGui.Button("Create"))
                    {
                        if (_newAssetName != null && _newAssetName != "")
                        {
                            string path = Path.Combine(PathToAssetsFolder, _newAssetName + ".asset");
                            ContentLoader.AssetManager.SaveAsset(path, _createNewAssetFunction?.Invoke(path));
                            LoadAsset(path);

                            if (_loaded)
                            {
                                _asset.Name = _newAssetName;
                            }

                            SaveAsset();

                            _createNewAssetWindow = false;
                        }
                    }

                    ImGui.End();
                }
            }
            if (ImGui.Button("Load Existing Asset"))
            {
                OpenLoadAssetFileDialog();
            }
            if (_loaded && _asset != null)
            {
                ImGui.InputText("Current Asset Path", ref _assetPath, 400);
                if (ImGui.Button("Save Asset"))
                {
                    SaveAsset();
                }
                if (_assetSavedTimer > 0f)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0f, 1f, 0f, 1f), "Asset Saved!");
                }

                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Base Properties");
                // Drawing asset properties
                // Draw the base scriptable object properties separately
                for (int i = _assetProperties.Length - _scriptableObjectMainPropCount; i < _assetProperties.Length; i++)
                {
                    DrawProperty(_assetProperties[i]);
                }
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Asset Specific Properties");
                // Draw the rest of the properties
                for (int i = 0; i < _assetProperties.Length - _scriptableObjectMainPropCount; i++)
                {
                    DrawProperty(_assetProperties[i]);
                    ImGui.Spacing();
                }
            }

            ImGui.EndChild();
        }

        private void DrawProperty(PropertyInfo property)
        {
            // Check for header attribute
            HeaderAttribute headerAttribute = property.GetCustomAttribute<HeaderAttribute>();
            if (headerAttribute != null)
            {
                ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), headerAttribute.Header);
            }

            // Check range attribute
            RangeAttribute rangeAttribute = property.GetCustomAttribute<RangeAttribute>();

            // TODO: Check for other attributes

            if (property.CanRead && !property.CanWrite)
            {
                var value = property.GetValue(_asset);
                ImGui.Text($"{property.Name}: {value}");
            }
            else if (property.CanRead && property.CanWrite)
            {

                if (rangeAttribute != null)
                {
                    // This property has a RangeAttribute
                    float minValue = rangeAttribute.MinValue;
                    float maxValue = rangeAttribute.MaxValue;

                    // Render an ImGui slider with the specified range
                    float value = (float)property.GetValue(_asset);
                    ImGui.SliderFloat(property.Name, ref value, minValue, maxValue);
                    property.SetValue(_asset, value);
                }
                // TODO: Check for other attributes
                else
                {
                    if (property.PropertyType == typeof(string))
                    {
                        string value = (string)property.GetValue(_asset);
                        ImGui.InputText(property.Name, ref value, 255);
                        property.SetValue(_asset, value);
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        int value = (int)property.GetValue(_asset);
                        ImGui.InputInt(property.Name, ref value);
                        property.SetValue(_asset, value);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        float value = (float)property.GetValue(_asset);
                        ImGui.InputFloat(property.Name, ref value);
                        property.SetValue(_asset, value);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        float value = (float)property.GetValue(_asset);
                        ImGui.InputFloat(property.Name, ref value);
                        property.SetValue(_asset, value);
                    }
                    else if (property.PropertyType == typeof(Vector2))
                    {
                        Vector2 value = (Vector2)property.GetValue(_asset);
                        var numericVector2 = value.ToNumerics();
                        ImGui.InputFloat2(property.Name, ref numericVector2);
                        value = numericVector2.ToXnaVector2();
                        property.SetValue(_asset, value);
                    }
                    // TODO: Check other property types, render them and set their values
                }
            }

        }
    }
}
