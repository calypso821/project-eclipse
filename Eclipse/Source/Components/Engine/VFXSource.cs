using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Engine
{
    internal abstract class VFXSource : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.VFX;

        // VFX parameters
        internal bool Loop { get; set; } = false;
        internal bool IsStatic { get; set; } = false; // Non static - attached to object 
        internal float Scale { get; set; } = 1f;
        internal Color Color { get; set; } = Color.White;

        // For manager to track actual playing instances
        internal List<int> ActiveInstances { get; set; } = new();
        internal bool IsPlaying => ActiveInstances.Count > 0;

        internal override void OnReset()
        {
            Stop();
        }

        internal abstract void Stop();

        internal override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
