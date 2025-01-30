using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Systems;
using Eclipse.Engine.Systems.Render;

namespace Eclipse.Engine.UI
{
    internal class UICanvas  // or UIRoot/UIGraph
    {
        private UIObject _root;
        internal UIObject Root => _root;

        private readonly CanvasRenderer _canvasRenderer;
        private readonly UISystem _uiSystem;
        private readonly SystemManager _systemManager;

        internal UICanvas(CanvasRenderer canvasRenderer, UISystem uiSystem)
        {
            _systemManager = SystemManager.Instance;
            //_canvasRenderer = _systemManager.GetSystem<CanvasRenderer>();
            _canvasRenderer = canvasRenderer;
            _uiSystem = uiSystem;
            var root = new UIObject();
            //root.OnAddedToCanvas(this);
            _root = root;
        }

        public void AddObject(UIObject uiObject)
        {
            _root.AddChild(uiObject);
            Register(uiObject);
        }

        internal void Register(UIObject uiObject)
        {
            // Register with UI systems
            _canvasRenderer.Register(uiObject);
            // Register any additional UI systems (animation, input, etc.)
            _uiSystem.Register(uiObject);
        }

        //internal void Unregister(UIObject uiObject)
        //{
        //    _canvasRenderer.Unregister(uiObject);
        //    // Unregister from other UI systems
        //}

        internal void Unload()
        {
            _root.Destroy();
            _canvasRenderer.Clear();
            _uiSystem.Clear();
            _root = null;
        }
    }
}
