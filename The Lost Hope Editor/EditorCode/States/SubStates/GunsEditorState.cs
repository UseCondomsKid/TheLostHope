using ImGuiNET;
using Microsoft.Xna.Framework;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEditor.EditorCode.StateManagement;
using TheLostHopeEditor.EditorCode.States.SuperStates;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Assets.Core;

namespace TheLostHopeEditor.EditorCode.States.SubStates
{
    public class GunsEditorState : AssetEditorState
    {
        public GunsEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void DrawGame(GameTime gameTime)
        {
        }

        public override void DrawImGui(GameTime gameTime)
        {
            base.DrawImGui(gameTime);
            _editorAssetManager.RenderAsset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        protected override ScriptableObject CreateAsset(string path)
        {
            return new WeaponAsset();
        }

        protected override ScriptableObject LoadAsset(string path)
        {
            return ContentLoader.AssetManager.LoadAsset<WeaponAsset>(path);
        }
    }
}
