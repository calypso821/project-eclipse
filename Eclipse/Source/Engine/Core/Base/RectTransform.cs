using Microsoft.Xna.Framework;

namespace Eclipse.Engine.Core
{
    internal class RectTransform
    {
        // Owenr
        public UIObject UIObject { get; }

        // Position in screen space
        internal Vector2 Position { get; set; }

        // Width and Height of the rect
        internal Vector2 Size { get; set; }

        // Pivot point (0,0 is top-left, 1,1 is bottom-right)
        internal Vector2 Pivot { get; set; } = new Vector2(0.5f, 0.5f);

        // Scale (if needed)
        internal Vector2 Scale { get; set; } = Vector2.One;

        // Rotation (if needed) - around pivot
        internal float Rotation { get; set; }

        internal RectTransform(UIObject uiObject)
        {
            UIObject = uiObject;
        }


        // TODO: Hierarhical updates like Transform
        private Vector2 _localPosition;  // Relative to parent
        private Vector2 _screenPosition; // Cached final position

        internal Vector2 ScreenPosition => _screenPosition;

        internal Vector2 LocalPosition
        {
            get => _localPosition;
            set
            {
                _localPosition = value;
                UpdateScreenPosition();
            }
        }

        private void UpdateScreenPosition()
        {
            _screenPosition = UIObject != null
                ? UIObject.Transform.ScreenPosition + _localPosition
                : _localPosition;

            // Update all children
            // Similar to Transform system
        }
    }
}
