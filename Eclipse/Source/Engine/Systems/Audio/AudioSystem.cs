using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Components.Engine;
using Eclipse.Engine.Managers;
using System;

namespace Eclipse.Engine.Systems.Audio
{
    internal class AudioSystem : ComponentSystem<AudioSource>
    {
        public override void Update(GameTime gameTime)
        {
            foreach (var source in _components)
            {
                if (!source.IsEnabled || !source.GameObject.IsActive) continue;
                if (!source.IsPlaying) continue;

                // Cleanup finished sounds (instances of source)
                AudioManager.Instance.UpdateInstances(source);

                // Update spatial audio if needed
                if (source is SoundEffectSource soundSource &&
                    soundSource.Is3D)
                {
                    UpdateSpatialAudio(soundSource);
                }

            }
        }
        internal void UpdateSpatialAudio(SoundEffectSource source)
        {
            var listenerPos = PlayerManager.Instance.GetPlayerPosition();
            var sourcePos = source.GameObject.Transform.WorldPosition;

            var distance = Vector2.Distance(listenerPos, sourcePos);

            if (distance < 0.5f) return; // Skip if too close

            var volume = source.Volume;
            if (distance > source.MinDistance)
            {
                volume *= 1.0f - Math.Min((distance - source.MinDistance) /
                    (source.MaxDistance - source.MinDistance), 1.0f);
            }

            Vector2 direction = sourcePos - listenerPos;
            float pan = (direction.X / (source.MaxDistance * 0.5f));
            pan = Math.Clamp(pan, -1f, 1f);

            AudioManager.Instance.UpdateSource(source, volume, pan);
        }
    }
}
