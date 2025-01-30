using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;

namespace Eclipse.Components.Controller
{
    enum PassOrder
    {
        Early,    // State updates, cooldowns
        Default,    // Core gameplay decisions/actions
        Late      // Final movements
    }

    internal abstract class Controller : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Controller;
        internal sealed override bool IsUnique => false;

        internal virtual PassOrder PassOrder => PassOrder.Default; // Default

        internal Controller()
            : base()
        {
        }
        internal abstract override void Update(GameTime gameTime);
    }
}
