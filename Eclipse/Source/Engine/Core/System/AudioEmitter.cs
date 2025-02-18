using System;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

using Eclipse.Components.Engine;
using Eclipse.Engine.Managers;


namespace Eclipse.Engine.Core
{
    internal class SFXData
    {
        internal float Volume { get; set; } = 1f;
        internal float Pitch { get; set; } = 0f;
        internal float Pan { get; set; } = 0f;
        internal bool Loop { get; set; } = false;
    }

    internal class AudioEmitter
    {
        internal string SoundId { get; private set; }
        internal SoundEffectInstance SoundInstance { get; private set; }
        internal SFXData SFXData { get; private set; }
        internal SFXSource AudioSource { get; private set; }
        internal bool IsPlaying => SoundInstance?.State == SoundState.Playing;

        private bool _needInstanceUpdate = false;

        internal void Configure(string soundId, SFXSource source, SFXData sfxData)
        {
            SoundId = soundId;
            AudioSource = source;
            SFXData = sfxData;

            // Set initial position if 3D
            UpdateSpatialAudio();
        }

        internal void Play(SoundEffect sound)
        {
            Stop();  // Stop any existing playback

            SoundInstance = sound.CreateInstance();
            SoundInstance.IsLooped = SFXData.Loop;

            // Set instacne parameters (volume, pitch, pan)
            UpdateInstance();

            SoundInstance.Play();
        }

        internal void Stop()
        {
            if (SoundInstance != null)
            {
                SoundInstance.Stop();
                SoundInstance.Dispose();
                SoundInstance = null;
            }
        }

        internal void Update()
        {
            UpdateSpatialAudio();

            if (_needInstanceUpdate)
            {
                UpdateInstance();
            }
        }

        internal void UpdateSpatialAudio()
        {
            if (!AudioSource.Is3D) return;

            var listenerPos = PlayerManager.Instance.GetPlayerPosition();
            var sourcePos = AudioSource.GameObject.Transform.WorldPosition;
            var distance = Vector2.Distance(listenerPos, sourcePos);

            if (distance < 0.5f) return; // Skip if too close

            var volume = SFXData.Volume;
            if (distance > AudioSource.MinDistance)
            {
                volume *= 1.0f - Math.Min((distance - AudioSource.MinDistance) /
                        (AudioSource.MaxDistance - AudioSource.MinDistance), 1.0f);
            }

            Vector2 direction = sourcePos - listenerPos;
            float pan = (direction.X / (AudioSource.MaxDistance * 0.5f));

            // Set paraemters
            SFXData.Pan = MathHelper.Clamp(pan, -1f, 1f);
            SFXData.Volume = MathHelper.Clamp(volume, 0f, 1f);

            _needInstanceUpdate = true;
        }

        internal void UpdateInstance()
        {
            if (SoundInstance != null)
            {
                SoundInstance.Volume = SFXData.Volume;
                SoundInstance.Pitch = SFXData.Pitch;
                SoundInstance.Pan = SFXData.Pan;
            }
            _needInstanceUpdate = false;
        }
    }
}
