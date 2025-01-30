using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Components.Animation;

namespace Eclipse.Engine.Core
{
    public class VFXEmitter
    {
        private AnimationData _animData;
        private TweenData _tweenData;
        private float _currentTime;
        private float _frameTime;
        private int _currentFrame;
        private bool _isActive;
        private bool _pingPongReverse;
        private Color _color;

        // Color transitions (since not part of TweenData)
        private Color _startColor;
        private Color _endColor;

        public bool IsActive => _isActive;

        public void Initialize(VFXData data, Vector2 spawnPosition)
        {
            _animData = data.AnimationData;
            _tweenData = data.TweenData;

            // For effects that need offset from spawn position
            _tweenData = new TweenData(
                startPosition: spawnPosition + _tweenData.StartPosition,
                endPosition: spawnPosition + _tweenData.EndPosition,
                startRotation: _tweenData.StartRotation,
                endRotation: _tweenData.EndRotation,
                startScale: _tweenData.StartScale,
                endScale: _tweenData.EndScale,
                duration: _tweenData.Duration,
                pingPing: _tweenData.PingPong,
                isLooping: _tweenData.IsLooping
            );

            _startColor = data.StartColor;
            _endColor = data.EndColor;

            _currentTime = 0;
            _frameTime = 0;
            _currentFrame = 0;
            _pingPongReverse = false;
            _isActive = true;
        }

        public void Update(float deltaTime)
        {
            if (!_isActive) return;

            _currentTime += deltaTime;

            // Handle frame animation
            _frameTime += deltaTime;
            while (_frameTime >= _animData.FrameDuration)
            {
                _frameTime -= _animData.FrameDuration;
                _currentFrame++;

                if (_currentFrame >= _animData.FrameCount)
                {
                    if (_animData.IsLooping)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _isActive = false;
                        return;
                    }
                }
            }

            // Handle transformations using TweenData
            float progress = _currentTime / _tweenData.Duration;

            if (_currentTime >= _tweenData.Duration)
            {
                if (_tweenData.PingPong)
                {
                    _currentTime = 0;
                    _pingPongReverse = !_pingPongReverse;
                }
                else if (_tweenData.IsLooping)
                {
                    _currentTime = 0;
                }
                else
                {
                    _isActive = false;
                    return;
                }
            }

            // Adjust progress for ping-pong
            if (_pingPongReverse)
            {
                progress = 1 - progress;
            }

            _color = Color.Lerp(_startColor, _endColor, progress);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isActive) return;

            var frame = _animData.Frames[_currentFrame];
            var position = Vector2.Lerp(_tweenData.StartPosition, _tweenData.EndPosition, _currentTime / _tweenData.Duration);
            var rotation = MathHelper.Lerp(_tweenData.StartRotation, _tweenData.EndRotation, _currentTime / _tweenData.Duration);
            var scale = Vector2.Lerp(_tweenData.StartScale, _tweenData.EndScale, _currentTime / _tweenData.Duration);

            spriteBatch.Draw(
                _animData.Texture,
                position,
                frame.SourceRectangle,
                _color,
                rotation,
                frame.Origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public class VFXData
    {
        public AnimationData AnimationData { get; set; }
        public TweenData TweenData { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
    }
}
