using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    internal class UIVisual : UIElement
    {
        internal Vector2 Offset { get; set; } = Vector2.Zero;
        // Maybe layer if needed
        internal Vector2 ScreenPosition => GetScreenPosition();

        private Vector2 GetScreenPosition()
        {
            return UIObject.Transform.Position + Offset;
        }
    }
}
