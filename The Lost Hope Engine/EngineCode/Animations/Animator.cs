using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Content.Processors;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Aseprite.Sprites;

namespace TheLostHopeEngine.EngineCode.Animations
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
        public Animator(AsepriteFile asepriteFile, GraphicsDevice graphicsDevice)
        {
            Animations = new Dictionary<object, Animation>();
            if (asepriteFile == null) return;

            var spriteSheet = SpriteSheetProcessor.Process(graphicsDevice, asepriteFile, mergeDuplicates: false, includeTilemapLayers: false,
                innerPadding: 1, borderPadding: 1);
            SpriteSheetTexture = spriteSheet.TextureAtlas.Texture;

            List<AnimationTag> events = new List<AnimationTag>();
            Dictionary<string, AnimationTag> animations = new Dictionary<string, AnimationTag>();

            foreach (var tagName in spriteSheet.GetAnimationTagNames())
            {
                var animationTag = spriteSheet.GetAnimationTag(tagName);

                if (animationTag.Frames.Length == 1)
                {
                    events.Add(animationTag);
                }
                else if (animationTag.Frames.Length > 1)
                {
                    animations.Add(tagName, animationTag);
                }
            }


            List<AnimationFrame> frames = new List<AnimationFrame>();
            foreach (var anim in animations)
            {
                frames = new List<AnimationFrame>();
                foreach (var frame in anim.Value.Frames)
                {
                    bool triggerEvent = false;
                    foreach (var evt in events)
                    {
                        if (evt.Frames[0].FrameIndex == frame.FrameIndex)
                        {
                            triggerEvent = true;
                            break;
                        }
                    }

                    frames.Add(new AnimationFrame(frame.TextureRegion.Bounds.X,
                        frame.TextureRegion.Bounds.Y, frame.TextureRegion.Bounds.Width,
                        frame.TextureRegion.Bounds.Height, (float)frame.Duration.TotalSeconds,
                        triggerEvent));
                }
                Animation animation = new Animation(frames, anim.Value.IsLooping);
                AddAnimation(anim.Key, animation);
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
