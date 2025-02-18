using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Components.Engine;
using Eclipse.Engine.Physics.Collision;

namespace Eclipse.Components.Combat
{
    internal abstract class Ability : Component, IDamageSource
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Ability;
        internal sealed override bool IsUnique => false;

        // TODO: Set need? 
        // remainngCooldonw - cooldownTimer

        private float _abilityTimer = 0f; // Current cooldown timer
        private float _abilityCooldown;   // Base cooldown time

        internal AbilityData AbilityData { get; }
        internal bool IsActive { get; set; }

        internal Ability(AbilityData abilityData)
        {
            AbilityData = abilityData;
            IsActive = false;

            _abilityCooldown = abilityData.Cooldown;
        }

        internal override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Ability is on cooldown
            if (_abilityTimer > 0)
            {
                _abilityTimer -= dt;
            }
        }
        internal bool CanActivate()
        {
            return _abilityTimer <= 0 && !IsActive;
        }

        internal bool TryActivate(Vector2 direction, Element abilityElement)
        {
            if (!CanActivate()) return false;

            var damage = AbilityData.Damage;

            Activate(direction, damage, abilityElement);
            _abilityTimer = _abilityCooldown; // reset colodown timer

            // Play audio
            //if (!string.IsNullOrEmpty(WeaponData.AttackAudioId))
            //{
            //    _audioSource.Play(WeaponData.AttackAudioId);
            //}

            return true;
        }

        internal abstract void Activate(Vector2 direction, float damage, Element element);

        public virtual void OnDamageDealt(GameObject target, Collision2D collision)
        {
            // Handle effects, animations, etc
        }
    }
}
