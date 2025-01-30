using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Engine.Systems.Render;

namespace Eclipse.Engine.Utils.Load.Assets
{
    internal class FontAsset : Asset
    {
        internal SpriteFont Font { get; private set; }
        private float _defaultSize;

        internal FontAsset(SpriteFont font)
        {
            Font = font;
            //_defaultSize = defaultSize;
        }

        // Get dimensions of text with current font
        //internal Vector2 MeasureString(string text)
        //{
        //    return Font.MeasureString(text);
        //}

        //// Get height of the font
        //internal float GetLineSpacing()
        //{
        //    return Font.LineSpacing;
        //}

        //// Convert to world units (similar to your PPU conversion in SpriteAsset)
        //internal Vector2 MeasureStringInUnits(string text)
        //{
        //    return PPU.ToUnits(MeasureString(text));
        //}

        //// Get character spacing
        //internal float GetSpacing()
        //{
        //    return Font.Spacing;
        //}

        //// Get default character for missing glyphs
        //internal char GetDefaultCharacter()
        //{
        //    return Font.DefaultCharacter ?? ' ';
        //}
    }
}
