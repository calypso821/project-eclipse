

namespace Eclipse.Engine.Config
{
    public class TweenConfig
    {
        public string Name { get; set; }
        public Vector2Config StartPosition { get; set; } = new Vector2Config(0f, 0f);
        public Vector2Config EndPosition { get; set; } = new Vector2Config(0f, 0f);

        public float StartRotation { get; set; } = 0f;
        public float EndRotation { get; set; } = 0f;

        public Vector2Config StartScale { get; set; } = new Vector2Config(1.0f, 1.0f);
        public Vector2Config EndScale { get; set; } = new Vector2Config(1.0f, 1.0f);
        public float Duration { get; set; } = 1.0f;
        public bool PingPong { get; set; } = false;
        public bool IsLooping { get; set; } = false;
    }
}
