using System;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Engine.Physics.Collision;

namespace Eclipse.Components.Combat
{
    internal abstract class DamageDealer : Component
    {
        internal DamageData DamageData { get; private set; }

        // Shared functionality
        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
            gameObject.OnCollision += OnCollision;
        }

        internal override void OnDestroy()
        {
            GameObject.OnCollision -= OnCollision;
            base.OnDestroy();
        }

        internal virtual void Configure(DamageData damageData)
        {
            DamageData = damageData;
        }

        // Invoked by GameObject.OnCollision() <--- PhysicsSystem
        internal virtual void OnCollision(Collision2D collision)
        {
            // DamageDealer = Projectile, DamageCollider...
            // Collison happend with DamageDealer and other object
            // DamageDealer itselfe is GameObject
            // Set other to be object
            var other =
                collision.ColliderA.GameObject == GameObject ?
                collision.ColliderB :
                collision.ColliderA;

            // If other is source (owner) ignore
            if (other.GameObject == DamageData.Source.GameObject.Parent)
                return;

            OnHit(other.GameObject, collision);
        }

        internal virtual void OnHit(GameObject target, Collision2D collision)
        {
            if (target is IDamageable damageable)
            {
                //Console.WriteLine("Target hit: " + target);

                // Stats -> tageDamage
                damageable.TakeDamage(DamageData);

                // Invoke animation, VFX, SFX...
                DamageData.Source.OnDamageDealt(target, collision);
            }
        }
    }
}
