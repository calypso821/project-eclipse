using Microsoft.Xna.Framework;
using System;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;

namespace Eclipse.Components.Engine
{
    internal enum CollisionLayer
    {
        Default = 0,
        Player = 1,
        Enemy = 2,
        Trigger = 3,
        Hitbox = 4,
        Projectile = 5,
        Platform = 6
        // etc...
    }
    [Flags]
    internal enum ElementMask
    {
        None = 0,
        Light = 1 << 0, 
        Dark = 1 << 1
    }

    internal abstract class Collider2D : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Collider;
        internal sealed override bool IsUnique => false;
        public CollisionLayer Layer { get; set; } = CollisionLayer.Default;
        public ElementMask Mask { get; set; } = ElementMask.None;
        internal bool IsTrigger { get; set; }
        internal Vector2 Offset { get; set; }

        internal Collider2D()
            : base()
        {
            
        }
    }
}
