
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Components.Engine;
using System;

namespace Eclipse.Engine.Systems.Render
{
    public static class PPU
    {
        // Core definition
        public const float Value = 100f;  // 100 pixels = 1 unit

        // Conversion helpers
        public static float ToPixels(float units) => units * Value;
        public static float ToUnits(float pixels) => pixels / Value;

        public static Vector2 ToPixels(Vector2 units) => units * Value;
        public static Vector2 ToUnits(Vector2 pixels) => pixels / Value;

        // For final screen space transformation
        public static Matrix GetProjectionMatrix() => Matrix.CreateScale(Value);
    }

    internal class SpriteRenderer : ComponentSystem<Sprite>, IDrawableSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly CameraManager _cameraManager;

        // Shader (static color)
        private Effect _colorEffect;
        private Effect _outlineEffect;

        internal SpriteRenderer(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _cameraManager = CameraManager.Instance;

            _colorEffect = AssetManager.Instance.GetShader("ColorShader");
            _outlineEffect = AssetManager.Instance.GetShader("OutlineShader");
        }

        public override void Update(GameTime gameTime)
        {
            // Could be used for sorting
            // Camera culling...
        }

        public void Draw()
        {
            // Set the color you want (here it's red),
            //_colorEffect.Parameters["CustomColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1));


            _spriteBatch.Begin(
                transformMatrix: _cameraManager.ScreenViewMatrix,
                sortMode: SpriteSortMode.Immediate,
                //effect: _colorEffect
                effect: _outlineEffect
            );

            foreach (var renderObject in _components)
            {
                if (!renderObject.IsEnabled || !renderObject.GameObject.IsActive) continue;

                //Console.WriteLine(renderObject.GameObject.Name);
                var texture = renderObject.Texture;
                var transform = renderObject.Transform;

                if (texture == null || transform == null) continue;

                transform.GetWorldTransform(out Vector2 position, out float rotation, out Vector2 scale);
                // LocalSpace (units) -> WorldSpace (pixels)
                position = PPU.ToPixels(position);
                rotation += renderObject.IsRotated ? MathHelper.Pi / 2 : 0.0f;


                // Color shader
                //if (renderObject.Color == Color.White)
                //{
                //    _colorEffect.Parameters["CustomColor"].SetValue(new Vector4(1f, 1f, 1f, 1));
                //}
                //else if (renderObject.Color == Color.Black)
                //{
                //    _colorEffect.Parameters["CustomColor"].SetValue(new Vector4(0f, 0f, 0f, 1));
                //}
                //else if (renderObject.Color == Color.Gray)
                //{
                //    _colorEffect.Parameters["CustomColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1));
                //}

                // Apply the shader before drawing
                //_colorEffect.CurrentTechnique.Passes[0].Apply();

                // Outline shader
                float outlineWidth = 0.003f; // Adjust based on your needs
                var outlineColor = new Color(120, 120, 120, 255).ToVector4();

                if (renderObject.Outline)
                {
                    // Set up and apply effect only for outlined objects
                    _outlineEffect.Parameters["OutlineColor"].SetValue(outlineColor);
                    _outlineEffect.Parameters["OutlineWidth"].SetValue(outlineWidth);
                    _outlineEffect.Parameters["DrawOutline"].SetValue(true);
                }
                else
                {
                    _outlineEffect.Parameters["DrawOutline"].SetValue(false);
                }

                _outlineEffect.Parameters["OriginalColor"].SetValue(renderObject.Color.ToVector4());
                _outlineEffect.CurrentTechnique.Passes[0].Apply();


                //spriteBatch.Draw(
                //    texture, (sprite)
                //    position, (tranfrom)
                //    sourceRect, (sprite)
                //    color, (sprite)
                //    rotation, (transfrom)
                //    origin, (sprite)  
                //    scale, (transfrom)
                //    effects, - None
                //    layerDepth (Sprite)
                //);


                _spriteBatch.Draw(
                    texture,
                    position,
                    renderObject.SourceRectangle,
                    renderObject.Color,
                    rotation,
                    renderObject.Origin,
                    scale,
                    renderObject.SpriteEffects,
                    0                     
                );


            }

            _spriteBatch.End();
        }
    }
}

