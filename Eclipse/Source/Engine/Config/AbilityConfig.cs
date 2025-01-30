
using Eclipse.Engine.Data;

namespace Eclipse.Engine.Config
{
    public class AbilityConfig : ObjectConfig
    {
        public float Damage { get; } = 30.0f;
        public float Cooldown { get; set; } = 5.0f;
        public float Range { get; set; } = 5.0f;
        public AbilityType AbilityType { get; set; } = AbilityType.Melee;
        public string AnimationId { get; set; } = null;
        public HitboxConfig Hitbox { get; set; } = null;
        public string ProjectileId { get; set; } = null;
    }
}
