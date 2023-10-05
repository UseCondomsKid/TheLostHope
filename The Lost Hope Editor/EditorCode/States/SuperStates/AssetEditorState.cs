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
            _editorAssetManager = new EditorAssetManager(CreateAsset, LoadAsset);
        }

        protected abstract ScriptableObject LoadAsset(string path);
        protected abstract ScriptableObject CreateAsset(string path);

        public override void Update(GameTime gameTime)
        {
            _editorAssetManager.Update(gameTime);
        }
    }
}
