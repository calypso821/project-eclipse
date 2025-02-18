using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Engine.Systems.Physics;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Debug
{
    internal class DebugRenderer : IDrawableSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly PhysicsSystem _physicsSystem;
        private readonly CameraManager _cameraManager;

        private readonly Texture2D _pixel;

        internal DebugRenderer(
            SpriteBatch spriteBatch,
            PhysicsSystem physicsSystem)
        {
            _spriteBatch = spriteBatch;
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _physicsSystem = physicsSystem;
            _cameraManager = CameraManager.Instance;
        }

        public void Draw()
        {
            DrawColliders();
            DrawVelocities();
            DrawCollisionNormals();
        }

        public void DrawColliders()
        {
            _spriteBatch.Begin(transformMatrix: _cameraManager.ScreenViewMatrix);

            foreach (var renderObject in _physicsSystem.Colliders)
            {
                if (!renderObject.IsEnabled || !renderObject.GameObject.IsActive)
                    continue;

                renderObject.GameObject.Transform.GetWorldTransform(
                    out Vector2 position,
                    out float rotation,
                    out Vector2 scale
                );

                // + Transfrom size with scale
                var pos = position + renderObject.Offset;

                pos = PPU.ToPixels(pos);

                // AABB intersection check
                if (renderObject is BoxCollider2D box)
                {
                    var size = PPU.ToPixels(box.HalfSize * scale);
                    var rect = new Rectangle(
                      (int)(pos.X - size.X),  // top-left X 
                      (int)(pos.Y - size.Y),  // top-left Y
                      (int)(size.X * 2),      // full width
                      (int)(size.Y * 2)       // full height
                  );

                    // Draw outline using primitive line
                    var thickness = 2;
                    _spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), Color.Red); // Top
                    _spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), Color.Red); // Bottom  
                    _spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), Color.Red); // Left
                    _spriteBatch.Draw(_pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), Color.Red); // Right
                }
            }
            _spriteBatch.End();
        }
        public void DrawVelocities()
        {
            _spriteBatch.Begin(transformMatrix: _cameraManager.ScreenViewMatrix);

            const float VELOCITY_SCALE = 0.2f;

            foreach (var rigidbody in _physicsSystem.RigidBodies)
            {
                if (!rigidbody.IsEnabled || !rigidbody.GameObject.IsActive)
                    continue;

                // Get position and velocity
                rigidbody.GameObject.Transform.GetWorldTransform(
                    out Vector2 position,
                    out float rotation,
                    out Vector2 scale
                );

                var pos = PPU.ToPixels(position);
                var velocity = PPU.ToPixels(rigidbody.Velocity) * VELOCITY_SCALE;

                if (velocity.Length() < 0.1f) // Skip if barely moving
                    continue;

                // Draw main velocity line
                var endPoint = pos + velocity;
                DrawLine(pos, endPoint, Color.Blue);

                // Draw arrow head
                float arrowSize = 10f;
                Vector2 direction = Vector2.Normalize(velocity);
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

                Vector2 arrowLeft = endPoint - direction * arrowSize + perpendicular * arrowSize * 0.5f;
                Vector2 arrowRight = endPoint - direction * arrowSize - perpendicular * arrowSize * 0.5f;

                DrawLine(endPoint, arrowLeft, Color.Blue);
                DrawLine(endPoint, arrowRight, Color.Blue);
            }

            _spriteBatch.End();
        }

        public void DrawCollisionNormals()
        {
            _spriteBatch.Begin(transformMatrix: _cameraManager.ScreenViewMatrix);

            const float NORMAL_SCALE = 20f; // Adjust scale to make normals visible

            foreach (var collision in _physicsSystem.Collisions)  // Assuming you store active collisions
            {
                var point = PPU.ToPixels(collision.Point);
                var normalEnd = point + PPU.ToPixels(collision.Normal) * NORMAL_SCALE;

                // Draw main normal line
                DrawLine(point, normalEnd, Color.Green);

                // Draw arrow head
                float arrowSize = 10f;
                Vector2 direction = Vector2.Normalize(collision.Normal);
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

                Vector2 arrowLeft = normalEnd - direction * arrowSize + perpendicular * arrowSize * 0.5f;
                Vector2 arrowRight = normalEnd - direction * arrowSize - perpendicular * arrowSize * 0.5f;

                DrawLine(normalEnd, arrowLeft, Color.Green);
                DrawLine(normalEnd, arrowRight, Color.Green);
            }

            _spriteBatch.End();
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            _spriteBatch.Draw(_pixel,
                new Rectangle(           // rectangle defines line position and size
                    (int)start.X,
                    (int)start.Y,
                    (int)length,        // length of the line
                    2),                 // thickness of the line
                null,
                color,
                angle,                  // angle of the line
                new Vector2(0, 0.5f),   // rotate around the start of the line
                SpriteEffects.None,
                0);
        }
    }
}
