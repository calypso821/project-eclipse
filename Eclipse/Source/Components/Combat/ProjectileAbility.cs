using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Engine.Factories;
using Eclipse.Components.Engine;

namespace Eclipse.Components.Combat
{
    internal class ProjectileAbility : Ability, IProjectileDamageSource
    {
        private readonly ProjectileFactory _projectileFactory;

        internal ProjectileAbility(AbilityData abilityData)
            : base(abilityData)
        {
            _projectileFactory = ProjectileFactory.Instance;
        }

        internal override void Activate(Vector2 aimDirection, float damage, Element element)
        {
            var damageData = new DamageData
            {
                Amount = damage,
                Source = this,
                Element = element,
                Direction = aimDirection,
                Position = GetFirePosition()
            };

            // Configure(damageData) in factory 
            // Set DamageDealer properties
            ProjectileFactory.Instance.SpawnProjectile(
                AbilityData.ProjectileId,
                damageData
            );
        }

        public Vector2 GetFirePosition()
        {
            // Origin of Ability -> GameObject owner
            return new Vector2(0, 0);
        }

        public void OnProjectileTimeout(GameObject projectile)
        {
            _projectileFactory.DespawnProjectile(projectile);
        }
    }
}
