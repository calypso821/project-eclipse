using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Eclipse.Engine.Config;
using Eclipse.Engine.Data;

namespace Eclipse.Engine.Data
{
    public enum ColliderType
    {
        [JsonProperty("Box")]
        Box,
        [JsonProperty("Circle")]
        Circle,
        // Could add more like Polygon, Capsule etc.
    }

    public class HitboxData : ObjectData
    {
        public ColliderType ColliderType { get; }
        public float Width { get; }
        public float Height { get; }
        public Vector2 Offset { get; }  // Offset from owner position
        public int MaxTargets { get; }
        public float Duration { get; }

        public HitboxData(string id, HitboxConfig config)
        {
            Id = id;
            ColliderType = config.ColliderType;
            Width = config.Width;
            Height = config.Height;
            Offset = config.Offset.ToVector2();
            MaxTargets = config.MaxTargets;
            Duration = config.Duration;
        }
    }
}
