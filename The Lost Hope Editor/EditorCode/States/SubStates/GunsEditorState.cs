﻿using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame.ImGui.Extensions;
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
    public class GunsEditorState : AssetEditorState, IDisposable
    {
        private WeaponAsset _weaponAsset;
        private Animator _gunAnimator;
        private Vector2 _gunDrawPosition;

        private Texture2D _gunShootPositionPixel;
        private System.Numerics.Vector4 _gunShootPositionPixelColor = new System.Numerics.Vector4(0f, 0f, 0f, 1f);
        private Vector2 _gunShootPositionDrawPosition;

        private WeaponShootDirection _currentWeaponShootDirection;

        private float _rotation;

        public GunsEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _editorAssetManager.OnLoadedAsset += AssetLoaded;

            _gunShootPositionPixel = new Texture2D(_gameRef.GraphicsDevice, 1, 1);
            _gunShootPositionPixel.SetData(new Color[] { Color.White });

            _currentWeaponShootDirection = WeaponShootDirection.Right;
        }

        private void AssetLoaded(ScriptableObject asset)
        {
            _weaponAsset = (WeaponAsset)asset;
            _gunAnimator = null;
        }

        public override void DrawGame(GameTime gameTime)
        {
            if (_gunAnimator != null)
            {
                // Draw Gun
                _gameRef.SpriteBatch.Draw(_gunAnimator.SpriteSheetTexture, _gunDrawPosition, _gunAnimator.GetSourceRectangle(),
                    Color.White,
                    _rotation,
                    _weaponAsset.SpriteHalfSize, 1f,
                    _currentWeaponShootDirection == WeaponShootDirection.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0.0f);

                // Draw Gun Shoot Position
                _gameRef.SpriteBatch.Draw(_gunShootPositionPixel, _gunShootPositionDrawPosition,
                        new Rectangle((int)_gunShootPositionDrawPosition.X, (int)_gunShootPositionDrawPosition.Y,
                        (int)_weaponAsset.Size.X, (int)_weaponAsset.Size.Y), new Color(_gunShootPositionPixelColor),
                        0.0f, 
                        Vector2.Zero, 1f,
                        SpriteEffects.None, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_weaponAsset != null)
            {
                switch (_currentWeaponShootDirection)
                {
                    case WeaponShootDirection.Up:
                        _gunShootPositionDrawPosition = _weaponAsset.UpFirePosition + _gunDrawPosition;
                        _rotation = _weaponAsset.UpDirectionRotation;
                        break;
                    case WeaponShootDirection.Down:
                        _gunShootPositionDrawPosition = _weaponAsset.DownFirePosition + _gunDrawPosition;
                        _rotation = _weaponAsset.DownDirectionRotation;
                        break;
                    case WeaponShootDirection.Left:
                        _gunShootPositionDrawPosition = _weaponAsset.LeftFirePosition + _gunDrawPosition;
                        _rotation = _weaponAsset.LeftDirectionRotation;
                        break;
                    case WeaponShootDirection.Right:
                        _gunShootPositionDrawPosition = _weaponAsset.RightFirePosition + _gunDrawPosition;
                        _rotation = _weaponAsset.RightDirectionRotation;
                        break;
                }
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
                            _gunAnimator.SetActiveAnimation("Shoot");

                            // Find the shoot frame
                            int shootFrameIndex = 0;
                            int index = 0;
                            foreach (var frame in _gunAnimator.CurrentAnimation.AnimationFrames)
                            {
                                if (frame.Value.TriggerEvent)
                                {
                                    shootFrameIndex = index;
                                    break;
                                }
                                index++;
                            }
                            _gunAnimator.CurrentAnimation.Stop(true, shootFrameIndex);

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
                    if (ImGui.Button("Right"))
                    {
                        _currentWeaponShootDirection = WeaponShootDirection.Right;
                    }
                    if (ImGui.Button("Left"))
                    {
                        _currentWeaponShootDirection = WeaponShootDirection.Left;
                    }
                    if (ImGui.Button("Up"))
                    {
                        _currentWeaponShootDirection = WeaponShootDirection.Up;
                    }
                    if (ImGui.Button("Down"))
                    {
                        _currentWeaponShootDirection = WeaponShootDirection.Down;
                    }

                    System.Numerics.Vector2 pos = _gunDrawPosition.ToNumerics();
                    ImGui.SliderFloat2("Gun Draw Position", ref pos, -200f, 200f);
                    _gunDrawPosition = pos.ToXnaVector2();
                    ImGui.ColorEdit4("Shoot Pixel Color", ref _gunShootPositionPixelColor);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected override string GetAssetCustomFolder()
        {
            return "Guns";
        }
    }
}
