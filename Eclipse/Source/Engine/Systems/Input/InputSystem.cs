using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Systems.Input
{
    internal class InputSystem : ISystem
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        internal KeyboardState GetKeyboardState() => _currentKeyboardState;
        internal KeyboardState GetPreviousKeyboardState() => _previousKeyboardState;
        internal MouseState GetMouseState() => _currentMouseState;
        internal MouseState GetPreviousMouseState() => _previousMouseState;

        public void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
        }
    }
}