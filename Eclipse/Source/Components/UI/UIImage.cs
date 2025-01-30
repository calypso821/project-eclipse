using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Eclipse.Engine.Utils.Load.Assets;

namespace Eclipse.Components.UI
{
    internal class UIImage : UIVisual
    {
        internal Texture2D Texture { get; set; }
        internal Rectangle SourceRectangle { get; set; }
        internal Color Color { get; set; } = Color.White;
        internal Vector2 Origin { get; set; } = Vector2.Zero;
        internal Vector2 Size => new Vector2(SourceRectangle.Width, SourceRectangle.Height);

        internal UIImage(SpriteAsset spriteAsset)
        {
            Texture = spriteAsset.Texture;
            // TODO: Support for animated UIImages (add Frames)
            SourceRectangle = spriteAsset.Frames[0].SourceRectangle;
            //Origin = spriteAsset.Frames[0].Origin;
            Origin = new Vector2(0, 0); // UIElemnts 0,0 by default
        }
    }

}