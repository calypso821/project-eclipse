using System;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Config;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Factories
{
#nullable enable
    internal class TiledObjectConfig : ObjectConfig
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public bool HorizontalFlip { get; set; }
        public bool VerticalFlip { get; set; }
        public Element? Element { get; set; }

        internal TiledObjectConfig(
           string name,
           string className,
           string spriteId,
           bool horizontalFlip = false,
           bool verticalFlip = false,
           Element? element = null,
           Vector2Config? offset = null,
           float rotation = 0.0f,
           Vector2Config? scale = null)
        {
            Name = name;
            ClassName = className;
            SpriteId = spriteId;
            HorizontalFlip = horizontalFlip;
            VerticalFlip = verticalFlip;
            Element = element;
            Offset = offset ?? new Vector2Config(0, 0);
            Rotation = rotation;
            Scale = scale ?? new Vector2Config(1, 1);
        }
    }
#nullable restore

    internal class TiledObjectFactory
    {

        internal TiledObjectFactory()
        {
        }
        public GameObject CreateObject(TiledObjectConfig objConfig)
        {

            var obj = new GameObject(objConfig.SpriteId);

            // Add components
            var spriteAsset = AssetManager.Instance.GetSprite(objConfig.SpriteId);
            var sprite = new Sprite(spriteAsset);
            sprite.FlipX = objConfig.HorizontalFlip;
            sprite.FlipY = objConfig.VerticalFlip;
            obj.AddComponent(sprite);

            // Get from TiledObjectData
            var colliderSize = spriteAsset.GetHalfSizeInUnits();
            bool shouldFlip = Math.Abs(Math.Abs(objConfig.Rotation) - MathHelper.Pi/2) < MathHelper.Pi/4;

            // If near 90/-90, flip width and height, otherwise keep original
            colliderSize = shouldFlip ? new Vector2(colliderSize.Y, colliderSize.X) : colliderSize;

            var collider = new BoxCollider2D(colliderSize, new Vector2(0, 0));
            collider.Layer = CollisionLayer.Platform;
            obj.AddComponent(collider);

            // Element state
            if (objConfig.Element != null)
            {
                var element = objConfig.Element.Value;
                var elementState = new ElementState(element, true);
                obj.AddComponent(elementState);

                //if (element == Element.Neutral)
                //{
                //    sprite.Outline = true;
                //}
            }

            // Get from TiledObjectData
            // Set transform
            obj.Transform.SetTransform(
                objConfig.Offset.ToVector2(),
                objConfig.Rotation,
                objConfig.Scale.ToVector2()
            );

            return obj;
        }
    }
}
