using Microsoft.Xna.Framework;
using System;

using Eclipse.Components.Engine;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Engine.Factories;


namespace Eclipse.Components.Combat
{
    internal class MeleeWeapon : Weapon, IMeleeDamageSource
    {
        private DamageCollider _hitbox;
        private bool _isActiveHitbox = false;

        internal MeleeWeapon(WeaponData weaponData)
            : base(weaponData)
        {
        }

        internal void SetupHitbox(HitboxData hitboxData)
        {
            if (hitboxData == null) return;

            var hitbox = new GameObject();
            var collider = PhysicsFactory.Instance.CreateHitboxCollider(hitboxData);
            hitbox.AddComponent(collider);
            var damageCollider = new DamageCollider(hitboxData);
            hitbox.AddComponent(damageCollider);
            GameObject.AddChild(hitbox);

            _hitbox = damageCollider;
        }
        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
            SetupHitbox(WeaponData.HitboxData);
        }
        internal override void Update(GameTime gameTime)
        {
            // Update cooldown
            base.Update(gameTime);

            if (_isActiveHitbox)
            {
                // Update activeHitbox timer
                _hitbox.Update(gameTime);

                // When hitbox finishes its duration, it disables itself
                if (!_hitbox.IsActive)
                {
                    // + Ability Inactive
                    _isActiveHitbox = false;
                    //Console.WriteLine("End melee attack");
                }
            }
        }

        internal override void AttackAction(Vector2 direction, float damage, Element element)
        {
            var damageData = new DamageData
            {
                Amount = damage,
                Source = this,
                Element = element,
                Direction = direction,
                Position = GetHitboxOffset(direction)
            };
            //Console.WriteLine(element);

            // Configure(damageData)
            // Set DamageDealer properties
            _hitbox.Configure(damageData);

            _hitbox.Enable();
            _isActiveHitbox = true;
        }

        private Vector2 GetHitboxOffset(Vector2 direction)
        {
            // Base offset from config
            Vector2 baseOffset = WeaponData.HitboxData.Offset;

            // Flip offset.X based on direction
            // Assuming right = positive X, left = negative X
            float offsetX = direction.X < 0 ? -baseOffset.X : baseOffset.X;

            return new Vector2(offsetX, baseOffset.Y);
        }
    }
}
