

namespace Eclipse.Engine.Config
{
    public class SpriteAnimationConfig
    {
        public string Name { get; set; }  // "walk", "idle", etc.
        public string SpriteId { get; set; }
        public float Duration { get; set; } = 1.0f;
        public bool IsLooping { get; set; } = false;
        public Vector2Config Origin { get; set; } = null;
    }
}
