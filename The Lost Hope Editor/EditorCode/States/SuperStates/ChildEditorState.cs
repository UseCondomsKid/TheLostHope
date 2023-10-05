using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEditor.EditorCode.StateManagement;

namespace TheLostHopeEditor.EditorCode.States.SuperStates
{
    public abstract class ChildEditorState : EditorState
    {
        protected ChildEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void DrawImGui(GameTime gameTime)
        {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("Navigation"))
                {
                    if (ImGui.MenuItem("Return to base"))
                    {
                        _stateManager.SetState(_gameRef.EditorBaseState);
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
        }
    }
}
