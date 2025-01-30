using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Components.UI;
using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    internal class UISlider : UIWidget
    {
        internal event Action<float> OnValueChanged;

        internal float Value { get; private set; }
        internal float MinValue { get; set; } = 0f;
        internal float MaxValue { get; set; } = 1f;
        internal bool IsDragging { get; private set; }
        internal Rectangle Bounds => _bounds;

        private UIImage _background;
        private UIImage _handle;
        private Rectangle _bounds;
        private Rectangle _handleBounds;

        internal override void OnInitialize(UIObject uiObject)
        {
            base.OnInitialize(uiObject);
            // Optional components
            _background = UIObject.GetComponent<UIImage>();
            _handle = UIObject.GetComponents<UIImage>()?.Skip(1).FirstOrDefault(); // Second image if exists
            UpdateBounds();
        }

        internal void SetValue(float newValue)
        {
            Value = MathHelper.Clamp(newValue, MinValue, MaxValue);
            UpdateHandlePosition();
            OnValueChanged?.Invoke(Value);
        }

        internal void StartDrag()
        {
            IsDragging = true;
        }

        internal void EndDrag()
        {
            IsDragging = false;
        }

        internal void UpdateDrag(Point mousePosition)
        {
            if (!IsDragging) return;

            // Calculate value based on mouse position relative to slider bounds
            float percentage = MathHelper.Clamp(
                (mousePosition.X - _bounds.X) / (float)_bounds.Width,
                0f,
                1f
            );

            float newValue = MinValue + (MaxValue - MinValue) * percentage;
            SetValue(newValue);
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

            UpdateHandlePosition();
        }

        private void UpdateHandlePosition()
        {
            if (_handle == null) return;

            float percentage = (Value - MinValue) / (MaxValue - MinValue);
            int handleX = (int)(_bounds.X + (_bounds.Width * percentage) - (_handleBounds.Width / 2));

            _handleBounds = new Rectangle(
                handleX,
                _bounds.Y,
                (int)(_handle.Size.X * UIObject.Transform.Scale.X),
                (int)(_handle.Size.Y * UIObject.Transform.Scale.Y)
            );

            _handle.Offset = new Vector2(handleX, _bounds.Y);
        }
    }
}
