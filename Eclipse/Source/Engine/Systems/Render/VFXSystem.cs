using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Systems.Render
{
    internal class VFXSystem : ComponentSystem<VFXSource>
    {
        public override void Update(GameTime gameTime)
        {
            foreach (var source in _components)
            {
                if (!source.IsEnabled || !source.GameObject.IsActive) continue;
                if (!source.IsPlaying) continue;

                // Cleanup finished effects
                VFXManager.Instance.UpdateInstances(source);

                // Update 3D effects if needed
                //if (source.Is3D)
                //{
                //    Update3DEffect(source);
                //}
            }
        }

        internal void Update3DEffect(VFXSource source)
        {
            var listenerPos = PlayerManager.Instance.GetPlayerPosition();
            var sourcePos = source.GameObject.Transform.WorldPosition;
            var distance = Vector2.Distance(listenerPos, sourcePos);

            if (distance < 0.5f) return; // Skip if too close

            var scale = source.Scale;
            // Scale down with distance
            //if (distance > source.MinDistance)
            //{
            //    scale *= 1.0f - Math.Min((distance - source.MinDistance) /
            //        (source.MaxDistance - source.MinDistance), 1.0f);
            //}

            VFXManager.Instance.UpdateSource(source, scale);
        }

        public void Draw()
        {
            VFXManager.Instance.DrawInstances();
        }
    }
}
