using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Components.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Eclipse.Engine.Systems.Render
{
    internal class VFXSystem : ISystem, IDrawableSystem
    {
        private IReadOnlyDictionary<int, VFXEmitter> _activeEmitters;

        private readonly SpriteBatch _spriteBatch;
        private readonly CameraManager _cameraManager;

        internal VFXSystem(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _cameraManager = CameraManager.Instance;

            _activeEmitters = VFXManager.Instance.ActiveEmitters;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var kvp in _activeEmitters)
            {
                var emitter = kvp.Value;
                var emitterId = kvp.Key;

                emitter.Update(gameTime); // Update animation, position

                if (!emitter.IsPlaying)
                {
                    VFXManager.Instance.ReleaseEmitter(emitterId);
                    continue;
                }
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin(transformMatrix: _cameraManager.ScreenViewMatrix);
            //Console.WriteLine("Active emitters: " + _activeEmitters.Count);
            foreach (var emitter in _activeEmitters.Values)
            {
                if (!emitter.IsPlaying) continue;

                var renderObject = emitter.Sprite;
                var VFXData = emitter.VFXData;

                var texture = renderObject.Texture;
                if (texture == null) continue;

                var position = PPU.ToPixels(emitter.CurrentPosition);
                var rotation = renderObject.IsRotated ? MathHelper.Pi / 2 : 0.0f;

                _spriteBatch.Draw(
                    texture,
                    position,
                    renderObject.SourceRectangle,
                    VFXData.Color,
                    rotation,
                    renderObject.Origin,
                    VFXData.Scale,
                    emitter.SpriteEffects, // FlipX if needed (non static)
                    0f
                );
            }

            _spriteBatch.End();
        }
    }
}
