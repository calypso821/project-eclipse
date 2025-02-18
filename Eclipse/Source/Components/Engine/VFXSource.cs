using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;
using Eclipse.Components.Animation;
using Eclipse.Engine.Managers;
using System;

namespace Eclipse.Components.Engine
{
    internal class VFXSource : Component
    {
        // Dictionary of predefined effects this source can play
        private Dictionary<string, AnimationData> _effects = new();

        // Settings
        internal bool AllowOverlap { get; set; } = true;

        // True = static position (at Play)
        // False = follows source object position
        internal bool IsStatic { get; set; } = true;  

        internal override void OnReset()
        {
            Stop();
        }

        internal void AddEffect(string effectId, AnimationData animation)
        {
            _effects[effectId] = animation;
        }
        internal void AddEffects(Dictionary<string, AnimationData> animations)
        {
            _effects = animations;
        }

        internal void Play(
            string effectId,
            Vector2 spawnPosition,
            Vector2 normal,
            float scale = 1.0f,
            Color? color = null)
        {
            if (!_effects.TryGetValue(effectId, out var effect))
            {
                //Console.WriteLine($"VisualEffect {effectId} not found.");
                return;
            }

            if (!AllowOverlap)
            {
                Stop();
            }

            var vfxData = new VFXData
            {
                Scale = scale,
                Color = color ?? Color.White,
                SpawnPosition = spawnPosition,
                Direction = normal
            };

            VFXManager.Instance.PlayEffect(
                this,
                effectId,
                effect,
                vfxData
            );
        }

        internal void Stop(string effectId = null)
        {
            VFXManager.Instance.StopEffect(this, effectId);
        }
    }
}
