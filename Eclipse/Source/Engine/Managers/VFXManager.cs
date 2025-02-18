using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Components.Animation;
using Eclipse.Components.Engine;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Managers
{
    internal class VFXManager : Singleton<VFXManager>
    {
        private readonly Dictionary<int, VFXEmitter> _activeEmitters = new();
        private readonly Queue<VFXEmitter> _emitterPool = new();

        internal IReadOnlyDictionary<int, VFXEmitter> ActiveEmitters => _activeEmitters;
        private const int POOL_SIZE = 32;

        internal void Initialize()
        {
            // Initialize emitter pool
            for (int i = 0; i < POOL_SIZE; i++)
            {
                _emitterPool.Enqueue(new VFXEmitter());
            }
        }

        internal void PlayEffect(
            VFXSource source,
            string effectId,
            AnimationData animData,
            VFXData vfxData)
        {
            if (source == null || string.IsNullOrEmpty(effectId)) return;

            // Get emitter from pool
            if (!_emitterPool.TryDequeue(out var emitter))
            {
                Console.WriteLine("VFX emitter pool exhausted");
                return;
            }

            // Setup emitter
            var instanceId = IDManager.GetId();

            // Track active emitter
            _activeEmitters[instanceId] = emitter;

            // Initialize emitter
            emitter.Configure(effectId, source, vfxData, animData);

            // Play 
            emitter.Play();
        }

        internal void StopEffect(VFXSource source, string effectId = null)
        {
            // Find all emitters for this source
            var emittersToStop = _activeEmitters
                .Where(kvp => kvp.Value.VFXSource == source &&
                        (effectId == null || kvp.Value.EffectId == effectId))
                .Select(kvp => kvp.Key)
                .ToList();

            // Release emitters
            foreach (var emitterId in emittersToStop)
            {
                ReleaseEmitter(emitterId);
            }
        }

        internal bool ReleaseEmitter(int emitterId)
        {
            if (_activeEmitters.TryGetValue(emitterId, out var emitter))
            {
                emitter.Stop();
                _activeEmitters.Remove(emitterId);
                _emitterPool.Enqueue(emitter);
                IDManager.ReleaseId(emitterId);
                return true;
            }
            return false;
        }

        internal void Cleanup()
        {
            foreach (var kvp in _activeEmitters)
            {
                ReleaseEmitter(kvp.Key);
            }

            _activeEmitters.Clear();
            _emitterPool.Clear();
        }
    }
}
