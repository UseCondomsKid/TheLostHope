using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame.Aseprite.Content.Processors;
using MonoGame.Aseprite.Sprites;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        // In case of using Monogame.Aseprite
        public Texture2D SpriteSheetTexture { get; private set; }

        public Point AnimationCanvasSize { get; private set; }

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

            var spriteSheet = SpriteSheetProcessor.Process(graphicsDevice, asepriteFile, mergeDuplicates: false);
            SpriteSheetTexture = spriteSheet.TextureAtlas.Texture;

            List<AnimationFrame> frames = new List<AnimationFrame>();
            foreach (var tagName in spriteSheet.GetAnimationTagNames())
            {
                var animationTag = spriteSheet.GetAnimationTag(tagName);
                AnimationCanvasSize = animationTag.Frames[0].TextureRegion.Bounds.Size;

                frames = new List<AnimationFrame>();

                foreach(var frame in animationTag.Frames)
                {
                    frames.Add(new AnimationFrame(frame.TextureRegion.Bounds.X,
                        frame.TextureRegion.Bounds.Y, frame.TextureRegion.Bounds.Width,
                        frame.TextureRegion.Bounds.Height, (float)frame.Duration.TotalSeconds));
                }
                Animation animation = new Animation(frames, animationTag.IsLooping);
                AddAnimation(tagName, animation);
            }
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
                _currentAnimation.Update(gameTime);
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
