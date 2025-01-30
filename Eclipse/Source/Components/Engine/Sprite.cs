using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Utils.Load.Assets;
using Eclipse.Components.Animation;
using Eclipse.Engine.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Eclipse.Components.Engine
{
    internal class Frame
    {
        internal Rectangle SourceRectangle { get; }
        internal Vector2 Origin { get; private set; }
        internal bool IsRotated { get; }
        internal float Width => IsRotated ? SourceRectangle.Height : SourceRectangle.Width;
        internal float Height => IsRotated ? SourceRectangle.Width : SourceRectangle.Height;

        internal Frame(Rectangle sourceRectangle, Vector2 origin, bool isRotated = false)
        {
            SourceRectangle = isRotated ?
                new Rectangle(
                    sourceRectangle.X,
                    sourceRectangle.Y,
                    sourceRectangle.Height,  // Swap width and height
                    sourceRectangle.Width
                ) : sourceRectangle;

            //Origin = isRotated ? new Vector2(origin.Y, origin.X) : origin;
            Origin = isRotated ? new Vector2(origin.Y, SourceRectangle.Height - origin.X) : origin;
            //TODO: Rotated custom origin

            //For a 90 - degree clockwise rotation, regardless of where the origin is:
            //Original X coordinate becomes the new Y coordinate
            //Original Y coordinate becomes the new X coordinate(but inverted / flipped from the height)

            // If origin is at(0, 0) -> becomes(Height - 0, 0)
            //If origin is at center (Width / 2, Height / 2)->becomes(Height - Height / 2, Width / 2)
            //If origin is at bottom - right(Width, Height)->becomes(Height - Height, Width)
            //If origin is at any custom point(x, y) -> becomes(Height - y, x)

            IsRotated = isRotated;
        }

        internal void SetCustomOrigin(Vector2 origin)
        {
            Origin = IsRotated ? new Vector2(origin.Y, SourceRectangle.Height - origin.X) : origin;
        }
    }

    internal class Sprite : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Render;
        internal sealed override bool IsUnique => false;
        internal Texture2D Texture { get; set; } // Maybe default texture
        internal Transform Transform { get; private set; }

        private IReadOnlyList<Frame> _frames;
        internal bool IsAnimated => _frames.Count > 1;
        internal int CurrentFrameIndex { get; set; }

        internal bool FlipX { get; set; }
        internal bool FlipY { get; set; }

        // SpriteRenderr properties
        internal Rectangle SourceRectangle => _currentFrame.SourceRectangle;
        internal Vector2 Origin => _currentFrame.Origin;
        internal bool IsRotated => _currentFrame.IsRotated;
        private Frame _currentFrame => _frames[CurrentFrameIndex];
        internal Color Color { get; set; } = Color.White;
        internal bool Outline { get; set; } = false;
        internal SpriteEffects SpriteEffects => GetSpriteEffects();

        // Size = Width, Height + applied IsRotated
        internal Vector2 Size => new Vector2(_currentFrame.Width, _currentFrame.Height);

        // Center = Origin + applied IsRotated
        internal Vector2 Center => GetCenter();

        // Additional render properties
        //internal virtual int ZOrder { get; set; }
        //internal virtual RenderLayer Layer { get; set; } = RenderLayer.Default;
        //internal virtual bool IsVisible { get; set; } = true;
        //internal virtual float Opacity { get; set; } = 1.0f;

        // Render ordering
        //int ZOrder { get; set; }
        //RenderLayer Layer { get; set; }  // enum

        // Visibility control
        //bool IsVisible { get; set; }
        //float Opacity { get; set; }      // 0.0f to 1.0f

        internal Sprite()
            : base()
        {
            _frames = new List<Frame>();
        }

        internal Sprite(SpriteAsset asset)
            : base()
        {
            Texture = asset.Texture;
            _frames = asset.Frames.ToList();
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
            Transform = gameObject.Transform;
        }

        // Method to update frames for new animation
        internal void SetAnimation(AnimationData animationData)
        {
            Texture = animationData.Texture;
            _frames = animationData.Frames;
        }
        private Vector2 GetCenter()
        {
            if (!_currentFrame.IsRotated)
                return Origin;

            return new Vector2(
                _currentFrame.SourceRectangle.Height - _currentFrame.Origin.Y,
                _currentFrame.Origin.X
            );
        }

        private SpriteEffects GetSpriteEffects()
        {
            if (FlipX && FlipY)
            {
                // When both are flipped, combine the effects
                return SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            }

            if (FlipX)
                return _currentFrame.IsRotated ?
                    SpriteEffects.FlipVertically :      // Flip along y-axis
                    SpriteEffects.FlipHorizontally;     // Flip along x-axis

            if (FlipY)
                return _currentFrame.IsRotated ?
                    SpriteEffects.FlipHorizontally :    // Flip along x-axis
                    SpriteEffects.FlipVertically;       // Flip along y-axis
            return SpriteEffects.None;
        }
    }
}
