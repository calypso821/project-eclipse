using Microsoft.Xna.Framework;

namespace Eclipse.Components.Engine
{
    internal sealed class BoxCollider2D : Collider2D
    {
        internal float Width { get; }
        internal float Height { get; }
        internal Vector2 HalfSize { get; }

        internal BoxCollider2D(Vector2 halfSize, Vector2 offset = default)
            : base()
        {
            HalfSize = halfSize;
            Offset = offset; // Default = Vector(0, 0)
            Width = halfSize.X * 2;
            Height = halfSize.Y * 2;
        }

        internal BoxCollider2D(float width, float height, Vector2 offset = default)
            : base()
        {
            HalfSize = new Vector2(width / 2, height / 2);
            Offset = offset; // Default = Vector(0, 0)
            Width = width;
            Height = height;
        }
    }
}