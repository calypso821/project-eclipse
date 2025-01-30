using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Bson;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Engine
{
    public enum Element
    {
        Light,  // White
        Dark,   // Black
        Neutral // Zone/level color (dark)
    }

    internal class ElementState : Component
    {
        private List<Sprite> _sprites;
        private List<Collider2D> _colliders;

        private Element _state;
        private bool _isStatic;

        internal Element State => _state;

        internal ElementState(Element element = Element.Neutral, bool isStatic = true)
        {
            _state = element;
            _isStatic = isStatic;
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            // Get the sprite component on initialization
            _sprites = GameObject.GetComponents<Sprite>() ??
                throw new ArgumentException("ElementState requires Sprite component");

            // Optional
            _colliders = GameObject.GetComponents<Collider2D>();

            // Initial color and collison mask setup
            SetState(_state);
        }

        internal void SetState(Element state)
        {
            _state = state;
            SetColor(_state);
            SetCollisionMask(_state);
        }

        internal void ToggleState()
        {
            if (_isStatic || _state == Element.Neutral) return;

            // Update element state
            _state = _state == Element.Light ? Element.Dark : Element.Light;

            // Update color and collision mask
            SetState(_state);

            // Toggle all children recursively
            foreach (var child in GameObject.Children)
            {
                var childColorModifier = child.GetComponent<ElementState>();
                childColorModifier?.ToggleState();
            }
        }

        internal void SetColor(Element element)
        {
            var color = element switch
            {
                Element.Light => Color.White,
                Element.Dark => Color.Black,
                // TODO: Zone color
                Element.Neutral => new Color(3, 41, 28, 255),
                //Element.Neutral => new Color(3, 59, 39, 255),
                _ => throw new ArgumentException($"Unsupported element: {element}")
            };

            foreach (var sprite in _sprites)
            {
                sprite.Color = color;
            }
        }

        internal void SetCollisionMask(Element element)
        {
            foreach (var collider in _colliders)
            {
                var mask = element switch
                {
                    Element.Light => ElementMask.Light,
                    Element.Dark => ElementMask.Dark,
                    Element.Neutral => ElementMask.Light | ElementMask.Dark,
                    _ => throw new ArgumentException($"Unsupported element: {element}")
                };
                collider.Mask = mask;
            }
        }
    }
}
