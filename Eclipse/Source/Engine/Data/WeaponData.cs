using System;
using System.Collections.Generic;

using Eclipse.Engine.Config;
using Eclipse.Components.Combat;

namespace Eclipse.Engine.Data
{
    public enum WeaponType
    {
        Melee,
        Ranged  // Projectile/Hitscan??
    }

    public class WeaponData : ObjectData
    {
        public int BaseDamage { get; set; }
        public float AttackRate { get; set; }
        public float AttackDuration { get; set; }
        public bool IsChargeable { get; set; }
        public float ChargeMultiplier { get; set; }
        public float ChargeTime { get; set; }
        public HitboxData HitboxData { get; }
        public WeaponType WeaponType { get; set; }

        // Map weapon states to character animations
        public Dictionary<WeaponState, string> CharacterAnimations { get; set; }

        // Maybe weakpoint multipler??
        //public float HeadshotMultiplier { get; set; }
        public string ProjectileId { get; set; }

        public WeaponData(string id, WeaponConfig config)
        {
            Id = id;
            BaseDamage = config.BaseDamage;
            AttackRate = config.AttackRate;
            AttackDuration = config.AttackDuration;
            IsChargeable = config.IsChargeable;
            ChargeMultiplier = config.ChargeMultiplier;
            ChargeTime = config.ChargeTime;
            WeaponType = GetWeaponType(config.WeaponType);
            CharacterAnimations = config.CharacterAnimations;

            ProjectileId = config.ProjectileId;

            HitboxData = CreateHitboxData(config.Hitbox);
        }

        public WeaponType GetWeaponType(string weaponType)
        {
            if (Enum.TryParse<WeaponType>(weaponType, true, out var type))
                return type;
            throw new Exception($"Invalid WeaponType: {weaponType}");
        }
        private HitboxData CreateHitboxData(HitboxConfig hitboxConfig)
        {
            if (hitboxConfig == null) return null;

            // Override hitbox duration
            hitboxConfig.Duration = AttackDuration;
            return new HitboxData($"{Id}_hitbox", hitboxConfig);
        }
    }
}
