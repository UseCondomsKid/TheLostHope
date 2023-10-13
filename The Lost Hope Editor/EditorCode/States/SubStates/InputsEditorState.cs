using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEditor.EditorCode.StateManagement;
using TheLostHopeEditor.EditorCode.States.SuperStates;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Assets.Core;

namespace TheLostHopeEditor.EditorCode.States.SubStates
{
    public class InputsEditorState : AssetEditorState
    {
        public InputsEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void DrawGame(GameTime gameTime)
        {
        }
        protected override void DrawImGuiTool()
        {
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
