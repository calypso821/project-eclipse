using Eclipse.Components.Animation;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace Eclipse.Components.Engine
{
    public class AudioData
    {
        public string AudioId { get; set; }
        public float Volume { get; set; } = 1f;
        public float Pitch { get; set; } = 0f;
        public float Pan { get; set; } = 0f;
        public bool Loop { get; set; } = false;
        public bool RandomPitch { get; set; } = false;
    }

    internal class SFXSource : Component
    {

        // Dictionary of predefined effects this source can play
        private Dictionary<string, AudioData> _effects = new();

        internal  bool AllowOverlap { get; set; } = true;

        // 3D audio settings
        internal bool Is3D { get; set; } = true;
        internal float MinDistance { get; set; } = 1f;
        internal float MaxDistance { get; set; } = 20f;

        internal override void OnReset()
        {
            Stop();
        }
        internal void AddEffect(string effectId, AudioData audioData)
        {
            _effects[effectId] = audioData;
        }
        internal void AddEffects(Dictionary<string, AudioData> soundEffects)
        {
            _effects = soundEffects;
        }

        internal void Play(string soundId)
        {
            if (!_effects.TryGetValue(soundId, out var sound))
            {
                //Console.WriteLine($"SoundEffect {soundId} not found.");
                return;
            }

            if (!AllowOverlap)
            {
                Stop();
            }



            AudioManager.Instance.PlaySound(this, soundId, sound);
        }

        internal void Stop(string soundId = null)
        {
            AudioManager.Instance.StopSound(this, soundId);
        }
    }
}
