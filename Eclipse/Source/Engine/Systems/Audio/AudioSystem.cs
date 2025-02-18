using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Components.Engine;
using Eclipse.Engine.Managers;
using System;
using System.Collections.Generic;

namespace Eclipse.Engine.Systems.Audio
{
    internal class AudioSystem : ISystem
    {
        private IReadOnlyDictionary<int, AudioEmitter> _activeEmitters;

        internal AudioSystem()
        {
            _activeEmitters = AudioManager.Instance.ActiveEmitters;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var kvp in _activeEmitters)
            {
                var emitter = kvp.Value;
                var emitterId = kvp.Key;

                // Release emitter if stopped playing
                if (!emitter.IsPlaying)
                {
                    AudioManager.Instance.ReleaseEmitter(emitterId);
                    continue;
                }

                emitter.Update(); // Update 3D spatial audio
            }
        }
    }
}
