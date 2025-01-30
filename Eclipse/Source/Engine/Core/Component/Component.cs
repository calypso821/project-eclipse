
using System;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Managers;
using Eclipse.Source.Engine.Core.Entity;

namespace Eclipse.Engine.Core
{
    internal abstract class Component
    {
        // TODO: Component --> GameComponent, UIComponent
        public Entity Owner { get; private set; }
        public GameObject GameObject { get; private set; }

        internal virtual DirtyFlag DirtyFlag => DirtyFlag.None;
        internal virtual bool IsUnique => true;
        internal bool IsRegistered { get; set; } = false;

        internal readonly int InstanceId;

        internal bool IsEnabled => _enabled;
        private bool _enabled = true;

        internal Component()
        {
            InstanceId = IDManager.GetId();
        }

        // Internal engine lifecycle methods
        internal virtual void OnInitialize(GameObject gameObject) // When added to GameObject
        {
            if (GameObject != null)
                throw new InvalidOperationException("GameObject can only be initialized once");

            GameObject = gameObject;

            if (DirtyFlag != DirtyFlag.None)
            {
                SetDirtyFlag();
            }
        }
        internal void Enable()
        {
            if (!_enabled)
            {
                _enabled = true;
                OnEnable();
            }
        }

        internal void Disable()
        {
            if (_enabled)
            {
                _enabled = false;
                OnDisable();
            }
        }
        internal virtual void OnStart() { } // When added to Scene
        internal virtual void OnEnable() { } // When components is enabled
        internal virtual void OnDisable() { } // When components is disabled
        internal virtual void Update(GameTime gameTime) { } // EveryFrame
        internal virtual void OnReset() { } // Reset component (pooling)
        internal virtual void OnDestroy() // When destroyed
        {
            IDManager.ReleaseId(InstanceId);
        } 

        // TODO: 
        //internal void ReleaseId() {} // Called when removing component

        // Internal engine state management

        // Set DirtyFlag + add to system (runtime use)
        internal virtual void Register()
        {
            if (IsUnique)
                GameObject.Register(DirtyFlag);
            else
                GameObject.Register(this);
        }

        internal virtual void Unregister()
        {
            if (IsUnique)
                GameObject.Unregister(DirtyFlag);
            else
                GameObject.Unregister(this);
        }

        // Set DirtyFlag (initialization pahse)
        internal virtual void SetDirtyFlag()
        {
            if (IsUnique)
                GameObject.SetDirtyFlag(DirtyFlag);
            else
                GameObject.SetDirtyFlag(this);
        }


        internal virtual void ClearDirty()
        {
            if (IsUnique)
                GameObject.ClearDirtyFlag(DirtyFlag);
            else
                GameObject.ClearDirtyFlag(this);
        }

        internal virtual bool IsDirty()
        {
            return IsUnique ?
                GameObject.HasDirtyFlag(DirtyFlag) :
                GameObject.HasDirtyFlag(this);
        }
    }
}