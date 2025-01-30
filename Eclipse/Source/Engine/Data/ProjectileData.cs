using Eclipse.Engine.Config;

namespace Eclipse.Engine.Data
{
    // Data with projectile properties (runtime)
    public class ProjectileData : ObjectData
    {
        public float Speed { get; set; }
        public float MaxDistance { get; set; }
        public float Lifetime { get; set; }
        public float GravityScale { get; set; }
        public float Mass { get; set; }
        public bool DestroyOnImpact { get; set; }

        // Audio
        public string FireAudioId { get; set; }     // Release audio
        public string ImpactAudioId { get; set; }   // Impact/Hit audio

        public ProjectileData(string id, ProjectileConfig config)
        {
            Id = id;
            Speed = config.Speed;
            MaxDistance = config.MaxDistance;
            Lifetime = config.Lifetime;
            GravityScale = config.GravityScale;
            Mass = config.Mass;
            DestroyOnImpact = config.DestroyOnImpact;
            FireAudioId = config.FireAudioId;
            ImpactAudioId = config.ImpactAudioId;
        }
    }
}
