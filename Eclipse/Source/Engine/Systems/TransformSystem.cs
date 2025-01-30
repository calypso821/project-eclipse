using System.Collections.Generic;


using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;

namespace Eclipse.Engine.Systems
{
    internal class TransformSystem : ComponentSystem
    {
        internal List<Transform> _dirtyTransforms = new();

        public override void Register(GameObject gameObject)
        {
            var transform = gameObject.Transform;
            if (!transform.IsRegistered)  // Check registration state
            {
                _dirtyTransforms.Add(transform);
                transform.IsRegistered = true;
            }
            // Keep dirtyFlag for update()
        }
 
        // Unregister()
        // No need to Unregister
        // Just call gameObject.Transform.ClearDirty();

        // CleanUp()
        // No cleanup needed - components cleared after every Update()
     

        public override void Update(GameTime gameTime)
        {
            // Sort by dpeth 
            _dirtyTransforms.Sort((a, b) =>
                a.GameObject.Depth.CompareTo(b.GameObject.Depth));

            // Sort dirtTranfroms by Depth
            foreach (var transform in _dirtyTransforms)
            {
                // Unregister - processed
                transform.IsRegistered = false;

                // Skip if inactive
                if (!transform.GameObject.IsActive)
                    continue;

                // Update tranfrom
                // Check if tranfrom is dirty()
                // if processed by parent it could be already updated
                if (transform.GameObject.HasDirtyFlag(DirtyFlag.Transform))
                {
                    transform.UpdateWorldTransform();
                }

            }
            _dirtyTransforms.Clear();
        }

        public override void Clear()
        {
            _dirtyTransforms.Clear();
        }
    }
}
