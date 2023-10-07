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
        protected string _message;
        protected bool _isMessageError;
        protected float _messageTimer;

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

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _messageTimer -= delta;
        }

        protected virtual void ShowMessage(string message, bool isError, float timer)
        {
            _message = message;
            _isMessageError = isError;
            _messageTimer = timer;
        }

        protected void DrawImGuiMessage()
        {
            if (_messageTimer > 0f)
            {
                ImGui.TextColored(_isMessageError ? 
                    new System.Numerics.Vector4(1f, 0f, 0f, 1f) : new System.Numerics.Vector4(0f, 1f, 0f, 1f),
                    _message);
            }
        }
    }
}
