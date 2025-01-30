using System.Collections.Generic;

using Eclipse.Engine.Managers;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Scenes
{
    internal class SceneGraph 
    {
        private GameObject _root;

        private readonly SystemManager _systemManager;

        internal SceneGraph()
        {
            _systemManager = SystemManager.Instance;

            var root = new GameObject("Root");
            root.OnAddedToScene(this);
            _root = root;
        }

        public void AddNode(GameObject gameObjct)
        {
            _root.AddChild(gameObjct);
        }

        internal void Register(GameObject gameObject)
        {
            foreach ((var flag, var system) in _systemManager.ComponentSystems)
            {
                if (gameObject.HasDirtyFlag(flag))
                {
                    system.Register(gameObject);
                }
            }
        }

        internal void Unregister(GameObject gameObject)
        {
            foreach ((var flag, var system) in _systemManager.ComponentSystems)
            {
                if (gameObject.HasDirtyFlag(flag))
                {
                    system.Unregister(gameObject);
                }
            }
        }
        internal void Clear()
        {
            // Clean resources
            // Call OnDestry() on all components (+ children recursevly)
            _root.Destroy();

            // Clear all componts from systems
            _systemManager.Clear();

            _root = null;
        }
    }
}