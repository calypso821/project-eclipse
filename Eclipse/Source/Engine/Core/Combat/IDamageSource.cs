using Microsoft.Xna.Framework;
using Eclipse.Components.Engine;
using Eclipse.Engine.Physics.Collision;

namespace Eclipse.Engine.Core
{
    // Base interface for anything that can deal damage
    interface IDamageSource : ISystemComponent
    {
        void OnDamageDealt(GameObject target, Collision2D collision);
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
