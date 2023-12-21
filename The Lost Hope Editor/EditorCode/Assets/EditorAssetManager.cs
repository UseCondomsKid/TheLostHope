using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui.Extensions;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEditor.EditorCode.Utils;
using TheLostHopeEngine.EngineCode.Assets.Core;
using TheLostHopeEngine.EngineCode.CustomAttributes;

namespace TheLostHopeEditor.EditorCode.Assets
{
    public class DictionaryPair
    {
        public object Key { get; set; }
        public object Value { get; set; }
    }

    public class EditorAssetManager
    {
        public string PathToAssetsFolder =>
            "C:\\000\\Programming\\The Lost Hope\\The Lost Hope\\bin\\Debug\\net6.0\\Assets" +
            (_assetCutsomFolder != "" ? $"\\{_assetCutsomFolder}" : "");

        public ScriptableObject Asset { get { return _asset; } }
        public Action<ScriptableObject> OnLoadedAsset;

        private string _assetPath;
        private ScriptableObject _asset;

        // Function that returns the name of the asset
        private Func<string, ScriptableObject> _createNewAssetFunction;
        private Func<string, ScriptableObject> _loadAssetFunction;
        private string _assetCutsomFolder;

        private bool _loaded;


        // Imgui Editor Stuff
        private bool _createNewAssetWindow = false;
        private string _newAssetName = "";

        private float _assetSavedMessageShowTime = 1.5f;
        private float _assetSavedTimer = -1.0f;

        private bool _newDictionaryEntryWindow = false;
        private IDictionary _currentDictionaryToAddTo = null;
        private object _currentDictionaryKey = null;
        private object _currentDictionaryValue = null;


        private float _dictionaryMessageShowTime = 1.5f;
        private float _dictionaryMessageTimer = -1.0f;
        private string _dictionaryMessage = "";
        private System.Numerics.Vector4 _dictionaryMessageColor;

        public EditorAssetManager(Func<string, ScriptableObject> createNewAssetFunction, Func<string, ScriptableObject> loadAssetFunction, string assetCutsomFolder = "")
        {
            _loaded = false;
            _assetPath = "";

            _createNewAssetFunction = createNewAssetFunction;
            _loadAssetFunction = loadAssetFunction;
            _assetCutsomFolder = assetCutsomFolder;

            _createNewAssetWindow = false;
            _newDictionaryEntryWindow = false;
            _currentDictionaryToAddTo = null;
            _dictionaryMessage = "";
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
            OnLoadedAsset?.Invoke(_asset);
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
            // Checking if ctrl + s is pressed to save
            // Very crude implementation, but it works
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.S))
            {
                SaveAsset();
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _assetSavedTimer -= delta;
            _dictionaryMessageTimer -= delta;
        }

