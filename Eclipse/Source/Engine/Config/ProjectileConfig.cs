
namespace Eclipse.Engine.Config
{
    // Config with projectile properties (factory)
    public class ProjectileConfig : ObjectConfig
    {
        public float Speed { get; set; } = 20.0f;
        public float MaxDistance { get; set; } = 50.0f;
        public float Lifetime { get; set; } = 5.0f;
        public float GravityScale { get; set; } = 1.0f;
        public float Mass { get; set; } = 1.0f;
        public bool DestroyOnImpact { get; set; } = true;
    }
}
