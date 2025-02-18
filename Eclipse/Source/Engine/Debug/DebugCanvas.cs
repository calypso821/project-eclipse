using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Eclipse.Components.UI;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;

namespace Eclipse.Engine.Debug
{
    internal class DebugCanvas : IDrawableSystem
    {
        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _pixel;

        internal DebugCanvas(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw()
        {
            var canvas = UIManager.Instance.ActiveCanvas;
            if (canvas?.Root == null) return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            DrawElementRecursive(canvas.Root);
            _spriteBatch.End();

            // TODO: Get objects from CanvasRenderer
        }

        private void DrawElementRecursive(UIObject uiObject)
        {
            var button = uiObject.GetComponent<UIButton>();
            if (button != null)
            {
                DrawButton(button);
            }

            var textInput = uiObject.GetComponent<UITextInput>();
            if (textInput != null)
            {
                DrawTextInput(textInput);
            }

            foreach (var child in uiObject.Children)
            {
                DrawElementRecursive(child);
            }
        }

        private void DrawButton(UIButton button)
        {
            _spriteBatch.Draw(_pixel, button.Bounds, Color.Red * 0.5f);
        }
        private void DrawTextInput(UITextInput textInput)
        {
            _spriteBatch.Draw(_pixel, textInput.Bounds, Color.Green * 0.5f);
        }
    }
}
