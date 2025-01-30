using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;

namespace Eclipse.Engine.Systems
{
    internal class DestroySystem : ComponentSystem
    {
        private List<GameObject> _toDestroy = new();

        public override void Register(GameObject gameObject)
        {
            if (!gameObject.HasDirtyFlag(DirtyFlag.Destroy))
            {
                _toDestroy.Add(gameObject);
            }
            // Keep dirtyFlag for duplication check
        }

        public override void Update(GameTime gameTime)
        {
            // Skip if nothing to destroy
            if (_toDestroy.Count == 0) return;

            foreach (var destroyObject in _toDestroy)
            {
                // Call OnDestroy():
                // - GameObject
                // - all its compoennts
                // + Recursive on all children 
                destroyObject.Destroy();

                // Remove components from systems
                UnregisterComponents(destroyObject);
            }

            // Final removal from scene (clear parent)
            foreach (var destroyObject in _toDestroy)
            {
                destroyObject.Parent?.RemoveChild(destroyObject);
            }

            // Clear destroy list for next frame
            _toDestroy.Clear();

            // Final removal of components (pending)
            // Is done by CleanUp() - each 2s 
        }

        public override void Clear()
        {
            _toDestroy.Clear();
        }

        private void UnregisterComponents(GameObject gameObject)
        {
            // Mark all components dirty
            // Pending for removal from systems
            foreach (var component in gameObject.Components.Values)
            {
                component.Unregister();
            }

            // Remove children's components
            foreach (var child in gameObject.Children)
            {
                UnregisterComponents(child);
            }
        }
    }
}
