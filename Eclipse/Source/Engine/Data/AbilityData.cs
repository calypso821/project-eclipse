using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Components.Animation;
using Eclipse.Engine.Config;

namespace Eclipse.Engine.Data
{
    public enum AbilityType  // More common name than AttackType
    {
        [JsonProperty("Melee")]
        Melee,      // Close-range/hitbox based
        [JsonProperty("Projectile")]
        Projectile,     // Projectile based
        [JsonProperty("Area")]
        Area,       // AoE effects
        // Buff,    // Status effects
        // Support, // Healing/buffs
        // Utility  // Movement/non-combat
    }
    public class AbilityData : ObjectData
    {
        public float Damage { get; }
        public float Cooldown { get; }
        public float Range { get; }
        public AbilityType AbilityType { get; }
        public string AnimationId { get; }
        public Dictionary<string, AnimationData> SpriteAnimations { get; set; }
        public Dictionary<string, TweenData> TransformAnimations { get; set; }

        // Projectile Ability Data
        public string ProjectileId { get; }

        // Melee Ability Data
        public HitboxData HitboxData { get; }

        // Could have additional properties for:
        // - Resource costs
        // - Effects
        // - Animation IDs - SpriteID
        // - Type Of Damage
        public AbilityData(string id, AbilityConfig config)
        {
            Id = id;
            Damage = config.Damage;
            Cooldown = config.Cooldown;
            Range = config.Range;
            AbilityType = config.AbilityType;
            AnimationId = config.AnimationId;

            // Only create hitbox data if config has hitbox settings
            HitboxData = config.Hitbox != null ?
                new HitboxData($"{id}_hitbox", config.Hitbox) :
                null;

            ProjectileId = config.ProjectileId;
        }
    }
}
