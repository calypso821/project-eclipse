using System;

using Microsoft.Xna.Framework;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Physics.Collision
{
    internal static class CollisionDetection2D
    {
        internal static bool AABBIntersects(
            Vector2 centerA, Vector2 halfSizeA,
            Vector2 centerB, Vector2 halfSizeB)
        {
            return Math.Abs(centerA.X - centerB.X) < halfSizeA.X + halfSizeB.X &&
                   Math.Abs(centerA.Y - centerB.Y) < halfSizeA.Y + halfSizeB.Y;
        }

        internal static Collision2D GetCollisionInfo(
            Vector2 centerA, Vector2 halfSizeA,
            Vector2 centerB, Vector2 halfSizeB,
            Collider2D colliderA, Collider2D colliderB)
        {
            // Calculate overlap on each axis
            Vector2 delta = centerB - centerA;
            float overlapX = halfSizeA.X + halfSizeB.X - Math.Abs(delta.X);
            float overlapY = halfSizeA.Y + halfSizeB.Y - Math.Abs(delta.Y);

            // Normal should point from B to A
            Vector2 normal;
            float depth;
            if (overlapX < overlapY)
            {
                // Use X axis - normal points opposite to delta.X
                normal = new Vector2(delta.X > 0 ? -1 : 1, 0);
                depth = overlapX;
            }
            else
            {
                // Use Y axis - normal points opposite to delta.Y
                normal = new Vector2(0, delta.Y > 0 ? -1 : 1);
                depth = overlapY;
            }

            // Rest remains the same
            Vector2 minA = centerA - halfSizeA;
            Vector2 maxA = centerA + halfSizeA;
            Vector2 minB = centerB - halfSizeB;
            Vector2 maxB = centerB + halfSizeB;
            Vector2 point = new Vector2(
                (Math.Max(minA.X, minB.X) + Math.Min(maxA.X, maxB.X)) * 0.5f,
                (Math.Max(minA.Y, minB.Y) + Math.Min(maxA.Y, maxB.Y)) * 0.5f
            );
            return new Collision2D(colliderA, colliderB, normal, point, depth);
        }

    }
}
