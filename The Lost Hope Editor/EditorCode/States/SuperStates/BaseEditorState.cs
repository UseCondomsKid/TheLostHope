using ImGuiNET;
using Microsoft.Xna.Framework;
using TheLostHopeEditor.EditorCode.StateManagement;

namespace TheLostHopeEditor.EditorCode.States.SuperStates
{
    public class BaseEditorState : EditorState
    {
        public BaseEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void DrawGame(GameTime gameTime)
        {
        }

        public override void DrawImGui(GameTime gameTime)
        {
            if (ImGui.Button("Open Guns Editor"))
            {
                _stateManager.SetState(_gameRef.GunsEditorState);
            }
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
