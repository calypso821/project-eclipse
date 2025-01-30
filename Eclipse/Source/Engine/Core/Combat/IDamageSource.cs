using Microsoft.Xna.Framework;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Core
{
    // Base interface for anything that can deal damage
    interface IDamageSource : ISystemComponent
    {
        void OnDamageDealt(GameObject target);
    }

    interface IMeleeDamageSource : IDamageSource
    {
        //void OnMeleeHit(GameObject target); Implementes IDamageDealer
    }

    interface IProjectileDamageSource : IDamageSource
    {
        // Barrel position

        // Implementes IDamageDealer
        //void OnProjectileHit(GameObject target, GameObject projectile);
    }

    //interface IRaycastDamageSource : IDamageSource
    //{
    //    Vector2 GetFirePosition();
    //    // No timeout needed for raycast
    //}
}
