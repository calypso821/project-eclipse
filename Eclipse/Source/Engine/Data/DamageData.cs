using Microsoft.Xna.Framework;
using Eclipse.Components.Engine;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Data
{
    internal struct DamageData
    {
        internal float Amount;
        internal IDamageSource Source;
        internal Element Element;
        internal Vector2 Direction;
        internal Vector2 Position;
    }
}
