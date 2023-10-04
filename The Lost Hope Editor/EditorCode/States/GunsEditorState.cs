using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHopeEditor.EditorCode.Guns;
using TheLostHopeEditor.EditorCode.StateManagement;
using System.Windows.Forms;
using MonoGame.Aseprite;
using System.IO;
using System.Threading;
using System.Diagnostics;
using TheLostHopeEditor.EditorCode.Animations;
using Microsoft.Xna.Framework.Graphics;

namespace TheLostHopeEditor.EditorCode.States
{
    public class GunsEditorState : EditorState
    {
        // Create Gun Window
        private bool _createNewWeaponWindow;

        // New gun props
        private float _cameraZoom = 1.0f;

        string _asepriteFilePath = string.Empty;
        private Animator _gunAnimator;
        private System.Numerics.Vector2 _gunDrawPosition;
        private int _gunCurrentAnimationIndex = -1;
        private bool _gunIsAnimationPlaying = false;
        private bool _gunAnimationInShootFrame = false;
        private int _gunShootFrameIndex = -1;


        // Bullet
        private Texture2D _gunShootPixel;
        private System.Numerics.Vector2 _gunShootPixelDrawPosition;
        private System.Numerics.Vector2 _gunShootPixelPosition;
        private System.Numerics.Vector4 _gunShootPixelColor = new System.Numerics.Vector4(0f, 0f, 0f, 1f);
        private float _bulletMaxRange = 0f;
        private float _bulletMaxLifeTime = 0f;
        private int _bulletDamage = 0;
        private float _bulletEnemyKnockbackForceOnHit = 0f;
        private float _bulletSpeed = 0f;
        private System.Numerics.Vector2 _bulletSize = new System.Numerics.Vector2(1f, 1f);
        private int _bulletPeneratration = 0;
        private float _bulletGravityScale = 0f;
        private int _bulletBounciness = 0;

        private string _gunName = "";
        private string _gunAsepriteFilePath = "";
        private int _gunMagSize = 0;
        private float _gunTimeBetweenShots = 0f;
        private float _gunPlayerKnockbackForceOnFire = 0f;
        private List<GunReloadPattern> _gunReloadPattern = new List<GunReloadPattern>();
        private int _gunFailedReloadMag = 0;

        public GunsEditorState(Game1 gameRef, EditorStateManager stateManager, string name) : base(gameRef, stateManager, name)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _gunShootPixel = new Texture2D(_gameRef.GraphicsDevice, 1, 1);
            _gunShootPixel.SetData(new Color[] { Color.White });

            _cameraZoom = _gameRef.Camera.Zoom;
        }

