using System;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;


namespace Eclipse.Components.UI
{
    internal class UIButton : UIWidget
    {
        internal event Action OnClick;
        internal bool IsPressed { get; set; }
        internal bool IsHovered { get; set; }
        internal Rectangle Bounds => _bounds;

        private UIImage _background;
        private UIText _text;
        private Rectangle _bounds;

        internal override void OnInitialize(UIObject uiObject)
        {
            base.OnInitialize(uiObject);
            // Optinal components
            _background = UIObject.GetComponent<UIImage>();
            _text = UIObject.GetComponent<UIText>();
            UpdateBounds();
        }
        internal void InvokeClick()  // Method to invoke the event
        {
            OnClick?.Invoke();
        }

        private void UpdateBounds()
        {
            // Create button bounds (parent object)
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
