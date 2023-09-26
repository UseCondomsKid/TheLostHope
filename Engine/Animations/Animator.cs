using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LostHope.Engine.Animations
{
    public class Animator
    {
        // Dictionary that houses all animations with each animation associated with an object,
        public Dictionary<object, Animation> Animations { get; private set; }

        // The animation that's currently playing
        private Animation _currentAnimation;

        // Public property to access the variable above
        public Animation CurrentAnimation { get { return _currentAnimation; } }
        // Public property to change the animation speed
        public float AnimationSpeedMultiplier { get; set; }
        public Texture2D SpriteSheetTexture { get; private set; }


        // Constructor that initializes the animations dict
        public Animator()
        {
            Animations = new Dictionary<object, Animation>();
        }

        // Constructor using the Monogame.Aseprite module
        public Animator(AsepriteExportData asepriteExportData)
        {
            Animations = new Dictionary<object, Animation>();
            if (asepriteExportData == null) return;

            SpriteSheetTexture = asepriteExportData.Texture;

            // Seperate events and animations
            List<FrameTag> frameTags = asepriteExportData.AsepriteData.Meta.FrameTags.ToList<FrameTag>();
            List<FrameTag> events = frameTags
                .Where(tag => tag.From == tag.To)
                .ToList();
            List<FrameTag> animations = frameTags
                .Where(tag => tag.From != tag.To)
                .ToList();

            foreach (var animation in animations)
            {
                List<AnimationFrame> frames = new List<AnimationFrame>();
                for (int i = (int)animation.From; i <= (int)animation.To; i++)
                {
                    var frameElement = asepriteExportData.AsepriteData.Frames[i];

                    bool triggerEvent = false;
                    foreach (var evt in events)
                    {
                        if ((int)evt.From == i)
                        {
                            triggerEvent = true;
                            break;
                        }
                    }

                    frames.Add(new AnimationFrame((int)frameElement.Frame.X, (int)frameElement.Frame.Y,
                        (int)frameElement.Frame.W, (int)frameElement.Frame.H, frameElement.Duration / 1000f,
                        triggerEvent));
                }

                Animation anim = new Animation(frames, animation.Repeat == null);
                AddAnimation(animation.Name, anim);
            }

            AnimationSpeedMultiplier = 1.0f;
        }

        // Adds animation to the dict
        public void AddAnimation(object key, Animation animation)
        {
            Animations.Add(key, animation);
        }

        // Sets the active animation
        public Animation SetActiveAnimation(object key)
        {
            // If an animation is already playing, we stop it.
            if (_currentAnimation != null)
            {
                _currentAnimation.Stop();
            }

            // Set new animation
            Animations.TryGetValue(key, out _currentAnimation);

            // If it is not null, we start it.
            if (_currentAnimation != null)
            {
                _currentAnimation.Start();
            }

            return _currentAnimation;
        }

        // Updates the current animation by calling it's update funtion
        public void Update(GameTime gameTime)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Update(gameTime, AnimationSpeedMultiplier);
            }
        }


        // Calls the GetSourceRectangle() from the currently active animation, and returns the rectangle it got.
        public Rectangle GetSourceRectangle()
        {
            Rectangle sourceRect = new Rectangle();

            if (_currentAnimation != null)
            {
                sourceRect = _currentAnimation.GetSourceRectangle();
            }

            return sourceRect;
        }
    }
}
