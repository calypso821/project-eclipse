using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Eclipse.Engine.Core
{
    internal abstract class ComponentSystem : IComponentSystem
    {
        public virtual void Register(GameObject gameObject) { }
        public virtual void Unregister(GameObject gameObject) { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Cleanup() { } // Runtime cleanup (drity components)
        public virtual void Clear() { } // System reset (clear all components)
    }

    internal abstract class ComponentSystem<T> : ComponentSystem where T : Component
    {
        internal List<T> _components = new();
        internal bool _isDirty = false;

        // Virtual for systems that need multiple components like Animator
        public override void Register(GameObject gameObject)
        {
            var component = gameObject.GetComponent<T>();
            if (component == null) return;

            if (component.IsUnique)
            {
                // Single component
                RegisterComponent(component);
            }
            else
            {
                // Multiple components possible (of Type T)
                foreach (var comp in gameObject.GetComponents<T>())
                {
                    RegisterComponent(comp);
                }
            }
        }

        private void RegisterComponent(T component)
        {
            if (!component.IsRegistered)
            {
                _components.Add(component);
                component.IsRegistered = true;
            }
            component.ClearDirty();
        }

        public override void Unregister(GameObject gameObject)
        {
            _isDirty = true;
        }

        public override void Update(GameTime gameTime)
        {

            foreach (var component in _components)
            {
                if (!component.IsEnabled || !component.GameObject.IsActive) continue;

                component.Update(gameTime);
            }
        }

        public override void Cleanup()
        {
            if (_isDirty)
            {
                _components.RemoveAll(component =>
                {
                    if (component.IsDirty())
                    {
                        // Cleanup component flags
                        component.ClearDirty();
                        // Cleanup component states
                        component.IsRegistered = false;

                        return true;
                    }
                    return false;
                });
                _isDirty = false;
            }
        }

        public override void Clear()
        {
            _components.Clear();
            _isDirty = false;
        }
    }
}
