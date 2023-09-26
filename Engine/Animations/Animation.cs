using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace LostHope.Engine.Animations
{
    public class AnimationFrame
    {
        // The X and Y positions of the frame in the texture
        public int X, Y;
        // The width and height of the frame
        public int W, H;
        // The duration of the frame
        public float Duration;
        // The source rectangle of the frame,
        // this is used to render the frame from the spritesheet/texture
        public Rectangle Rectangle;
        // The name of the frame event. If it's equal to "", then there is not event associated with this frame
        public bool TriggerEvent { get; private set; }

        // Constructor
        public AnimationFrame(int x, int y, int width, int height, float duration, bool triggerEvent = false)
        {
            // The the X and Y positions
            X = x;
            Y = y;

            // Set the width and height
            W = width;
            H = height;

            // Set the rectangle
            Rectangle = new Rectangle(X, Y, W, H);

            // Set the duration and the event
            Duration = duration;

            // Set the trigger event bool
            TriggerEvent = triggerEvent;
        }
    }

    public class Animation
    {
        // Triggered when the animation encounters an event in a frame
        public event Action OnAnimationFrameEvent;
        // Triggered when the animation finishes
        public event Action OnAnimationFinished;

        // Dictionary of frame indexes as keys and frames as values
        private Dictionary<int, AnimationFrame> _animation;

        // The index of the currently playing frame
        private int _currentFrame;
        // Frame timer
        private float _timer;

        // Is the animation active
        private bool _active;
        // Should the animation loop
        private bool _loop;


        public Animation(List<AnimationFrame> frames, bool loop = true)
        {
            // Initialize variables
            _active = false;
            _loop = loop;

            _currentFrame = 0;
            _timer = 0;

            _animation = new Dictionary<int, AnimationFrame>();


            // Loops through all the frames provided in the constructor, sets up up their rectangles,
            // and adds them to the animation dict
            for (int i = 0; i < frames.Count; i++)
            {
                var frame = new AnimationFrame(frames[i].X, frames[i].Y,
                    frames[i].W, frames[i].H, frames[i].Duration, frames[i].TriggerEvent);

                _animation.Add(i, frame);
            }
        }

        // Stops the animation
        public void Stop(bool resetFrame = true)
        {
            Reset(resetFrame);
            _active = false;
        }

        // Starts the animation
        public void Start()
        {
            Reset();
            _active = true;

            var frame = GetCurrentAnimationFrame();

            if (frame == null)
            {
                return;
            }

            // If the frame isn't null, we invoke it's event because we just started the animation
            if (frame.TriggerEvent)
            {
                OnAnimationFrameEvent?.Invoke();
            }
        }

        // Resets the animation
        public void Reset(bool resetFrame = true)
        {
            if (resetFrame) _currentFrame = 0;

            _timer = 0;
        }

        // Updates the animation
        public void Update(GameTime gameTime, float animationSpeedMultiplier = 1f)
        {
            // If the animation is not active, we return.
            if (!_active) return;

            // Increment the timer.
            _timer += ((float)gameTime.ElapsedGameTime.TotalSeconds * animationSpeedMultiplier);

            // Get the currently active frame
            var frame = GetCurrentAnimationFrame();

            // If it is null that means the frame doesn't exist, we return.
            if (frame == null)
            {
                return;
            }

            // If the timer exceeds the active frame's duration, the frame ended
            if (_timer > frame.Duration)
            {
                // Reset the timer
                _timer = 0;

                // If the animation is finished and loop is false
                if (_currentFrame == _animation.Count - 1 && !_loop)
                {
                    // Invoke animation finished
                    OnAnimationFinished?.Invoke();

                    // Stop the animation
                    Stop(false);
                }
                else
                {
                    // If not, we increment the current frame
                    _currentFrame = (_currentFrame + 1) % _animation.Count;
                    
                    // Invoke the frame's event if it has one
                    if (GetCurrentAnimationFrame().TriggerEvent)
                    {
                        OnAnimationFrameEvent?.Invoke();
                    }    
                }
            }
        }

        // Returns the source rectangle of the currently active frame,
        // This is used for rendering the frames from a texture.
        public Rectangle GetSourceRectangle()
        {
            var frame = GetCurrentAnimationFrame();

            if (frame != null)
            {
                return frame.Rectangle;
            }
            else
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }

        // Returns the currently active frame from the dict, using the current frame index.
        public AnimationFrame GetCurrentAnimationFrame()
        {
            AnimationFrame frame = null;
            _animation.TryGetValue(_currentFrame, out frame);
            return frame;
        }
    }
}
