using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System;
using System.IO;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEditor.EditorCode.StateManagement;
using TheLostHopeEditor.EditorCode.States.SuperStates;
using TheLostHopeEngine.EngineCode.Animations;
using TheLostHopeEngine.EngineCode.Assets;
using TheLostHopeEngine.EngineCode.Assets.Core;

namespace TheLostHopeEditor.EditorCode.States.SubStates
{
    public class GunsEditorState : AssetEditorState
    {
        private WeaponAsset _weaponAsset;
        private Animator _gunAnimator;

        public GunsEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _editorAssetManager.OnLoadedAsset += AssetLoaded;
        }

        private void AssetLoaded(ScriptableObject asset)
        {
            _weaponAsset = (WeaponAsset)asset;
        }

        public override void DrawGame(GameTime gameTime)
        {
            if (_gunAnimator != null)
            {
                _gameRef.SpriteBatch.Draw(_gunAnimator.SpriteSheetTexture, new Vector2(0, 0), _gunAnimator.GetSourceRectangle(),
                    Color.White);
            }
        }


        protected override ScriptableObject CreateAsset(string path)
        {
            return new WeaponAsset();
        }

        protected override ScriptableObject LoadAsset(string path)
        {
            return ContentLoader.AssetManager.LoadAsset<WeaponAsset>(path);
        }

        protected override void DrawImGuiTool()
        {
            if (_weaponAsset != null)
            {
                if (ImGui.Button("Load Gun Animator"))
                {
                    if (_weaponAsset.AsepriteFileName != null && _weaponAsset.AsepriteFileName != "")
                    {
                        string path = Path.Combine(PathToGameContent, "AsepriteFiles", _weaponAsset.AsepriteFileName + ".aseprite");
                        if (!File.Exists(path))
                        {
                            ShowMessage($"Couldn't find a file: '{path}'", true, 2f);
                        }
                        else
                        {
                            _gunAnimator = new Animator(AsepriteFile.Load(path), _gameRef.GraphicsDevice);
                            if (_gunAnimator == null)
                            {
                                ShowMessage("Couldn't Load Animator, Check path to aseprite file", true, 1.5f);
                            }
                            else
                            {
                                ShowMessage("Successfully loaded animator", false, 1.5f);
                            }
                        }
                    }
                    else
                    {
                        ShowMessage("Can't load animator, Check path to aseprite file", true, 1.5f);
                    }
                }

                if (_gunAnimator != null)
                {
                    _gunAnimator.SetActiveAnimation("Shoot");
                    _gunAnimator.CurrentAnimation.Stop(false, 0);
                }
            }
        }
    }
}
