
using System;
using System.Collections.Generic;

using Eclipse.Engine.Physics.Collision;
using Eclipse.Engine.Scenes;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Core
{
    [Flags]
    internal enum DirtyFlag
    {
        None = 0,
        Transform = 1 << 1,    // Position/rotation changed
        Collider = 1 << 2,     // Collider properties changed
        RigidBody = 1 << 3,    // RigidBody state changed (velocity, kinematic)
        Render = 1 << 4,       // Visual properties changed
        Animator = 1 << 5,      // Animation update
        Controller = 1 << 6,
        Projectile = 1 << 7,
        Ability = 1 << 8,       // Cooldowns
        Audio = 1 << 9,       // AudioSoruce
        VFX = 1 << 10,       // AudioSoruce
        Destroy = 1 << 11,        // MarkForDestroy GameObject and its components

        // All flags except Destroy
        All = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) |
              (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8) |
              (1 << 9) | (1 << 10)
    }

    public class GameObject
    {
        public string Name { get; set; }

        // Hierarchy
        internal GameObject Parent { get; private set; }
        internal List<GameObject> _children = new List<GameObject>();
        public IReadOnlyList<GameObject> Children => _children;
        private int _depth;
        internal int Depth => _depth;

        // State
        private bool _active = true;
        internal bool IsActive => _active;

        private bool _isStatic = true;
        internal bool IsStatic => _isStatic;

        // Components
        private readonly Dictionary<Type, Component> _components = new();
        internal IReadOnlyDictionary<Type, Component> Components => _components;
        internal Transform Transform { get; private set; }
        internal event Action<Collision2D> OnCollision;

        // Scene
        private DirtyFlag _dirtyFlags = DirtyFlag.None;
        private Dictionary<DirtyFlag, HashSet<int>> _instanceFlags;
        private SceneGraph _sceneGraph = null;

        public GameObject(string name = "GameObject")
        {
            Name = name;
            Parent = null;
            Transform = new Transform(this);
        }

        // When object/component added to scene ("live" objects)
        internal void OnAddedToScene(SceneGraph sceneGraph)
        {
            _sceneGraph = sceneGraph;
            UpdateDepth();

            if (IsDirty)
            {
                sceneGraph.Register(this);
            }

            // Call OnStart on all components
            Start();

            // Propagate to children
            foreach (var child in _children)
            {
                child.OnAddedToScene(sceneGraph);
            }
        }
        public void SetActive(bool state)
        {
            if (_active == state) return;

            _active = state;

            // Cascade to children
            foreach (var child in Children)
            {
                child.SetActive(state);
            }

            if (state)
                OnActivate();
            else
                OnDeactivate();
        }
        internal virtual void OnActivate() { }
        internal virtual void OnDeactivate() { }

        internal virtual void Start()
        {
            // Initilzie all components
            foreach (var component in _components.Values)
            {
                component.OnStart();
            }
        }

        internal virtual void Reset()
        {
            foreach (var component in _components.Values)
            {
                component.OnReset();
            }
        }

        internal void MarkForDestroy()
        {
            // Prevent duplication in _destroyList
            if (!HasDirtyFlag(DirtyFlag.Destroy))
            {
                SetActive(false);
                Register(DirtyFlag.Destroy);
            }
        }

        internal virtual void Destroy()
        {
            // Call Destroy() on all children
            foreach (var child in _children)
            {
                child.Destroy();
            }

            // Clear all compoennt resources (subscriptions...)
            foreach (var component in _components.Values)
            {
                component.OnDestroy();
            }
        }

        internal void UpdateDepth()
        {
            _depth = Parent == null ? 0 : Parent._depth + 1;

            foreach (var child in _children)
            {
                child.UpdateDepth();  // Recursive
            }
        }
        internal void Register(DirtyFlag flag)
        {
            SetDirtyFlag(flag);
            HandleSceneRegistration(true);
        }

        internal void Register(Component component)
        {
            SetDirtyFlag(component);
            HandleSceneRegistration(true);
        }

        internal void Unregister(DirtyFlag flag)
        {
            // Only handle scene unregistration if component was newly marked
            // Could be aready set -> pending for system removal (cleanup)
            if (SetDirtyFlag(flag))
            {
                HandleSceneRegistration(false);
            }
        }

        internal void Unregister(Component component)
        {
            // Only handle scene unregistration if component was newly marked
            // Could be aready set -> pending for system removal (cleanup)
            if (SetDirtyFlag(component))
            {
                HandleSceneRegistration(false);
            }
        }

        private void HandleSceneRegistration(bool register)
        {
            if (_sceneGraph != null)
            {
                if (register)
                    _sceneGraph.Register(this);
                else
                    _sceneGraph.Unregister(this);
            }
        }

        internal bool SetDirtyFlag(DirtyFlag flag)  // Just sets flag
        {
            // DirtyFlag already set
            if ((_dirtyFlags & flag) != 0) return false;
            _dirtyFlags |= flag;
            return true;
        }

        internal bool SetDirtyFlag(Component component)
        {
            var flag = component.DirtyFlag;
            // Create colcetion if does not exist yet
            _instanceFlags ??= new Dictionary<DirtyFlag, HashSet<int>>();

            // Check if flag is not yet present in _instanceFlags
            if (!_instanceFlags.TryGetValue(flag, out var instances))
            {
                instances = new HashSet<int>();
                _instanceFlags[flag] = instances;
                _dirtyFlags |= flag;
            }
            // Return true only if instance wasn't already marked
            return instances.Add(component.InstanceId);
        }

        internal bool HasDirtyFlag(DirtyFlag flag) => (_dirtyFlags & flag) != 0;
        internal bool HasDirtyFlag(Component component)
        {
            var flag = component.DirtyFlag;
            if ((_dirtyFlags & flag) == 0) return false;

            return _instanceFlags?.TryGetValue(flag, out var instances) == true
                    && instances.Contains(component.InstanceId);
        }
        internal void ClearDirtyFlag(DirtyFlag flag) => _dirtyFlags &= ~flag;

        internal void ClearDirtyFlag(Component component)
        {
            var flag = component.DirtyFlag;

            // Check if flag is present
            if (_instanceFlags?.TryGetValue(flag, out var instances) == true)
            {
                instances.Remove(component.InstanceId);

                // Ceck if thre is any component with its flag present
                if (instances.Count == 0)
                {
                    _instanceFlags.Remove(flag);
                    ClearDirtyFlag(flag);
                }
            }
        }

        internal void ClearDirtyFlags()
        {
            _dirtyFlags = DirtyFlag.None;
            _instanceFlags?.Clear();
        }

        internal bool IsDirty => _dirtyFlags != DirtyFlag.None;

        // Component methods
        internal virtual T AddComponent<T>(T component) where T : Component
        {
            // Check if interface is unique
            var type = typeof(T);

            // Check if same Component already exist on obejct
            if (_components.ContainsKey(type))
                throw new Exception($"Component {type} already exists on {Name}");

            // If component is unique, check if we already have a component of that type
            if (component.IsUnique && component.DirtyFlag != DirtyFlag.None)
            {
                foreach (var existing in _components.Values)
                {
                    if (existing.DirtyFlag == component.DirtyFlag)
                        throw new Exception($"GameObject already has a unique component for {component.DirtyFlag}");
                }
            }

            // Set GameObject reference
            component.OnInitialize(this);
            UpdateGameObjectState(component, true);
            _components[type] = component;

            if (_sceneGraph != null)
            {
                // During runtime (when in scene)
                component.OnStart();
                component.Register();
            }

            return component;
        }

        internal T GetComponent<T>()
        {
            var requestedType = typeof(T);

            // Check if type is Component or interface
            if (!requestedType.IsAssignableTo(typeof(Component)) &&
                !requestedType.IsInterface)
            {
                throw new ArgumentException($"Type {requestedType} must be either Component or interface");
            }

            // Direct component check
            if (_components.TryGetValue(requestedType, out var exactComponent))
            {
                return (T)(object)exactComponent;
            }

            // Interface check
            foreach (var component in _components.Values)
            {
                if (component.GetType().IsAssignableTo(requestedType))
                {
                    return (T)(object)component;
                }
            }

            return default;
        }
        internal List<T> GetComponents<T>()
        {
            var requestedType = typeof(T);

            // Check if type is Component or interface
            if (!requestedType.IsAssignableTo(typeof(Component)) &&
                !requestedType.IsInterface)
            {
                throw new ArgumentException($"Type {requestedType} must be either Component or interface");
            }

            var outComponents = new List<T>();

            // If it's a Component type (not interface) and it's unique, we should warn
            if (!requestedType.IsInterface &&
                _components.TryGetValue(requestedType, out var comp) &&
                (comp as Component).IsUnique)
            {
                throw new InvalidOperationException($"Cannot get multiple components of unique type {requestedType}");
            }

            foreach (var component in _components.Values)
            {
                if (component.GetType().IsAssignableTo(requestedType))
                {
                    outComponents.Add((T)(object)component);
                }
            }

            return outComponents;
        }

        internal bool HasComponent<T>()
        {
            var type = typeof(T);

            // Direct check for Component type
            if (_components.ContainsKey(type))
                return true;

            // Interface check
            if (type.IsInterface)
            {
                foreach (var component in _components.Values)
                {
                    if (component.GetType().IsAssignableTo(type))
                        return true;
                }
            }

            return false;
        }

        internal bool RemoveComponent<T>() where T : Component
        {
            var component = GetComponent<T>();

            if (component != null)
            {
                component.OnDestroy();
                component.Unregister();
                UpdateGameObjectState(component, false);

                return _components.Remove(component.GetType());
            }
            return false;
        }

        private void UpdateGameObjectState(Component component, bool isAdding)
        {
            // If RigidBody component is present object is NOT Static
            if (component is RigidBody2D)
                _isStatic = !isAdding;
        }

        // Child methods

        public virtual void AddChild(GameObject child)
        {
            // Handle if child already has a parent
            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            _children.Add(child);
            child.Parent = this;

            // Parent object already in scene
            if (_sceneGraph != null)
            {
                // Update child depth, scene refrence...
                child.OnAddedToScene(_sceneGraph);
            }
        }

        public void RemoveChild(GameObject child)
        {
            if (_children.Remove(child))
            {
                child.Parent = null;
            }
        }

        public bool HasChild(GameObject child)
        {
            return _children.Contains(child);
        }

        public GameObject FindChild(string name)
        {
            return _children.Find(child => child.Name == name);
        }
        // TODO set parent (+ updaet dirty Flags Tranfrom)

        public void SetParent(GameObject parent)
        {
            // Remove from old parent's children list
            Parent?.RemoveChild(this);

            Parent = parent;

            // Add to new parent's children lis
            parent?.AddChild(this);
        }

        internal void ClearChildren()
        {
            _children.Clear();
        }

        // Collision response method (notifyes all components)
        // Example: RigidBody (move), take damage...
        // Actor (Player, Enemy) might process collsion response dirrent
        internal virtual void HandleCollision(Collision2D collision)
        {
            OnCollision?.Invoke(collision);
        }
    }
}