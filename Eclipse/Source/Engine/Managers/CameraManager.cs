using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Systems;
using Eclipse.Engine.Cameras;

namespace Eclipse.Engine.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        private CameraSystem _cameraSystem;
        internal Camera Camera => _cameraSystem.Camera;
        internal Matrix ScreenViewMatrix => Camera.GetScreenViewMatrix();
        internal Matrix ViewMatrix => Camera.GetViewMatrix();

        public CameraManager()
        {
        }
        internal void Initialize(CameraSystem cameraSystem)
        {
            _cameraSystem = cameraSystem;
        }
        internal void SetTarget(GameObject target)
        {
            _cameraSystem.Target = target;
        }
    }
}
