using Apos.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace TheLostHope.GameCode.GameSettings
{
    public class Settings
    {
        // Application
        public int X { get; set; } = 320;
        public int Y { get; set; } = 180;
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;
        public bool IsFixedTimeStep { get; set; } = true;
        public bool IsVSync { get; set; } = false;
        public bool IsFullscreen { get; set; } = false;
        public bool IsBorderless { get; set; } = false;

        // Player Input Bindings
        // Move Left
        public int KPlayerMoveLeftBinding { get; set; } = (int)Keys.A;
        public int GPlayerMoveLeftBinding { get; set; } = (int)GamePadButton.Left;
        // Move Right
        public int KPlayerMoveRightBinding { get; set; } = (int)Keys.D;
        public int GPlayerMoveRightBinding { get; set; } = (int)GamePadButton.Right;
        // Jump
        public int KPlayerJumpBinding { get; set; } = (int)Keys.W;
        public int GPlayerJumpBinding { get; set; } = (int)GamePadButton.A;
        // Roll
        public int KPlayerRollBinding { get; set; } = (int)Keys.LeftShift;
        public int GPlayerRollBinding { get; set; } = (int)GamePadButton.B;
        // Parry
        public int KPlayerParryBinding { get; set; } = (int)Keys.J;
        public int GPlayerParryBinding { get; set; } = (int)GamePadButton.X;
        // Shoot
        public int KPlayerShootBinding { get; set; } = (int)Keys.K;
        public int GPlayerShootBinding { get; set; } = (int)GamePadButton.LeftShoulder;
        // Reload
        public int KPlayerReloadBinding { get; set; } = (int)Keys.L;
        public int GPlayerReloadBinding { get; set; } = (int)GamePadButton.RightShoulder;
        // Interact
        public int KPlayerInteractBinding { get; set; } = (int)Keys.E;
        public int GPlayerInteractBinding { get; set; } = (int)GamePadButton.Up;



        public static string GetPath(string name) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);

        public static T LoadJson<T>(string name, JsonTypeInfo<T> typeInfo) where T : new()
        {
            T json;
            string jsonPath = GetPath(name);

            if (File.Exists(jsonPath))
            {
                json = JsonSerializer.Deserialize<T>(File.ReadAllText(jsonPath), typeInfo);
            }
            else
            {
                json = new T();
            }

            return json;
        }
        public static void SaveJson<T>(string name, T json, JsonTypeInfo<T> typeInfo)
        {
            string jsonPath = GetPath(name);
            string jsonString = JsonSerializer.Serialize(json, typeInfo);
            File.WriteAllText(jsonPath, jsonString);
        }
        public static T EnsureJson<T>(string name, JsonTypeInfo<T> typeInfo) where T : new()
        {
            T json;
            string jsonPath = GetPath(name);

            if (File.Exists(jsonPath))
            {
                json = JsonSerializer.Deserialize<T>(File.ReadAllText(jsonPath), typeInfo)!;
            }
            else
            {
                json = new T();
                string jsonString = JsonSerializer.Serialize(json, typeInfo);
                File.WriteAllText(jsonPath, jsonString);
            }

            return json;
        }
    }


    [JsonSourceGenerationOptionsAttribute(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
    [JsonSerializable(typeof(Settings))]
    internal partial class SettingsContext : JsonSerializerContext { }
}
