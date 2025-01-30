using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Eclipse.Components.UI;
using Eclipse.Engine.Scenes;

namespace Eclipse.Engine.Core
{
    [Flags]
    internal enum UIDirtyFlag
    {
        None = 0,
        Transform = 1 << 1,    // Position, scale, anchors changed
        Layout = 1 << 2,       // Layout properties changed (padding, margins)
        Style = 1 << 3,        // Visual style changes (color, opacity)
        Content = 1 << 4,      // Content updates (text, image)
        Render = 1 << 5,       // Visual properties that need re-render
        Animation = 1 << 6,    // UI animations
        State = 1 << 7,        // UI state changes (hover, pressed, etc.)
        Destroy = 1 << 8,
        All = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7)
    }

    internal class UIObject
    {

        internal bool Active { get; set; } = true;
        internal UIObject Parent { get; private set; }
        internal List<UIObject> Children { get; } = new();

        private readonly List<UIElement> _components = new();
        internal IReadOnlyList<UIElement> Components => _components;
        internal RectTransform Transform { get; }

        internal UIObject()
        {
            Transform = new RectTransform(this);
        }

        internal T AddComponent<T>(T component) where T : UIElement
        {
            component.OnInitialize(this);
            _components.Add(component);
            return component;
        }

        internal T GetComponent<T>()
        {
            return _components.OfType<T>().FirstOrDefault();
        }

        internal List<T> GetComponents<T>()
        {
            return _components.OfType<T>().ToList();
        }

        internal void AddChild(UIObject child)
        {

            Children.Add(child);
            child.Parent = this;

            // Parent object already in scene
            //if (_sceneGraph != null)
            //{
            //    // Update child depth, scene refrence...
            //    child.OnAddedToScene(_sceneGraph);
            //}
        }
        internal virtual void Destroy()
        {
            // Call Destroy() on all children
            foreach (var child in Children)
            {
                child.Destroy();
            }

            // Clear all compoennt resources (subscriptions...)
            foreach (var component in _components)
            {
                component.OnDestroy();
            }
        }
    }
}
