using System;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    internal class UITextInput : UIWidget
    {
        internal event Action<string> OnTextChanged;
        internal event Action<string> OnSubmit;

        internal string Text { get; private set; } = string.Empty;
        internal string Placeholder { get; set; } = string.Empty;
        internal bool IsFocused { get; private set; }
        internal Rectangle Bounds => _bounds;

        private UIImage _background;
        private UIText _text;
        private Rectangle _bounds;

        internal UITextInput(string text = "")
        {
            Text = text;
        }

        internal override void OnInitialize(UIObject uiObject)
        {
            base.OnInitialize(uiObject);
            // Required component
            _text = UIObject.GetComponent<UIText>() ??
                throw new Exception("UITextInput requires a UIText component");
 
            // Optional components
            _background = UIObject.GetComponent<UIImage>();

            if (string.IsNullOrEmpty(Text))
            {
                SetText(Placeholder);
                _text.Color = Color.Gray;
            }
            else
            {
                SetText(Text);
            }


            UpdateBounds();
        }

        internal void SetText(string newText)
        {
            Text = newText;
            _text.Text = newText;
            _text.Color = Color.Black;
            OnTextChanged?.Invoke(Text);
        }

        internal void SetFocus(bool focused)
        {
            IsFocused = focused;

            if (focused)
            {
                SetText(string.Empty);
            }
        }

        internal void HandleTextInput(char character)
        {
            if (!IsFocused) return;

            Text += character;
            _text.Text = Text;
            OnTextChanged?.Invoke(Text);
        }

        internal void HandleSpecialInput(Keys key)
        {
            if (!IsFocused) return;

            switch (key)
            {
                case Keys.Back:
                    if (Text.Length > 0)
                    {
                        Text = Text[..^1]; // Remove last character
                        _text.Text = Text;
                        OnTextChanged?.Invoke(Text);
                    }
                    break;

                case Keys.Enter:
                    if (Text.Length > 0)
                        OnSubmit?.Invoke(Text);
                    SetFocus(false);
                    break;
            }
        }

        private void UpdateBounds()
        {
            var pos = UIObject.Transform.Position;
            var size = UIObject.Transform.Size;
            var scale = UIObject.Transform.Scale;
            _bounds = new Rectangle(
                (int)(pos.X),
                (int)(pos.Y),
                (int)(size.X * scale.X),
                (int)(size.Y * scale.Y)
            );
        }
    }
}
