using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Eclipse.Components.UI;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;

namespace Eclipse.Engine.Systems
{
    internal class UISystem : ComponentSystem
    {
        private List<UIWidget> _widgets = new List<UIWidget>();
        private List<UIButton> _buttons = new List<UIButton>();
        private List<UITextInput> _textInputs = new List<UITextInput>();
        private Queue _pendingActions = new Queue();

        internal void QueueAction(Action action)
        {
            _pendingActions.Enqueue(action);
        }

        public void Register(UIObject uiObject)
        {
            foreach (var uiElement in uiObject.Components)
            {
                if (uiElement is UIWidget uiWidget && !uiWidget.IsRegistered)
                {
                    _widgets.Add(uiWidget);  // Add the cast version
                    uiWidget.IsRegistered = true;

                    switch (uiElement)
                    {
                        case UIButton button:
                            _buttons.Add(button);
                            break;
                        case UITextInput textInput:
                            _textInputs.Add(textInput);
                            break;
                        //case UISlider slider:
                        //    _sliders.Add(slider);
                        //    break;
                    }
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            // Process all UI input first
            ProcessInputs();
            // Widget update
            UpdateComponents(gameTime);
            // Execute queued actions last
            ExecutePendingActions();
        }

        private void ProcessInputs()
        {
            ProcessButtons();
            ProcessTextInputs();
        }

        private void ProcessButtons()
        {
            var mousePos = InputManager.Instance.MousePosition;

            foreach (var button in _buttons)
            {
                if (!button.IsEnabled || !button.UIObject.Active) continue;

                // All the input logic moves here from UIButton
                if (button.Bounds.Contains(mousePos))
                {
                    button.IsHovered = true;
                    if (InputManager.Instance.GetLeftMouseDown())
                    {
                        button.IsPressed = true;
                    }
                    else if (InputManager.Instance.GetLeftMouseUp() && button.IsPressed)
                    {
                        QueueAction(button.InvokeClick);
                        button.IsPressed = false;
                    }
                }
                else
                {
                    button.IsHovered = false;
                }
            }
        }
        private void ProcessTextInputs()
        {
            var mousePos = InputManager.Instance.MousePosition;

            // Handle focus changes with mouse
            if (InputManager.Instance.GetLeftMouseDown())
            {
                foreach (var textInput in _textInputs)
                {
                    if (!textInput.IsEnabled || !textInput.UIObject.Active) continue;
                    textInput.SetFocus(textInput.Bounds.Contains(mousePos));
                }
            }

            // Process keyboard for focused textInput
            foreach (var textInput in _textInputs)
            {
                if (!textInput.IsEnabled || !textInput.UIObject.Active || !textInput.IsFocused) continue;

                // Handle special keys when they're first pressed
                if (InputManager.Instance.GetKeyDown(Keys.Back))
                {
                    textInput.HandleSpecialInput(Keys.Back); // Invoke - last char + OnTextChanged() 
                }
                else if (InputManager.Instance.GetKeyDown(Keys.Enter))
                {
                    textInput.HandleSpecialInput(Keys.Enter); // Invoke OnSubmit()
                }

                // Handle character input
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (InputManager.Instance.GetKeyDown(key))
                    {
                        bool shift = InputManager.Instance.IsKeyDown(Keys.LeftShift) ||
                                    InputManager.Instance.IsKeyDown(Keys.RightShift);

                        char? character = KeyToChar(key, shift);
                        if (character.HasValue)
                        {
                            textInput.HandleTextInput(character.Value); // Invoke OnTextChanged()
                        }
                    }
                }
            }
        }
        private char? KeyToChar(Keys key, bool shift)
        {
            // Letters
            if (key >= Keys.A && key <= Keys.Z)
            {
                char baseChar = (char)('a' + (key - Keys.A));
                return shift ? char.ToUpper(baseChar) : baseChar;
            }

            // Numbers
            if (!shift && key >= Keys.D0 && key <= Keys.D9)
            {
                return (char)('0' + (key - Keys.D0));
            }

            // Common keys
            return key switch
            {
                Keys.Space => ' ',
                Keys.OemPeriod => shift ? '>' : '.',
                Keys.OemComma => shift ? '<' : ',',
                Keys.OemMinus => shift ? '_' : '-',
                Keys.OemPlus => shift ? '+' : '=',
                _ => null
            };
        }

        private void UpdateComponents(GameTime gameTime)
        {
            foreach (var component in _widgets)
            {
                if (!component.IsEnabled || !component.UIObject.Active) continue;

                component.Update(gameTime);
            }
        }

        private void ExecutePendingActions()
        {
            while (_pendingActions.Count > 0)
            {
                var action = (Action)_pendingActions.Dequeue();
                action();
            }
        }

        public override void Clear()
        {
            _widgets.Clear();
            _buttons.Clear();
            _pendingActions.Clear();
        }
    }
}
