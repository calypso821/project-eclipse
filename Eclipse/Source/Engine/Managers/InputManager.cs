using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Eclipse.Engine.Core;
using Eclipse.Engine.Systems.Input;
using Eclipse.Engine.Systems.Render;

namespace Eclipse.Engine.Managers
{
    public class InputManager : Singleton<InputManager>
    {
        private InputSystem _inputSystem;
        internal Point MousePosition => _inputSystem.GetMouseState().Position;
        internal Vector2 WorldMousePosition => GetWorldMousePosition();

        public InputManager()
        {
        }
        internal void Initialize(InputSystem inputSystem)
        {
            _inputSystem = inputSystem;
        }

        // ======== Keyboard ========
        // Direct key state (every frame)
        internal bool IsKeyDown(Keys key) => _inputSystem.GetKeyboardState().IsKeyDown(key);
        internal bool IsKeyUp(Keys key) => _inputSystem.GetKeyboardState().IsKeyUp(key);

        // Single frame detection (first/last frame only)
        internal bool GetKeyDown(Keys key) =>
            _inputSystem.GetKeyboardState().IsKeyDown(key) &&
            _inputSystem.GetPreviousKeyboardState().IsKeyUp(key);

        internal bool GetKeyUp(Keys key) =>
            _inputSystem.GetKeyboardState().IsKeyUp(key) &&
            _inputSystem.GetPreviousKeyboardState().IsKeyDown(key);

        // ======== Mouse ========
        // Held state (continuous pressing)
        internal bool IsLeftMouseHeld() =>
            _inputSystem.GetMouseState().LeftButton == ButtonState.Pressed;

        internal bool IsRightMouseHeld() =>
            _inputSystem.GetMouseState().RightButton == ButtonState.Pressed;

        // Single click detection (first frame only)
        internal bool GetLeftMouseDown() =>
            _inputSystem.GetMouseState().LeftButton == ButtonState.Pressed &&
            _inputSystem.GetPreviousMouseState().LeftButton == ButtonState.Released;

        internal bool GetRightMouseDown() =>
            _inputSystem.GetMouseState().RightButton == ButtonState.Pressed &&
            _inputSystem.GetPreviousMouseState().RightButton == ButtonState.Released;

        // Release detection (first frame of release)
        internal bool GetLeftMouseUp() =>
            _inputSystem.GetMouseState().LeftButton == ButtonState.Released &&
            _inputSystem.GetPreviousMouseState().LeftButton == ButtonState.Pressed;

        internal bool GetRightMouseUp() =>
            _inputSystem.GetMouseState().RightButton == ButtonState.Released &&
            _inputSystem.GetPreviousMouseState().RightButton == ButtonState.Pressed;


        internal Vector2 GetMovementDirection()
        {
            Vector2 direction = Vector2.Zero;

            if (IsKeyDown(Keys.W)) direction.Y -= 1;
            if (IsKeyDown(Keys.S)) direction.Y += 1;
            if (IsKeyDown(Keys.A)) direction.X -= 1;
            if (IsKeyDown(Keys.D)) direction.X += 1;

            if (direction != Vector2.Zero)
                // Normalize --> Unit vector (magnitude of 1)
                direction.Normalize();

            return direction;
        }

        private Vector2 GetWorldMousePosition()
        {
            return Vector2.Transform(
                        PPU.ToUnits(MousePosition.ToVector2()),
                        Matrix.Invert(CameraManager.Instance.ViewMatrix)
            );
        }
    }
}
