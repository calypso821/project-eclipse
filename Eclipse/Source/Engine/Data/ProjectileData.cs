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

        public ProjectileData(string id, ProjectileConfig config)
        {
            Id = id;
            Speed = config.Speed;
            MaxDistance = config.MaxDistance;
            Lifetime = config.Lifetime;
            GravityScale = config.GravityScale;
            Mass = config.Mass;
            DestroyOnImpact = config.DestroyOnImpact;
        }
    }
}
