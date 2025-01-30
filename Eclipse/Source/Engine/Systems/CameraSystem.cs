using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Cameras;

namespace Eclipse.Engine.Systems
{
    internal class CameraSystem : ISystem
    {
        private readonly Camera _camera;
        internal Camera Camera => _camera;

        private GameObject _target;
        internal GameObject Target
        {
            get => _target;
            set => _target = value;
        }

        internal CameraSystem(Camera camera)
        {
            _camera = camera;
        }

        public void Update(GameTime gameTime)
        {
            if (_target != null)
            {
                Vector2 targetPosition = _target.Transform.Position - _camera.Center;
                Vector2 rotatedTarget = Vector2.Transform(
                    targetPosition, 
                    Matrix.CreateRotationZ(-_camera.Rotation)
                );
                Vector2 rotatedCameraPos = Vector2.Transform(
                    _camera.Position, 
                    Matrix.CreateRotationZ(-_camera.Rotation)
                );

                _camera.Position = Vector2.Lerp(rotatedCameraPos, rotatedTarget, 0.1f);
                // Camera logic is separate from rendering
                // Makes it easier to:
                // 1. Add smooth following

                // Top left conrner 
                // Move to center of player
                // Camera == whole screen (prefferedWidth, Height)
                // Center = width/2, height/2
                // Target Position = Player.Posotion -  camer.Center
                // Center / Zoom 
                // + apply rotation

                //Console.WriteLine(
                //    "Camera: " + _camera.Position.X +
                //    "Player: " + _target.Transform.Position.X);
                //Console.WriteLine(_camera.Position);

                // 2. Add screen shake
                // 3. Add zoom effects
                // 4. Add boundaries
                // etc.
            }
        }
    }
}
