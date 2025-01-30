using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    internal abstract class UIElement : Component
    {
        internal UIObject UIObject { get; private set; }


        internal virtual void OnInitialize(UIObject uiObject)
        {
            UIObject = uiObject;
        }
    }
}
