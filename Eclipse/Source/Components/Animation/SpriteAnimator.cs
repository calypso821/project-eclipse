using System.Collections.Generic;
using System;

using Microsoft.Xna.Framework;

using Eclipse.Components.Engine;
using Microsoft.Xna.Framework.Graphics;
using Eclipse.Engine.Core;
using Eclipse.Engine.Utils.Load.Assets;
using Eclipse.Engine.Managers;

namespace Eclipse.Components.Animation
{
    public class AnimationData  // Class, not struct
    {
        internal string SpriteId { get; }
        internal float Duration { get; }
        internal float FrameDuration { get; }
        internal int FrameCount { get; }
        internal bool IsLooping { get; }
        internal Vector2? CustomOrigin { get; }

        internal AnimationData(
            string spriteId,
            Vector2? customOrigin,
            float duration = 1.0f,
            bool isLooping = true)
        {
            SpriteId = spriteId;
            var frames = AssetManager.Instance.GetSprite(spriteId).Frames;
            Duration = duration;
            FrameDuration = duration / frames.Count;
            FrameCount = frames.Count;
            IsLooping = isLooping;
            CustomOrigin = customOrigin;
        }
    }

    internal class SpriteAnimator : Animator
    {
        private Dictionary<string, AnimationData> _animations = new();

        private Sprite _sprite;

        internal bool _isFlipped = false;
        internal bool FlipX
        {
            set
            {
                if (_isFlipped == value) return;
                _isFlipped = value;
                UpdateFlipState();
            }
            get => _isFlipped;
        }

        private float _currentFrameTime;
        private int _currentFrameIndex;


        internal SpriteAnimator(Sprite sprite)
            : base()
        {
            _sprite = sprite;
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
        }
        internal void ShowSprite() => _sprite.Enable();
        internal void HideSprite() => _sprite.Disable();

        internal override void OnDisable() => HideSprite();
        internal override void OnEnable() => ShowSprite();

        internal void AddAnimation(string animationId, AnimationData animation)
        {
            _animations[animationId] = animation;
        }
        internal void AddAnimations(Dictionary<string, AnimationData> animations)
        {
            _animations = animations;
        }
        internal bool HasAnimation(string animationId)
        {
            return _animations.ContainsKey(animationId);
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

            //Console.WriteLine("Animation: " + animationId);
            CurrentAnimation = animationId;

            // Update AnimatedSprite frames for new animation
            _sprite.SetAnimation(_animations[animationId]);

            Play(forceRestart);
            UpdateFrame();
        }

        internal override void Update(GameTime gameTime)
        {
            if (!IsPlaying || !_animations.ContainsKey(CurrentAnimation))
                return;

            var currentAnim = _animations[CurrentAnimation];
            if (currentAnim.FrameCount == 1) return; // Update not needed (static sprite)

            _currentFrameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_currentFrameTime >= currentAnim.FrameDuration)
            {
                _currentFrameTime = 0;
                _currentFrameIndex = (_currentFrameIndex + 1) % currentAnim.FrameCount;

                if (!currentAnim.IsLooping && _currentFrameIndex == 0)
                {
                    // No loop - only 1 itteration
                    IsPlaying = false;
                    _currentFrameIndex = currentAnim.FrameCount - 1;
                }

                UpdateFrame();
            }
        }

        private void UpdateFrame()
        {
            _sprite.CurrentFrameIndex = _currentFrameIndex;
        }

        internal override void Play(bool restart = true)
        {
            if (restart)
            {
                _currentFrameIndex = 0;
                _currentFrameTime = 0;
            }
            IsPlaying = true;
        }

        internal override void Stop()
        {
            IsPlaying = false;
            _currentFrameIndex = 0;
            _currentFrameTime = 0;
        }

        internal override void Pause()
        {
            IsPlaying = false;
        }

        // This method will be called whenever direction changes
        private void UpdateFlipState()
        {
            // Miror along the X-axis
            _sprite.FlipX = _isFlipped;
            UpdateChildrenFlipState(GameObject);
        }

        private void UpdateChildrenFlipState(GameObject gameObject)
        {
            foreach (var child in gameObject.Children)
            {
                var childSprites = child.GetComponents<Sprite>();
                foreach (var sprite in childSprites)
                {
                    sprite.FlipX = _isFlipped;
                }
                // Recurse into children
                UpdateChildrenFlipState(child);
            }
        }
    }
}
