
using Eclipse.Engine.Data;

namespace Eclipse.Engine.Config
{
    public class HitboxConfig : ObjectConfig
    {
        public ColliderType ColliderType { get; set; } = ColliderType.Box;
        public float Width { get; set; } = 1.0f;
        public float Height { get; set; } = 1.0f;
        public override Vector2Config Offset { get; set; } = new Vector2Config(0f, 0f);
        public int MaxTargets { get; set; } = 1;
        public float Duration { get; set; } = 1.0f;
    }
}
