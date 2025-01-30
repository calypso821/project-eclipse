using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Components.Engine;

namespace Eclipse.Components.Combat
{
    internal class DamageCollider : DamageDealer
    {
        internal HitboxData HitboxData { get; }

        private Collider2D _hitboxCollider;
        private HashSet<GameObject> _hitTargets = new();

        private float _hitboxTimer;
        private bool _isActive = false;
        internal bool IsActive => _isActive;

        internal DamageCollider(HitboxData hitboxData)
            : base()
        {
            HitboxData = hitboxData;
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            _hitboxCollider = GameObject.GetComponent<Collider2D>() ??
                throw new ArgumentException("DamageCollider requires Collider2D component");

            GameObject.AddComponent(new ElementState(Element.Neutral, true));


            // Disabled state by default
            Disable();
        }
        internal override void OnReset()
        {
            Disable();
        }
        internal override void Configure(DamageData damageData)
        {
            base.Configure(damageData);
            GameObject.Transform.Position = damageData.Position;

            // Set collision mask element state
            GameObject.GetComponent<ElementState>().SetState(damageData.Element);
        }

        internal override void OnEnable()
        {
            // Enable collider
            _hitboxCollider.Enable();
            // Start damage checking
            _hitboxTimer = HitboxData.Duration;
            //Console.WriteLine("Hitbox duration: " + HitboxData.Duration);
            _isActive = true;
        }
        internal override void OnDisable()
        {
            // Disable collider
            _hitboxCollider.Disable();
            // Stop damage checking
            _isActive = false;
            _hitboxTimer = 0;
            _hitTargets.Clear();
        }

        // Invoked by (Source) Ability 
        internal override void Update(GameTime gameTime)
        {
            if (!_isActive) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _hitboxTimer -= dt;
            if (_hitboxTimer <= 0)
            {
                Disable();
            }
        }

        internal override void OnHit(GameObject target)
        {
            // Already hit this target
            if (_hitTargets.Contains(target))
                return;

            // Apply Damage if Damagable
            // target.TakeDamage(), source.OnDamageDealt() 
            base.OnHit(target);

            // If target was damageable (hit was successful)
            if (target is IDamageable)
            {
                // Prevent multiple hits on same target
                _hitTargets.Add(target);

                // Check max targets after successful hit
                if (HitboxData.MaxTargets > 0 &&
                    _hitTargets.Count >= HitboxData.MaxTargets)
                {
                    Disable();
                }
            }
        }
    }
}
