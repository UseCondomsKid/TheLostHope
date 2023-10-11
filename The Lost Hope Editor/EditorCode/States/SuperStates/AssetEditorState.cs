using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEditor.EditorCode.Assets;
using TheLostHopeEditor.EditorCode.StateManagement;
using TheLostHopeEngine.EngineCode.Assets.Core;

namespace TheLostHopeEditor.EditorCode.States.SuperStates
{
    public abstract class AssetEditorState : ChildEditorState
    {
        protected EditorAssetManager _editorAssetManager;

        public AssetEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
            _editorAssetManager = new EditorAssetManager(CreateAsset, LoadAsset, GetAssetCustomFolder());
        }

        protected abstract string GetAssetCustomFolder();
        protected abstract ScriptableObject LoadAsset(string path);
        protected abstract ScriptableObject CreateAsset(string path);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _editorAssetManager.Update(gameTime);
        }

        public override void DrawImGui(GameTime gameTime)
        {
            base.DrawImGui(gameTime);

            ImGui.BeginChild("Scrolling");
            _editorAssetManager.RenderEditorBase();

            ImGui.TextColored(new System.Numerics.Vector4(1f, 1f, 0f, 1f), _name);
            DrawImGuiMessage();
            DrawImGuiTool();

            _editorAssetManager.RenderAsset();
            ImGui.EndChild();
        }

        protected abstract void DrawImGuiTool();
    }
}
