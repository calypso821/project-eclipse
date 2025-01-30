using Microsoft.Xna.Framework;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Physics.Collision
{
    internal struct Collision2D
    {
        internal Collider2D ColliderA { get; }
        internal Collider2D ColliderB { get; }
        internal Vector2 Normal { get; }
        internal Vector2 Point { get; }
        internal float Depth { get; }

        internal Collision2D(
            Collider2D colliderA,
            Collider2D colliderB,
            Vector2 normal,
            Vector2 point,
            float depth)
        {
            ColliderA = colliderA;
            ColliderB = colliderB;
            Normal = normal;
            Point = point;
            Depth = depth;
        }
    }
}
