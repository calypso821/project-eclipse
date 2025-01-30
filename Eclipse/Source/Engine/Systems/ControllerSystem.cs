using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Eclipse.Components.Controller;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Systems
{
    internal class ControllerSystem : ComponentSystem
    {
        internal Dictionary<PassOrder, List<Controller>> _controllers;
        internal bool _isDirty = false;

        internal ControllerSystem()
        {
            // Initialize lists for all pass orders
            _controllers = new Dictionary<PassOrder, List<Controller>>
            {
                { PassOrder.Early, new List<Controller>() },
                { PassOrder.Default, new List<Controller>() },
                { PassOrder.Late, new List<Controller>() }
            };
        }

        public override void Register(GameObject gameObject)
        {
            if (gameObject.GetComponent<Controller>() == null) return;

            // Multiple components possible (of Type T)
            foreach (var component in gameObject.GetComponents<Controller>())
            {
                if (!component.IsRegistered)
                {
                    // Check if 
                    _controllers[component.PassOrder].Add(component);
                    component.IsRegistered = true;
                }
                component.ClearDirty();
            }
        }
        public override void Unregister(GameObject gameObject)
        {
            _isDirty = true;
        }

        public override void Update(GameTime gameTime)
        {
            // 1. Early update (cooldowns, timers)
            foreach (var component in _controllers[PassOrder.Early])
            {
                if (!component.IsEnabled || !component.GameObject.IsActive) continue;

                component.Update(gameTime);
            }
            // 2. Defualt update (core logic - playerController, AICOntroller)
            foreach (var component in _controllers[PassOrder.Default])
            {
                if (!component.IsEnabled || !component.GameObject.IsActive) continue;

                component.Update(gameTime);
            }
            // 3. Late update - MotionController (check groud sate)
            foreach (var component in _controllers[PassOrder.Late])
            {
                if (!component.IsEnabled || !component.GameObject.IsActive) continue;

                component.Update(gameTime);
            }
        }

        public override void Cleanup()
        {
            if (_isDirty)
            {
                CleanupList(PassOrder.Early);
                CleanupList(PassOrder.Default);
                CleanupList(PassOrder.Late);
                _isDirty = false;
            }
        }

        private void CleanupList(PassOrder passOrder)
        {
            _controllers[passOrder].RemoveAll(component =>
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
        }

        public override void Clear()
        {
            foreach (var list in _controllers.Values)
            {
                list.Clear();
            }
            _isDirty = false;
        }
    }
}
