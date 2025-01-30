using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities.Deflate;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Components.Animation;
using Eclipse.Components.Engine;
using Eclipse.Engine.Core;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.Scenes;

namespace Eclipse.Engine.Managers
{
    public class VFXManager : Singleton<VFXManager>
    {
        private readonly Dictionary<int, VFXEmitter> _activeEmitters = new();
        private readonly Queue<VFXEmitter> _emitterPool = new();
        private SpriteBatch _spriteBatch;

        private const int POOL_SIZE = 32;
        private VFXSystem _vfxSystem;

        internal void Initialize(VFXSystem vfxSystem, SpriteBatch spriteBatch)
        {
            _vfxSystem = vfxSystem;
            _spriteBatch = spriteBatch;

            // Initialize emitter pool
            for (int i = 0; i < POOL_SIZE; i++)
            {
                _emitterPool.Enqueue(new VFXEmitter());
            }
        }

        internal void PlayEffect(VFXSource source, string effectId)
        {
            if (source == null || string.IsNullOrEmpty(effectId)) return;

            // Get emitter from pool
            if (!_emitterPool.TryDequeue(out var emitter))
            {
                return;
            }

            // Get effect data
            //var effectData = AssetManager.Instance.GetVFXData(effectId);
            //if (effectData == null)
            //{
            //    _emitterPool.Enqueue(emitter);
            //    return;
            //}

            //// Setup emitter
            //var instanceId = IDManager.GetId();
            //source.ActiveInstances.Add(instanceId);

            //// Track active emitter
            //_activeEmitters[instanceId] = emitter;

            //// Initialize emitter with effect data
            //emitter.Initialize(effectData, source.GameObject.Transform.WorldPosition);
        }

        internal void StopEffect(VFXSource source)
        {
            foreach (var instanceId in source.ActiveInstances)
            {
                ReleaseEmitter(instanceId);
            }
            source.ActiveInstances.Clear();
        }

        internal void UpdateSource(VFXSource source, float scale)
        {
            foreach (var instanceId in source.ActiveInstances)
            {
                //if (_activeEmitters.TryGetValue(instanceId, out var emitter))
                //{
                //    // Update emitter parameters as needed
                //    // This might need adjustment based on your VFXEmitter implementation
                //    emitter.Scale = scale;
                //}
            }
        }

        internal void UpdateInstances(VFXSource source)
        {
            foreach (var instanceId in source.ActiveInstances.ToList())
            {
                if (ReleaseEmitter(instanceId, forceStop: false))
                {
                    source.ActiveInstances.Remove(instanceId);
                }
            }
        }

        private bool ReleaseEmitter(int instanceId, bool forceStop = true)
        {
            if (_activeEmitters.TryGetValue(instanceId, out var emitter))
            {
                if (forceStop || !emitter.IsActive)
                {
                    _emitterPool.Enqueue(emitter);
                    _activeEmitters.Remove(instanceId);
                    IDManager.ReleaseId(instanceId);
                    return true;
                }
            }
            return false;
        }

        internal void DrawInstances()
        {
            foreach (var emitter in _activeEmitters.Values)
            {
                // Actual drawing happens in VFXEmitter
                // This allows for proper batching and sorting if needed
                if (emitter.IsActive)
                {
                    emitter.Draw(_spriteBatch);
                }
            }
        }

        internal void Cleanup()
        {
            foreach (var kvp in _activeEmitters)
            {
                ReleaseEmitter(kvp.Key, true);
            }

            _activeEmitters.Clear();
            _emitterPool.Clear();
        }
    }
}
