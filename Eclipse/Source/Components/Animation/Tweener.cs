using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Eclipse.Engine.Config;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Animation
{
    public class TweenData  // Similar to AnimationData
    {
        internal Vector2 StartPosition { get; }
        internal Vector2 EndPosition { get; }
        internal float StartRotation { get; }
        internal float EndRotation { get; }
        internal Vector2 StartScale { get; }
        internal Vector2 EndScale { get; }
        internal float Duration { get; }
        internal bool PingPong { get; }
        internal bool IsLooping { get; }

        internal TweenData(
            Vector2? startPosition = null,
            Vector2? endPosition = null,
            float? startRotation = null,
            float? endRotation = null,
            Vector2? startScale = null,
            Vector2? endScale = null,
            float duration = 1.0f,
            bool pingPing = false,
            bool isLooping = false)
        {
            StartPosition = startPosition ?? Vector2.Zero;
            EndPosition = endPosition ?? Vector2.Zero;
            StartRotation = startRotation ?? 0f;
            EndRotation = endRotation ?? 0f;
            StartScale = startScale ?? Vector2.One;
            EndScale = endScale ?? Vector2.One;
            Duration = duration;
            PingPong = pingPing;
            IsLooping = isLooping;
        }

        internal TweenData(TweenConfig config)
        {
            StartPosition = config.StartPosition.ToVector2();
            EndPosition = config.EndPosition.ToVector2();
            StartRotation = config.StartRotation;
            EndRotation = config.EndRotation;
            StartScale = config.StartScale.ToVector2();
            EndScale = config.EndScale.ToVector2();
            Duration = config.Duration;
            PingPong = config.PingPong;
            IsLooping = config.IsLooping;
        }
    }

    internal sealed class Tweener : Animator
    {
        private Dictionary<string, TweenData> _animations = new();

        private Transform _transform;
        private float _currentTime;
        private float _lastProgress;

        // If PingPong = true
        private bool _isReversing;

        internal Tweener(Transform transform)
            : base()
        {
            _transform = transform;
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
        }

        internal void AddAnimation(string animationId, TweenData animation)
        {
            _animations[animationId] = animation;
        }
        internal bool HasAnimation(string animationId)
        {
            return _animations.ContainsKey(animationId);
        }
        internal void SetAnimations(Dictionary<string, TweenData> animations)
        {
            _animations = animations;
        }

        internal override void SetAnimation(string animationId, bool forceRestart = true)
        {
            // Validate input aniamtion first
            if (!_animations.ContainsKey(animationId))
            {
                Console.WriteLine($"Animation '{animationId}' does not exist");
                return;
            }

            // Then check if animation is already playing
            if (!forceRestart && CurrentAnimation == animationId)
                return;

            CurrentAnimation = animationId;

            // Reset to starting values
            var tween = _animations[animationId];
            UpdateTransform(0f);
            Play(forceRestart);
        }

        internal override void Update(GameTime gameTime)
        {
            if (!IsPlaying || !_animations.ContainsKey(CurrentAnimation))
                return;

            var currentTween = _animations[CurrentAnimation];
            // Decrease/Increase progress
            _currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds * (_isReversing ? -1 : 1);
            float progress = _currentTime / currentTween.Duration;

            // Increasing progress 0 -> 1 (start to end) 
            if (progress >= 1.0f)
            {
                if (currentTween.PingPong)
                {
                    // PingPong
                    _isReversing = true;
                    progress = 1.0f;
                }
                else if (currentTween.IsLooping)
                {
                    // Looping (start fresh)
                    _currentTime = 0;
                    progress = 0;
                }
                else
                {
                    // Stop playing (at end)
                    IsPlaying = false;
                    progress = 1.0f;
                }
            }

            // Decreasing progress 1 -> 0 (end to start) 
            if (_isReversing && progress < 0)
            {
                _isReversing = false;

                if (currentTween.IsLooping)
                {
                    // Looping (start fresh)
                    _currentTime = 0;
                    progress = 0;
                }
                else
                {
                    // Stop playing (at start)
                    IsPlaying = false;
                    progress = 0;
                }
            }

            //Console.WriteLine(progress);
            UpdateTransform(progress);
        }

        private void UpdateTransform(float progress)
        {
            var currentTween = _animations[CurrentAnimation];

            // Position lerp
            if (currentTween.StartPosition != currentTween.EndPosition)
            {
                // Used for attack transform Animtion 
                // Better practise to use sprite animation
                //var direction = GameObject.Transform.Direction.X > 0 ? 1 : -1;

                // Apply direction to end position
                var targetEndPos = new Vector2(
                    currentTween.EndPosition.X,  // Just flip direction
                    currentTween.EndPosition.Y  // Y stays the same
                );

                // Get delta position
                var prevPos = Vector2.Lerp(
                    currentTween.StartPosition,
                    targetEndPos,
                    _lastProgress
                );
                var newPos = Vector2.Lerp(
                    currentTween.StartPosition,
                    targetEndPos,
                    progress
                );

                var posDelta = newPos - prevPos;
                _transform.Translate(posDelta);
            }

            // Rotation lerp
            if (currentTween.StartRotation != currentTween.EndRotation)
            {
                // Get delta position
                var prevRotation = MathHelper.Lerp(
                    currentTween.StartRotation,
                    currentTween.EndRotation,
                    _lastProgress
                );
                var newRotation = MathHelper.Lerp(
                    currentTween.StartRotation,
                    currentTween.EndRotation,
                    progress
                );

                var rotationDelta = newRotation - prevRotation;
                _transform.Rotate(rotationDelta);

                // TOOD: PingPong rotation
                //Console.WriteLine($"Progress: {progress}, Reversing: {_isReversing}");
                //Console.WriteLine($"RotationDelta: {rotationDelta}");
                //Console.WriteLine($"Rotation: {_transform.Rotation}");
                //Console.WriteLine(_transform.Rotation);
            }

            // Scale lerp
            if (currentTween.StartScale != currentTween.EndScale)
            {
                // Get delta scale
                var prevScale = Vector2.Lerp(
                    currentTween.StartScale,
                    currentTween.EndScale,
                    _lastProgress
                );
                var newScale = Vector2.Lerp(
                    currentTween.StartScale,
                    currentTween.EndScale,
                    progress
                );
                var deltaScale = newScale / prevScale;
                _transform.ScaleBy(deltaScale); 
            }

            _lastProgress = progress;
        }

        internal override void Play(bool restart = true)
        {
            if (restart)
            {
                _currentTime = 0;
                _lastProgress = 0;
                UpdateTransform(0);
            }
            IsPlaying = true;
        }

        internal override void Stop()
        {
            IsPlaying = false;
            _currentTime = 0;
            _lastProgress = 0;
            UpdateTransform(0);
        }

        internal override void Pause()
        {
            IsPlaying = false;
        }
    }
}
