﻿using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGui.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
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
        public ScriptableObject Asset { get { return _asset; } }

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
                    DrawProperty(_assetProperties[i], _asset);
                }
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Asset Specific Properties");
                // Draw the rest of the properties
                for (int i = 0; i < _assetProperties.Length - _scriptableObjectMainPropCount; i++)
                {
                    DrawProperty(_assetProperties[i], _asset);
                    ImGui.Spacing();
                }
            }

            ImGui.EndChild();
        }

        private void DrawProperty(PropertyInfo property, object obj)
        {
            if (property == null) return;

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
                var value = property.GetValue(obj);
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
                    float value = (float)property.GetValue(obj);
                    ImGui.SliderFloat(property.Name, ref value, minValue, maxValue);
                    property.SetValue(obj, value);
                }
                // TODO: Check for other attributes
                else
                {
                    if (property.PropertyType == typeof(string))
                    {
                        string value = (string)property.GetValue(obj);
                        ImGui.InputText(property.Name, ref value, 255);
                        property.SetValue(obj, value);
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        int value = (int)property.GetValue(obj);
                        ImGui.InputInt(property.Name, ref value);
                        property.SetValue(obj, value);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        float value = (float)property.GetValue(obj);
                        ImGui.InputFloat(property.Name, ref value);
                        property.SetValue(obj, value);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        float value = (float)property.GetValue(obj);
                        ImGui.InputFloat(property.Name, ref value);
                        property.SetValue(obj, value);
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        bool value = (bool)property.GetValue(obj);
                        ImGui.Checkbox(property.Name, ref value);
                        property.SetValue(obj, value);
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        Enum enumValue = (Enum)property.GetValue(obj);
                        // Convert enum to int
                        int selectedIndex = Convert.ToInt32(enumValue); 
                        string[] names = Enum.GetNames(property.PropertyType);
                        if (ImGui.Combo(property.Name, ref selectedIndex, names, names.Length))
                        {
                            // Convert int back to enum
                            Enum newValue = (Enum)Enum.ToObject(property.PropertyType, selectedIndex);
                            property.SetValue(obj, newValue);
                        }
                    }
                    else if (property.PropertyType == typeof(Vector2))
                    {
                        Vector2 value = (Vector2)property.GetValue(obj);
                        var numericVector2 = value.ToNumerics();
                        ImGui.InputFloat2(property.Name, ref numericVector2);
                        value = numericVector2.ToXnaVector2();
                        property.SetValue(obj, value);
                    }
                    // TODO: Add more types
                    else if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                        var listItemValues = (IList)property.GetValue(obj);

                        if (listItemValues != null)
                        {
                            ImGui.Text($"{property.Name}");// (List of: {property.DeclaringType.Name})");

                            int indexToRemove = -1;
                            int index = 0;

                            // Iterate through the list
                            foreach (var itemValue in listItemValues)
                            {
                                // Display a collapsible node for each list element
                                if (ImGui.TreeNode(itemValue.GetType().GUID.ToString() + index, $"Element: {index}"))
                                {
                                    if (itemValue == null)
                                    {
                                        ImGui.Text("Element is null");
                                    }
                                    else
                                    {
                                        var itemProperty = property.PropertyType.GetProperty("Item");
                                        DrawProperty(itemProperty, property.GetValue(obj));
                                    }

                                    // Add a button to remove the element
                                    if (ImGui.Button($"Remove##{index}"))
                                    {
                                        indexToRemove = index;
                                    }

                                    ImGui.TreePop();
                                }
                                index++;
                            }

                            // Check if an element should be removed
                            if (indexToRemove != -1)
                            {
                                // Determine the type of elements in the list
                                Type elementType = property.PropertyType.GetGenericArguments()[0];

                                // Create a list of the correct type
                                var typedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                                // Add the existing list elements to the typed list
                                foreach (var item in listItemValues)
                                {
                                    typedList.Add(item);
                                }

                                // Remove the element at the specified index
                                typedList.RemoveAt(indexToRemove);

                                // Set the list property with the updated list
                                property.SetValue(obj, typedList);
                            }
                            // Add a button to add a new element to the list
                            if (ImGui.Button("Add Element"))
                            {
                                // Determine the type of elements in the list
                                Type elementType = property.PropertyType.GetGenericArguments()[0];

                                // Create a new instance of the element type
                                var newElement = Activator.CreateInstance(elementType);

                                // Cast the new element to the correct type
                                object typedElement = Convert.ChangeType(newElement, elementType);

                                // Create a list of the correct type
                                var typedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                                // Add the existing list elements to the typed list
                                foreach (var item in listItemValues)
                                {
                                    typedList.Add(item);
                                }

                                // Add the typed element to the list
                                typedList.Add(typedElement);

                                // Set the list property with the updated list
                                property.SetValue(obj, typedList);
                            }
                        }
                    }
                }
            }
        }

        private void DrawListProperties(object obj)
        {
            if (obj != null)
            {
                var objType = obj.GetType();
                var properties = objType.GetProperties();

                foreach (var prop in properties)
                {
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum)
                    {
                        // Simple property (primitive, string, or enum)
                        DrawProperty(prop, obj);
                    }
                    else
                    {
                        // Complex object - recursively draw its properties
                        var complexObject = prop.GetValue(obj);
                        if (complexObject != null)
                        {
                            ImGui.Text($"{prop.Name}:");
                            ImGui.Indent();
                            DrawListProperties(complexObject);
                            ImGui.Unindent();
                        }
                    }
                }
            }
        }
    }
}