        public void RenderEditorBase()
        {
            ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), "Editor");
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
            // Dictionary entry
            if (_newDictionaryEntryWindow)
            {
                if (_currentDictionaryToAddTo != null && _currentDictionaryKey != null && _currentDictionaryValue != null)
                {
                    if (ImGui.Begin($"Add Entry To Dictionary<{_currentDictionaryKey.GetType().Name}, {_currentDictionaryValue.GetType().Name}>"))
                    {
                        DrawNewDictionaryEntry();

                        if (ImGui.Button("Add Entry"))
                        {
                            if (_currentDictionaryKey == null || _currentDictionaryValue == null)
                            {
                                _dictionaryMessage = "Either the key or the value (or both) are null.";
                                _dictionaryMessageTimer = _dictionaryMessageShowTime;
                                _dictionaryMessageColor = new System.Numerics.Vector4(1f, 0f, 0f, 1f);
                            }
                            else if (_currentDictionaryToAddTo.Contains(_currentDictionaryKey))
                            {
                                _dictionaryMessage = "The Dictionary already contains an entry with the same key.";
                                _dictionaryMessageTimer = _dictionaryMessageShowTime;
                                _dictionaryMessageColor = new System.Numerics.Vector4(1f, 0f, 0f, 1f);
                            }
                            else
                            {
                                _currentDictionaryToAddTo.Add(_currentDictionaryKey, _currentDictionaryValue);
                                _dictionaryMessage = "Successfully added new entry to the dictionary.";
                                _dictionaryMessageTimer = _dictionaryMessageShowTime;
                                _dictionaryMessageColor = new System.Numerics.Vector4(0f, 1f, 0f, 1f);
                            }
                        }
                        if (ImGui.Button("Close"))
                        {
                            CloseNewDictionaryEntryWindow();
                        }

                        if (_dictionaryMessageTimer > 0f)
                        {
                            ImGui.TextColored(_dictionaryMessageColor, _dictionaryMessage);
                        }

                        ImGui.End();
                    }
                }
                else
                {
                    CloseNewDictionaryEntryWindow();
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

                // Displaying Messages
                if (_assetSavedTimer > 0f)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0f, 1f, 0f, 1f), "Asset Saved!");
                }
            }

            ImGui.Spacing();
            ImGui.Separator();
        }
        private void CloseNewDictionaryEntryWindow()
        {
            _newDictionaryEntryWindow = false;
            _currentDictionaryToAddTo = null;
            _currentDictionaryKey = null;
            _currentDictionaryValue = null;
        }

        public void RenderAsset()
        {
            if (_loaded && _asset != null)
            {
                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();

                DrawProperties(_asset);
            }
        }

        private void DrawProperties(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (!IsSimpleType(obj.GetType()))
            {
                //foreach (var method in obj.GetType().GetMethods())
                //{
                //    // Check for button attribute

                //}
                foreach (var property in obj.GetType().GetProperties())
                {
                    // Check for header attribute
                    HeaderAttribute headerAttribute = property.GetCustomAttribute<HeaderAttribute>();
                    if (headerAttribute != null)
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), headerAttribute.Header);
                    }

                    // Check range attribute
                    RangeAttribute rangeAttribute = property.GetCustomAttribute<RangeAttribute>();

                    if (property.CanRead && property.CanWrite)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            string value = (string)property.GetValue(obj) ?? "";
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
                            if (rangeAttribute != null)
                            {
                                float minValue = rangeAttribute.MinValue;
                                float maxValue = rangeAttribute.MaxValue;
                                float value = (float)property.GetValue(obj);
                                ImGui.SliderFloat(property.Name, ref value, minValue, maxValue);
                                property.SetValue(obj, value);
                            }
                            else
                            {
                                float value = (float)property.GetValue(obj);
                                ImGui.InputFloat(property.Name, ref value);
                                property.SetValue(obj, value);
                            }
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
                            int selectedIndex = Convert.ToInt32(enumValue);
                            string[] names = Enum.GetNames(property.PropertyType);
                            if (ImGui.Combo(property.Name, ref selectedIndex, names, names.Length))
                            {
                                Enum newValue = (Enum)Enum.ToObject(property.PropertyType, selectedIndex);
                                property.SetValue(obj, newValue);
                            }
                        }
                        else if (property.PropertyType == typeof(Vector2))
                        {
                            if (rangeAttribute != null)
                            {
                                float minValue = rangeAttribute.MinValue;
                                float maxValue = rangeAttribute.MaxValue;
                                Vector2 value = (Vector2)property.GetValue(obj);
                                var numericVector2 = value.ToNumerics();
                                ImGui.SliderFloat2(property.Name, ref numericVector2, minValue, maxValue);
                                value = numericVector2.ToXnaVector2();
                                property.SetValue(obj, value);
                            }
                            else
                            {
                                Vector2 value = (Vector2)property.GetValue(obj);
                                var numericVector2 = value.ToNumerics();
                                ImGui.InputFloat2(property.Name, ref numericVector2);
                                value = numericVector2.ToXnaVector2();
                                property.SetValue(obj, value);
                            }
                        }
                        else if (typeof(IDictionary).IsAssignableFrom(property.PropertyType))
                        {
                            IDictionary dictionary = (IDictionary)property.GetValue(obj);
                            if (dictionary != null)
                            {
                                ImGui.BeginGroup();
                                ImGui.Text($"{property.Name}:");

                                int i = 0;
                                foreach (DictionaryEntry entry in dictionary)
                                {
                                    string elementName = $"{property.Name}[{i}]";
                                    string elementId = $"{property.PropertyType.GUID}-{elementName}-{i}";

                                    ImGui.BeginGroup();
                                    if (ImGui.TreeNode(elementId, elementName))
                                    {
                                        // Draw entry
                                        ImGui.Indent();
                                        DrawDictionaryEntry(entry);

                                        // Display a button to remove the entry
                                        if (ImGui.Button($"Remove {elementName}"))
                                        {
                                            dictionary.Remove(entry.Key);
                                        }
                                        ImGui.Unindent();

                                        ImGui.Spacing();
                                    }
                                    ImGui.EndGroup();

                                    i++;
                                }


                                var dictGenericArgs = dictionary.GetType().GetGenericArguments();
                                if (dictGenericArgs[0].IsAbstract || dictGenericArgs[1].IsAbstract)
                                {
                                    ImGui.TextColored(new System.Numerics.Vector4(0f, 0f, 1f, 1f),
                                        "The Entry type of this dictionary (Key or Value) is abstract. Cannot create instances of it to add.");
                                }
                                else
                                {
                                    // Display a button to add entries
                                    string newDictionaryEntryWindowStatus = _newDictionaryEntryWindow ? "Open" : "Closed";
                                    if (ImGui.Button($"Add {property.Name} Entry (Window {newDictionaryEntryWindowStatus})"))
                                    {
                                        _currentDictionaryToAddTo = dictionary;
                                        _currentDictionaryKey = CreateDefaultElement(dictGenericArgs[0]);
                                        _currentDictionaryValue = CreateDefaultElement(dictGenericArgs[1]);

                                        _newDictionaryEntryWindow = true;
                                    }
                                }

                                ImGui.EndGroup();
                            }
                        }
                        else if (typeof(IList).IsAssignableFrom(property.PropertyType))
                        {
                            IList list = (IList)property.GetValue(obj);
                            if (list != null)
                            {
                                ImGui.BeginGroup();
                                ImGui.Text($"{property.Name}:");

                                for (int i = 0; i < list.Count; i++)
                                {
                                    // Create a unique name for each element
                                    string elementName = $"{property.Name}[{i}]";
                                    string elementId = $"{property.PropertyType.GUID}-{elementName}-{i}"; // Include the index in the ID
                                    if (ImGui.TreeNode(elementId, elementName))
                                    {
                                        ImGui.Indent();
                                        // Check if the list element is a simple type
                                        if (IsSimpleType(list[i].GetType()))
                                        {
                                            DrawListElement(list, i);
                                        }
                                        else
                                        {
                                            DrawProperties(list[i]);
                                        }

                                        // Display a button to remove the element
                                        if (ImGui.Button($"Remove {elementName}"))
                                        {
                                            list.RemoveAt(i);
                                            i--; // Adjust the loop counter
                                        }
                                        ImGui.Unindent();

                                        ImGui.Spacing();
                                        ImGui.TreePop();
                                    }
                                }


                                Type elementType = property.PropertyType.IsArray ? property.PropertyType.GetElementType() : property.PropertyType.GetGenericArguments()[0];
                                if (elementType.IsAbstract)
                                {
                                    ImGui.TextColored(new System.Numerics.Vector4(0f, 0f, 1f, 1f),
                                        "The Element type of this list is abstract. Cannot create instances of it to add.");
                                }
                                else
                                {
                                    // Display a button to add elements
                                    if (ImGui.Button($"Add {property.Name} Element"))
                                    {
                                        object newElement = CreateDefaultElement(elementType);
                                        list.Add(newElement);
                                    }
                                }

                                ImGui.EndGroup();
                            }
                        }
                        else if (property.PropertyType.IsClass)
                        {
                            object propertyValue = property.GetValue(obj);
                            if (propertyValue != null)
                            {
                                ImGui.BeginGroup();
                                ImGui.Text($"{property.Name}:");
                                DrawProperties(propertyValue);
                                ImGui.EndGroup();
                            }
                        }
                    }
                    else if (property.CanRead && !property.CanWrite)
                    {
                        var value = property.GetValue(obj);
                        ImGui.Text($"{property.Name}: {value}");
                    }
                }

            }
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type.IsEnum || type == typeof(Vector2);
        }

        private object CreateDefaultElement(Type elementType)
        {
            if (elementType == typeof(string))
            {
                return "";
            }
            else if (elementType.IsEnum)
            {
                Array enumValues = Enum.GetValues(elementType);
                return enumValues.GetValue(0);
            }
            else
            {
                return Activator.CreateInstance(elementType);
            }
        }

        public void DrawNewDictionaryEntry()
        {
            // Key
            Type keyType = _currentDictionaryKey.GetType();
            ImGui.Text("Key:");
            if (IsSimpleType(_currentDictionaryKey.GetType()))
            {
                if (keyType == typeof(string))
                {
                    string value = (string)_currentDictionaryKey;
                    ImGui.InputText(keyType.Name, ref value, 255);
                    _currentDictionaryKey = value;
                }
                else if (keyType == typeof(int))
                {
                    int value = (int)_currentDictionaryKey;
                    ImGui.InputInt(keyType.Name, ref value);
                    _currentDictionaryKey = value;
                }
                else if (keyType == typeof(float))
                {
                    float value = (float)_currentDictionaryKey;
                    ImGui.InputFloat(keyType.Name, ref value);
                    _currentDictionaryKey = value;
                }
                else if (keyType == typeof(bool))
                {
                    bool value = (bool)_currentDictionaryKey;
                    ImGui.Checkbox(keyType.Name, ref value);
                    _currentDictionaryKey = value;
                }
                else if (keyType.IsEnum)
                {
                    Enum enumValue = (Enum)_currentDictionaryKey;
                    int selectedIndex = Convert.ToInt32(enumValue);
                    string[] names = Enum.GetNames(keyType);
                    if (ImGui.Combo(keyType.Name, ref selectedIndex, names, names.Length))
                    {
                        Enum newValue = (Enum)Enum.ToObject(keyType, selectedIndex);
                        _currentDictionaryKey = newValue;
                    }
                }
                else if (keyType == typeof(Vector2))
                {
                    Vector2 value = (Vector2)_currentDictionaryKey;
                    var numericVector2 = value.ToNumerics();
                    ImGui.InputFloat2(keyType.Name, ref numericVector2);
                    value = numericVector2.ToXnaVector2();
                    _currentDictionaryKey = value;
                }
            }
            else
            {
                DrawProperties(_currentDictionaryKey);
            }


            // Value
            Type valueType = _currentDictionaryValue.GetType();
            ImGui.Text("Value:");
            if (IsSimpleType(_currentDictionaryValue.GetType()))
            {
                if (valueType == typeof(string))
                {
                    string value = (string)_currentDictionaryValue;
                    ImGui.InputText(valueType.Name, ref value, 255);
                    _currentDictionaryValue = value;
                }
                else if (valueType == typeof(int))
                {
                    int value = (int)_currentDictionaryValue;
                    ImGui.InputInt(valueType.Name, ref value);
                    _currentDictionaryValue = value;
                }
                else if (valueType == typeof(float))
                {
                    float value = (float)_currentDictionaryValue;
                    ImGui.InputFloat(valueType.Name, ref value);
                    _currentDictionaryValue = value;
                }
                else if (valueType == typeof(bool))
                {
                    bool value = (bool)_currentDictionaryValue;
                    ImGui.Checkbox(valueType.Name, ref value);
                    _currentDictionaryValue = value;
                }
                else if (valueType.IsEnum)
                {
                    Enum enumValue = (Enum)_currentDictionaryValue;
                    int selectedIndex = Convert.ToInt32(enumValue);
                    string[] names = Enum.GetNames(valueType);
                    if (ImGui.Combo(valueType.Name, ref selectedIndex, names, names.Length))
                    {
                        Enum newValue = (Enum)Enum.ToObject(valueType, selectedIndex);
                        _currentDictionaryValue = newValue;
                    }
                }
                else if (valueType == typeof(Vector2))
                {
                    Vector2 value = (Vector2)_currentDictionaryValue;
                    var numericVector2 = value.ToNumerics();
                    ImGui.InputFloat2(valueType.Name, ref numericVector2);
                    value = numericVector2.ToXnaVector2();
                    _currentDictionaryValue = value;
                }
            }
            else
            {
                DrawProperties(_currentDictionaryValue);
            }
        }

        private void DrawDictionaryEntry(DictionaryEntry entry)
        {
            Type keyType = entry.Key.GetType();
            Type valueType = entry.Value.GetType();

            ImGui.Text($"Key ({keyType.Name}):");
            ImGui.Indent();
            if (IsSimpleType(keyType))
            {
                ImGui.Text(entry.Key.ToString());
            }
            else
            {
                foreach (var property in entry.Key.GetType().GetProperties())
                {
                    if (!property.CanRead) continue;
                    ImGui.Text($"{property.Name}:");
                    ImGui.Text(property.GetValue(entry.Key).ToString());
                }
            }
            ImGui.Unindent();

            ImGui.Text($"Value ({valueType.Name}):");
            ImGui.Indent();
            if (IsSimpleType(valueType))
            {
                ImGui.Text(entry.Value.ToString());
            }
            else
            {
                foreach (var property in entry.Value.GetType().GetProperties())
                {
                    if (!property.CanRead) continue;
                    ImGui.Text($"{property.Name}:");
                    ImGui.Text(property.GetValue(entry.Value).ToString());
                }
            }
            ImGui.Unindent();
        }

        private void DrawListElement(IList list, int index)
        {
            Type elementType = list[index].GetType();

            if (elementType == typeof(int))
            {
                int value = (int)list[index];
                ImGui.InputInt(elementType.Name, ref value);
                list[index] = value;
            }
            else if (elementType == typeof(float))
            {
                float value = (float)list[index];
                ImGui.InputFloat(elementType.Name, ref value);
                list[index] = value;
            }
            else if (elementType == typeof(string))
            {
                string value = (string)list[index];
                if (value == null) value = "";
                ImGui.InputText(elementType.Name, ref value, 255);
                list[index] = value;
            }
            else if (elementType == typeof(bool))
            {
                bool value = (bool)list[index];
                ImGui.Checkbox(elementType.Name, ref value);
                list[index] = value;
            }
            else if (elementType.IsEnum)
            {
                Enum enumValue = (Enum)list[index];
                // Convert enum to int
                int selectedIndex = Convert.ToInt32(enumValue);
                string[] names = Enum.GetNames(elementType);
                if (ImGui.Combo(elementType.Name, ref selectedIndex, names, names.Length))
                {
                    // Convert int back to enum
                    Enum newValue = (Enum)Enum.ToObject(elementType, selectedIndex);
                    list[index] = newValue;
                }
            }
            else if (elementType == typeof(Vector2))
            {
                Vector2 value = (Vector2)list[index];
                var numericVector2 = value.ToNumerics();
                ImGui.InputFloat2(elementType.Name, ref numericVector2);
                value = numericVector2.ToXnaVector2();
                list[index] = value;
            }
        }
    }
}



