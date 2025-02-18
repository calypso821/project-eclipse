
using System.Collections.Generic;
using Eclipse.Components.Combat;

namespace Eclipse.Engine.Config
{
    public class WeaponConfig : ObjectConfig
    {
        public int BaseDamage { get; set; } = 10;
        public float AttackRate { get; set; } = 60.0f;
        public float AttackDuration { get; set; } = 0f;
        public bool IsChargeable { get; set; } = false;
        public float ChargeMultiplier { get; set; } = 3.0f;
        public float ChargeTime { get; set; } = 2.0f;
        public string WeaponType { get; set; }
        public Dictionary<WeaponState, string> CharacterAnimations { get; set; } = null;
        public string ProjectileId { get; set; }
        public HitboxConfig Hitbox { get; set; } = null;
    }
}
