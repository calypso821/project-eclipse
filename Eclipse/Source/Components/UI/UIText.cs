using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Eclipse.Engine.Utils.Load.Assets;
using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    internal enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }
    internal enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom
    }
    internal struct TextAlignment
    {
        internal HorizontalAlignment Horizontal;
        internal VerticalAlignment Vertical;

        internal TextAlignment(HorizontalAlignment horizontal = HorizontalAlignment.Left,
                             VerticalAlignment vertical = VerticalAlignment.Top)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        // Helpful static presets
        public static TextAlignment TopLeft => new(HorizontalAlignment.Left, VerticalAlignment.Top);
        public static TextAlignment Center => new(HorizontalAlignment.Center, VerticalAlignment.Middle);
        public static TextAlignment BottomRight => new(HorizontalAlignment.Right, VerticalAlignment.Bottom);
    }

    internal class UIText : UIVisual
    {

        private string _text = string.Empty;
        internal string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateAlignmentOffset();
            }
        }
        internal SpriteFont Font { get; set; }
        internal Color Color { get; set; } = Color.Black;
        internal Vector2 Origin { get; set; } = Vector2.Zero;
        internal float Size { get; set; } = 1.0f;

        private TextAlignment _alignment = TextAlignment.TopLeft;

        internal TextAlignment Alignment
        {
            get => _alignment;
            set
            {
                _alignment = value;
                UpdateAlignmentOffset();
            }
        }

        internal UIText(FontAsset fontAsset)
        {
            Font = fontAsset.Font;
            // By default 0,0 (top, left)
            Origin = new Vector2(0, fontAsset.Font.LineSpacing / 4.5f);
        }

        internal override void OnInitialize(UIObject uiObject)
        {
            base.OnInitialize(uiObject);
            UpdateAlignmentOffset();
        }

        internal void UpdateAlignmentOffset()
        {
            if (UIObject == null) return;

            var rawTextSize = Font.MeasureString(_text) * Size;
            var adjustedTextSize = new Vector2(
                rawTextSize.X,
                rawTextSize.Y - Origin.Y * 2.5f // Subtract the same padding we use in Origin
            );

            var size = UIObject.Transform.Size * UIObject.Transform.Scale;
            float xOffset = _alignment.Horizontal switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => size.X / 2f - adjustedTextSize.X / 2f,
                HorizontalAlignment.Right => size.X - adjustedTextSize.X,
                _ => 0
            };

            float yOffset = _alignment.Vertical switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Middle => size.Y / 2f - adjustedTextSize.Y / 2f,
                VerticalAlignment.Bottom => size.Y - adjustedTextSize.Y,
                _ => 0
            };


            Offset = new Vector2(xOffset, yOffset);
        }
    }
}
