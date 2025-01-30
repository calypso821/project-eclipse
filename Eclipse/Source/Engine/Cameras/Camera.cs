using Microsoft.Xna.Framework;

using Eclipse.Engine.Systems.Render;

namespace Eclipse.Engine.Cameras
{
    internal class Camera
    {
        private Vector2 _center;
        public Vector2 Center
        {
            get => _center / Zoom;
        }
        internal Vector2 Position { get; set; }
        internal float Rotation { get; set; } = 0f;
        internal float Zoom { get; set; } = 1.0f;

        internal Camera(Vector2 windowSize)
        {
            _center = new Vector2(
                PPU.ToUnits(windowSize.X / 2),
                PPU.ToUnits(windowSize.Y / 2)
            );
        }

        // Get camera view transform
        internal Matrix GetScreenViewMatrix()
        {
            // Inverse of translation matrix (Transform)
            return Matrix.CreateTranslation(new Vector3(PPU.ToPixels(-Position), 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom);
        }

        internal Matrix GetViewMatrix()
        {
            // Inverse of translation matrix (Transform)
            return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom);
        }
    }
}
