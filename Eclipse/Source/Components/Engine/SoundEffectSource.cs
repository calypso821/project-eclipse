using System;
using Eclipse.Components.Engine;
using Eclipse.Engine.Managers;

namespace Eclipse.Components.Engine
{
    internal class SoundEffectSource : AudioSource
    {
        internal override bool AllowOverlap { get; set; } = true;

        // 3D audio settings
        internal override bool Is3D { get; set; } = true;
        internal float MinDistance { get; set; } = 1f;
        internal float MaxDistance { get; set; } = 20f;

        internal void Play(string soundId, bool randomPitch = false)
        {
            if (!AllowOverlap)
            {
                Stop();
            }

            // Reset pitch to base value and then randomize if needed
            Pitch = randomPitch ?
                BasePitch + (float)(new Random().NextDouble() * 0.2 - 0.1) :
                BasePitch;

            AudioManager.Instance.PlaySound(this, soundId);
        }

        internal override void Stop()
        {
            AudioManager.Instance.StopSound(this);
        }
    }
}
