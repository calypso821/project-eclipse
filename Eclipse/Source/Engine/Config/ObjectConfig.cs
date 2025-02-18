using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Eclipse.Components.Animation;
using Eclipse.Engine.Managers;
using Eclipse.Components.Combat;
using Eclipse.Engine.Core;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Config
{
    // Base configuration class (.JSON file strucutre)
    // Used during Object Cration (factory)

    public class ObjectConfig
    {
        // TODO: Maybe placeholder??
        public string SpriteId { get; set; }
        public AnimationConfig[] SpriteAnimations { get; set; }
        public AnimationConfig[] OverlayAnimations { get; set; }
        public AnimationConfig[] VisualEffects { get; set; }
        public Dictionary<string, AudioData> SoundEffects { get; set; } = new();
        public TweenConfig[] TransformAnimations { get; set; }
        public virtual Vector2Config Offset { get; set; } = new Vector2Config(0f, 0f);
        public float Rotation { get; set; } = 0f;
        public Vector2Config Scale { get; set; } = new Vector2Config(1.0f, 1.0f);

        internal Dictionary<string, AnimationData> GetSpriteAnimations()
        {
            return GetSpriteAnimations(SpriteAnimations);
        }
        internal Dictionary<string, AnimationData> GetOverlayAnimations()
        {
            return GetSpriteAnimations(OverlayAnimations);
        }
        internal Dictionary<string, AnimationData> GetVisualEffects()
        {
            return GetSpriteAnimations(VisualEffects);
        }

        private Dictionary<string, AnimationData> GetSpriteAnimations(AnimationConfig[] spriteAnimations)
        {
            if (spriteAnimations == null || spriteAnimations.Length == 0) return new();

            var animations = new Dictionary<string, AnimationData>();
            foreach (var config in spriteAnimations)
            {
                ValidateSpriteAnimation(config);

                animations[config.Name] = new AnimationData(
                    spriteId: config.SpriteId,
                    customOrigin: config.Origin != null ? config.Origin.ToVector2() : null,
                    duration: config.Duration,
                    isLooping: config.IsLooping
                );
            }
            return animations;
        }

        internal Dictionary<string, TweenData> GetTransformAnimations()
        {
            if (TransformAnimations == null || TransformAnimations.Length == 0) return new();

            var animations = new Dictionary<string, TweenData>();
            foreach (var config in TransformAnimations)
            {
                ValidateTransformAnimation(config);
                animations[config.Name] = new TweenData(config);
            }
            return animations;
        }

        private void ValidateSpriteAnimation(AnimationConfig config)
        {
            if (string.IsNullOrEmpty(config.Name))
                throw new ArgumentException($"Animation name cannot be empty");
            if (string.IsNullOrEmpty(config.SpriteId))
                throw new ArgumentException($"SpriteId cannot be empty for animation {config.Name}");
            if (config.Duration <= 0)
                throw new ArgumentException($"Duration must be positive for animation {config.Name}");
        }

        private void ValidateTransformAnimation(TweenConfig config)
        {
            if (string.IsNullOrEmpty(config.Name))
                throw new ArgumentException("Tween animation name cannot be empty");
            if (config.Duration <= 0)
                throw new ArgumentException($"Duration must be positive for tween {config.Name}");
        }
    }
    public class Vector2Config
    {
        public float X { get; set; } = 0.0f;
        public float Y { get; set; } = 0.0f;

        public Vector2Config(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vector2 ToVector2() => new Vector2(X, Y);
    }





}