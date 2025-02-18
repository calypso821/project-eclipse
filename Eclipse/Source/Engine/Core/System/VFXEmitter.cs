using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Components.Animation;
using Eclipse.Components.Engine;
using Eclipse.Engine.Utils.Load.Assets;
using System;

namespace Eclipse.Engine.Core
{
    internal class VFXData
    {
        internal float Scale { get; set; } = 1f;
        internal Color Color { get; set; } = Color.White;
        internal Vector2 SpawnPosition { get; set; }
        internal Vector2 Direction { get; set; }
    }

    internal class VFXEmitter
    {
        internal string EffectId { get; private set; }
        internal VFXData VFXData { get; private set; }
        internal VFXSource VFXSource { get; private set; }
        internal AnimationData AnimationData { get; private set; }
        internal Vector2 CurrentPosition { get; set; }

        // For drawing
        private Sprite _sprite = new(); // empty sprite
        internal Sprite Sprite => _sprite;

        private float _currentFrameTime;
        private int _currentFrameIndex;

        private bool _isPlaying = false;
        internal bool IsPlaying => _isPlaying;

        private bool _flipX = false;
        internal SpriteEffects SpriteEffects =>
            _flipX ?
            SpriteEffects.FlipHorizontally :
            SpriteEffects.None;


        internal void Configure(
            string effectId,
            VFXSource source,
            VFXData vfxData,
            AnimationData animationData)
        {
            EffectId = effectId;
            VFXSource = source;
            VFXData = vfxData;
            AnimationData = animationData;

            // Store initial spawn position, direction
            CurrentPosition = VFXData.SpawnPosition;
            // TODO: Normal == direction?? 
            _flipX = VFXData.Direction.X < 0;
          
            // Set sprite
            _sprite.SetAnimation(animationData);
            // Update position if non static
            UpdatePosition();
        }

        internal void Update(GameTime gameTime)
        {
            if (!_isPlaying) return;

            UpdateAnimation(gameTime);
            UpdatePosition();
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            if (AnimationData.FrameCount == 1) return; // Update not needed (static sprite)
            
            _currentFrameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_currentFrameTime >= AnimationData.FrameDuration)
            {
                _currentFrameTime = 0;
                _currentFrameIndex = (_currentFrameIndex + 1) % AnimationData.FrameCount;

                if (!AnimationData.IsLooping && _currentFrameIndex == 0)
                {
                    // No loop - only 1 itteration
                    _isPlaying = false;
                    _currentFrameIndex = AnimationData.FrameCount - 1;
                }

                UpdateFrame();
            }
        }
        private void UpdateFrame()
        {
            _sprite.CurrentFrameIndex = _currentFrameIndex;
        }

        internal void UpdatePosition()
        {
            if (VFXSource.IsStatic) return;

            // Update source position (if not static)
            CurrentPosition = VFXSource.GameObject.Transform.WorldPosition;
        }

        internal void Play()
        {
            _currentFrameIndex = 0;
            _currentFrameTime = 0;
            _isPlaying = true;
        }

        internal void Stop()
        {
            _currentFrameIndex = 0;
            _currentFrameTime = 0;
            _isPlaying = false;
        }
    }
}
