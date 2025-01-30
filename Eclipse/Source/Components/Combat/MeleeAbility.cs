using System;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Data;
using Eclipse.Engine.Core;
using Eclipse.Components.Engine;
using Eclipse.Engine.Factories;

namespace Eclipse.Components.Combat
{
    internal class MeleeAbility : Ability, IMeleeDamageSource
    {
        private DamageCollider _hitbox;

        internal MeleeAbility(AbilityData abilityData)
            : base(abilityData)
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
            SetupHitbox(AbilityData.HitboxData);
        }

        internal override void Update(GameTime gameTime)
        {
            // Update cooldown
            base.Update(gameTime);

            if (IsActive)
            {
                // Update activeHitbox timer
                _hitbox.Update(gameTime);

                // When hitbox finishes its duration, it disables itself
                if (!_hitbox.IsActive)
                {
                    // + Ability Inactive
                    IsActive = false;
                    //Console.WriteLine("End ability");
                }
            }
        }

        internal override void Activate(Vector2 direction, float damage, Element element)
        {
            // Set Hitbox: Offset based of direction
            var damageData = new DamageData
            {
                Amount = damage,
                Source = this,
                Element = element,
                Direction = direction,
                Position = GetHitboxOffset(direction)
            };

            // Configure(damageData)
            // Set DamageDealer properties
            _hitbox.Configure(damageData);

            _hitbox.Enable();
            IsActive = true;
        }
        private Vector2 GetHitboxOffset(Vector2 direction)
        {
            // Base offset from config
            Vector2 baseOffset = AbilityData.HitboxData.Offset;

            // Flip offset.X based on direction
            // Assuming right = positive X, left = negative X
            float offsetX = direction.X < 0 ? -baseOffset.X : baseOffset.X;

            return new Vector2(offsetX, baseOffset.Y);
        }
    }
}