        public override void DrawGame(GameTime gameTime)
        {
            if (_gunAnimator != null)
            {
                _gameRef.SpriteBatch.Draw(_gunAnimator.SpriteSheetTexture,
                     _gunDrawPosition,
                    _gunAnimator.GetSourceRectangle(), Color.White, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None, 0f);

                if (_gunAnimationInShootFrame)
                {
                    _gameRef.SpriteBatch.Draw(_gunShootPixel, _gunShootPixelDrawPosition,
                        new Rectangle((int)_gunShootPixelDrawPosition.X, (int)_gunShootPixelDrawPosition.Y,
                        (int)_bulletSize.X, (int)_bulletSize.Y), new Color(_gunShootPixelColor),
                        0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        private void OpenFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\000\\Programming\\The Lost Hope\\The Lost Hope\\Content\\AsepriteFiles";
                openFileDialog.Filter = "aseprite file *.aseprite|*.aseprite";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    _asepriteFilePath = openFileDialog.FileName;

                    // Load aseprite file from path
                    _gunAnimator = new Animator(AsepriteFile.Load(_asepriteFilePath), _gameRef.GraphicsDevice);

                    // Set active animation to idle
                    _gunCurrentAnimationIndex = 0;
                    _gunAnimator.SetActiveAnimation("Idle");

                    _gunIsAnimationPlaying = true;
                    _gunAnimationInShootFrame = false;

                    // Set gun drawing properties
                    _gunDrawPosition = new System.Numerics.Vector2(-60, 0);

                    // Find Shoot frame
                    int counter = 0;
                    foreach (var shootFrame in _gunAnimator.Animations["Shoot"].AnimationFrames)
                    {
                        if (shootFrame.Value.TriggerEvent)
                        {
                            _gunShootFrameIndex = counter;
                            break;
                        }
                        counter++;
                    }
                    _gunShootPixelDrawPosition = _gunDrawPosition;
                }
            }
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

            string createNewWeaponWindowOpen = _createNewWeaponWindow ? "Open" : "Closed";
            if (ImGui.Button($"Create new gun ({createNewWeaponWindowOpen})"))
            {
                _createNewWeaponWindow = !_createNewWeaponWindow;
            }

            if (_createNewWeaponWindow)
            {
                if (ImGui.Begin("Create Gun"))
                {
                    ImGui.BeginChild("Scrolling");

                    ImGui.Spacing();
                    ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Base");

                    ImGui.SliderFloat("Camera Zoom", ref _cameraZoom, 0.1f, 20f);
                    _gameRef.Camera.Zoom = _cameraZoom;

                    if (ImGui.Button("Load Gun Aseprite File"))
                    {
                        OpenFileDialog();
                    }
                    if (_gunAnimator != null)
                    {
                        ImGui.SliderFloat2("Gun Position", ref _gunDrawPosition, -550f, 550f);


                        ImGui.Spacing();
                        ImGui.Separator();
                        ImGui.Spacing();
                        ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Animations");
                        ImGui.Spacing();
                        ImGui.TextColored(new System.Numerics.Vector4(0, 1, 0, 1), $"Current Animation: {(string)_gunAnimator.Animations.ElementAt(_gunCurrentAnimationIndex).Key}");

                        if (ImGui.BeginCombo("Animations", (string)_gunAnimator.Animations.ElementAt(_gunCurrentAnimationIndex).Key))
                        {
                            for (int i = 0; i < _gunAnimator.Animations.Count; i++)
                            {
                                bool is_selected = (_gunCurrentAnimationIndex == i);
                                if (ImGui.Selectable((string)_gunAnimator.Animations.ElementAt(i).Key, is_selected))
                                {
                                    _gunCurrentAnimationIndex = i;
                                    _gunAnimator.SetActiveAnimation(_gunAnimator.Animations.ElementAt(_gunCurrentAnimationIndex).Key);

                                    _gunAnimationInShootFrame = false;
                                }
                                if (is_selected)
                                {
                                    ImGui.SetItemDefaultFocus();
                                }
                            }
                            ImGui.EndCombo();
                        }
                        if (ImGui.SmallButton(_gunIsAnimationPlaying ? "Pause" : "Play"))
                        {
                            if (_gunIsAnimationPlaying)
                            {
                                _gunAnimator.CurrentAnimation.Stop(false);
                                _gunIsAnimationPlaying = false;
                            }
                            else
                            {
                                _gunAnimator.CurrentAnimation.Start();
                                _gunIsAnimationPlaying = true;
                            }

                            _gunAnimationInShootFrame = false;
                        }
                        if (ImGui.SmallButton("Reset"))
                        {
                            _gunAnimator.CurrentAnimation.Stop();

                            _gunIsAnimationPlaying = false;
                            _gunAnimationInShootFrame = false;
                        }

                        ImGui.Spacing();
                        if (ImGui.Button("Set Animation to shoot frame"))
                        {
                            _gunAnimator.SetActiveAnimation("Shoot");
                            _gunAnimator.CurrentAnimation.Stop(true, _gunShootFrameIndex);

                            _gunCurrentAnimationIndex = 1;

                            _gunAnimationInShootFrame = true;
                        }

                        ImGui.TextColored(_gunAnimationInShootFrame ? new System.Numerics.Vector4(0, 1, 0, 1) :
                            new System.Numerics.Vector4(1, 0, 0, 1), $"In Shoot Frame = {_gunAnimationInShootFrame}");
                    }

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                    ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Name and Content File Path");
                    ImGui.InputTextWithHint("Name", "The Name of the gun",
                        ref _gunName, 20);
                    ImGui.InputTextWithHint("Aseprite Path", "Path to the aseprite file in the game content",
                        ref _gunAsepriteFilePath, 100);

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                    ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Firing Properties");
                    ImGui.InputInt("Magazine Size", ref _gunMagSize);
                    ImGui.InputFloat("Time Between Shots", ref _gunTimeBetweenShots);
                    ImGui.InputFloat("Player Knockback Force", ref _gunPlayerKnockbackForceOnFire);


                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                    ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Bullet Properties");

                    ImGui.InputFloat("Speed", ref _bulletSpeed);
                    ImGui.InputInt("Damge", ref _bulletDamage);

                    ImGui.InputFloat("Max Range", ref _bulletMaxRange);
                    if (_bulletMaxRange < 0) _bulletMaxRange = -1f;

                    ImGui.InputFloat("Max Life Time", ref _bulletMaxLifeTime);
                    if (_bulletMaxLifeTime < 0) _bulletMaxLifeTime = -1f;

                    ImGui.InputFloat("Gravity Scale", ref _bulletGravityScale);
                    ImGui.InputFloat("Enemy Knockback Force", ref _bulletEnemyKnockbackForceOnHit);
                    ImGui.SliderInt("Penetration", ref _bulletPeneratration, -1, 30);
                    ImGui.SliderInt("Bounciness", ref _bulletBounciness, -1, 20);

                    if (_gunAnimationInShootFrame)
                    {
                        ImGui.ColorEdit4("Shoot Pixel Color (debug)", ref _gunShootPixelColor);

                        ImGui.SliderFloat2("Bullet Shoot Position", ref _gunShootPixelPosition, 0f, 60f);
                        ImGui.SliderFloat2("Bullet Size", ref _bulletSize, 1f, 30f);
                    }
                    else
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(1f, 0f, 0f, 1f), "Set Animation Frame to shoot frame to edit bullet fire pos and size");
                    }

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                    ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Reloading");

                    if (ImGui.SmallButton("Add UP Reload Pattern"))
                    {
                        _gunReloadPattern.Add(GunReloadPattern.Up);
                    }
                    if (ImGui.SmallButton("Add DOWN Reload Pattern"))
                    {
                        _gunReloadPattern.Add(GunReloadPattern.Down);
                    }
                    if (ImGui.SmallButton("Add LEFT Reload Pattern"))
                    {
                        _gunReloadPattern.Add(GunReloadPattern.Left);
                    }
                    if (ImGui.SmallButton("Add RIGHT Reload Pattern"))
                    {
                        _gunReloadPattern.Add(GunReloadPattern.Right);
                    }
                    if (ImGui.SmallButton("Remove Last Reload Pattern"))
                    {
                        if (_gunReloadPattern.Count > 0)
                        {
                            _gunReloadPattern.RemoveAt(_gunReloadPattern.Count - 1);
                        }
                    }
                    ImGui.TextColored(new System.Numerics.Vector4(0, 1, 0, 1), "Current Pattern:");
                    ImGui.BeginChild("Scrolling", new System.Numerics.Vector2(100, 80));
                    for (int i = 0; i < _gunReloadPattern.Count; i++)
                    {
                        ImGui.Text($"{i}. " + _gunReloadPattern[i].ToString());
                    }
                    ImGui.EndChild();
                    ImGui.InputInt("Failed Reload Mag Count", ref _gunFailedReloadMag);


                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();
                    ImGui.TextColored(new System.Numerics.Vector4(1, 1, 0, 1), "Editor");
                    if (ImGui.Button("Save Gun"))
                    {
                        // TODO: Save the gun
                    }
                    if (ImGui.Button("Reset"))
                    {
                        // TODO: Reset variables
                    }

                    ImGui.EndChild();
                    ImGui.End();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_gunAnimator != null)
            {
                _gunShootPixelDrawPosition = _gunDrawPosition + _gunShootPixelPosition;

                _gunAnimator.Update(gameTime);
            }
        }
    }
}
