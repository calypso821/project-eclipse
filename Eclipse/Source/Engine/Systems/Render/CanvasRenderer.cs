using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Components.UI;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Systems.Render
{
    internal class CanvasRenderer : ComponentSystem<UIVisual>, IDrawableSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<Type, Action<UIVisual>> _renderActions;

        internal CanvasRenderer(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _renderActions = new Dictionary<Type, Action<UIVisual>>
            {
                { typeof(UIImage), RenderImage },
                { typeof(UIText), RenderText }
            };
        }

        public void Register(UIObject uiObject)
        {
            foreach (var uiElement in uiObject.Components)
            {
                if (uiElement is UIVisual uiVisual && !uiVisual.IsRegistered)
                {
                    _components.Add(uiVisual);  // Add the cast version
                    uiVisual.IsRegistered = true;
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            // Handle sorting, visibility checks etc.
        }

        public void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (var element in _components)
            {
                if (!element.IsEnabled || !element.UIObject.Active)
                    continue;

                if (_renderActions.TryGetValue(element.GetType(), out var renderAction))
                {
                    renderAction(element);
                }
            }

            _spriteBatch.End();
        }

        private void RenderImage(UIVisual element)
        {
            var image = (UIImage)element;
            if (image.Texture == null) return;

            _spriteBatch.Draw(
                image.Texture,
                image.ScreenPosition,
                image.SourceRectangle,
                image.Color,
                0f,
                image.Origin,
                image.UIObject.Transform.Scale,  // Use scale from transform instead of Vector2.One
                SpriteEffects.None,
                0f
            );
        }

        private void RenderText(UIVisual element)
        {
            var text = (UIText)element;
            if (string.IsNullOrEmpty(text.Text) || text.Font == null) return;

            _spriteBatch.DrawString(
                text.Font,
                text.Text,
                text.ScreenPosition,
                text.Color,
                0f,
                text.Origin,
                text.Size,
                SpriteEffects.None,
                0f
            );
        }
    }
}
