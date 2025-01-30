using System.Collections.Generic;
using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Components.Engine;
using Eclipse.Engine.Systems.Render;

namespace Eclipse.Engine.Utils.Load.Assets
{

    internal class SpriteAsset : Asset
    {

        private readonly List<Frame> _frames = new();
        internal IReadOnlyList<Frame> Frames => _frames;

        internal Texture2D Texture { get; private set; }

        internal SpriteAsset(Texture2D texture, bool createDefaultFrame = false)
        {
            Texture = texture;

            // Create default frame (sttic sprites - 1 frame only)
            if (createDefaultFrame)
            {
                AddFrame(
                    index: 0,
                    sourceRectangle: new Rectangle(0, 0, texture.Width, texture.Height),
                    origin: new Vector2(Texture.Width / 2, Texture.Height / 2),
                    isRotated: false
                );
            }
        }

        internal void AddFrame(int index,
            Rectangle sourceRectangle, Vector2 origin, bool isRotated = false)
        {
            // If index is bigger than current size of list
            // fill it with nulls (replaced with sprites later)
            while (_frames.Count <= index)
            {
                _frames.Add(default);
            }
            _frames[index] = new Frame(sourceRectangle, origin, isRotated);
        }

        internal void SetCustomOrigin(Vector2 origin)
        {
            foreach (var frame in _frames)
            {
                frame.SetCustomOrigin(origin);
            }
        }

        internal Vector2 GetSize()
        {
            Vector2 max_size = Vector2.Zero;
            foreach (var frame in _frames)
            {
                var size = frame.IsRotated ?
                    new Vector2(frame.SourceRectangle.Height, frame.SourceRectangle.Width) :
                    new Vector2(frame.SourceRectangle.Width, frame.SourceRectangle.Height);

                max_size = new Vector2(
                    Math.Max(max_size.X, size.X),
                    Math.Max(max_size.Y, size.Y)
                );
            }
            return max_size;
        }
        internal Vector2 GetHalfSizeInUnits()
        {
            return PPU.ToUnits(GetSize() / 2.0f);
        }
    }
   
}
