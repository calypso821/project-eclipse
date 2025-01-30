using System;
using System.Collections.Generic;

using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;

namespace Eclipse.Components.Engine
{
    internal abstract class AudioSource : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Audio;

        internal virtual bool Loop { get; set; } = false;
        internal virtual bool AllowOverlap { get; set; } = false;
        internal virtual bool Is3D { get; set; } = false;

        // Audio parameters
        internal float Volume { get; set; } = 1f;  // 0.0 to 1.0, affected by master volume
        internal float BasePitch { get; set; } = 1f;  // Base pitch value, typically 1.0
        internal float Pitch { get; set; } = 1f;  // Current pitch, can be randomized from base
        internal float Pan { get; set; } = 0f;  // Stereo position: -1.0 (left) to 1.0 (right)


        // For manager to track actual playing instance
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
