
using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Animation
{
    internal abstract class Animator : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Animator;
        internal sealed override bool IsUnique => false;

        internal bool IsPlaying { get; set; } = false;
        internal string CurrentAnimation { get; set; }
        //internal string DefaultAnimation { get; set; }

        internal Animator()
            : base()
        {
            //DefaultAnimation = defaultAnimation;
        }

        // Core animation control methods used by both
        internal abstract void SetAnimation(string animationId, bool forceRestart);
        internal abstract override void Update(GameTime gameTime);
        internal abstract void Play(bool restart);
        internal abstract void Stop();
        internal abstract void Pause();
    }
}
