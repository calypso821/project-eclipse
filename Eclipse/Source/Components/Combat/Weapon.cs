using System;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Components.Engine;
using Eclipse.Components.Controller;
using Eclipse.Engine.Data;
using Eclipse.Components.Animation;
using Microsoft.Xna.Framework.Input;
using Eclipse.Engine.Physics.Collision;

namespace Eclipse.Components.Combat
{
    public enum WeaponState
    {
        Idle,       // Pointing down
        Ready,      // Before attack (fire)
        Charge,     // While charging,
        Attack,     // Attack duration
        Cooldown   // After attack -> between attacks
    }

    internal abstract class Weapon : Component, IDamageSource
    {
        public event Action<WeaponState, string> OnWeaponStateChanged;

        internal WeaponData WeaponData { get; }

        private float _attackCooldownTimer = 0f; // Current attack timer
        private float _attackCooldown;  // Total time between attacks (attack + cooldown)

        private float _attackTimer = 0f; 
        private float _attackDuration;  // Duration of attack

        private bool _isCharging = false;
        private float _chargeTime = 0f; // Time of charge
        internal bool IsCharging => _isCharging;

        private WeaponState _currentState = WeaponState.Ready;

        private SFXSource _audioSource;
        private VFXSource _vfxSource;
        private SpriteAnimator _spriteAnimator;

        internal Weapon(WeaponData weaponData)
            : base()
        {
            WeaponData = weaponData;

            // AttackRate -> per min
            _attackCooldown = 60 / weaponData.AttackRate;
            _attackDuration = weaponData.AttackDuration;
        }
        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            _audioSource = GameObject.GetComponent<SFXSource>();
            _vfxSource = GameObject.GetComponent<VFXSource>();
            _spriteAnimator = GameObject.GetComponent<SpriteAnimator>();
        }

        internal override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update timers
            if (_attackCooldownTimer > 0)
            {
                _attackCooldownTimer -= dt;
            }
            if (_attackTimer > 0)
            {
                _attackTimer -= dt;
            }

            // Update charge time
            if (_isCharging && _chargeTime < WeaponData.ChargeTime)
            {
                _chargeTime += dt;
                _chargeTime = Math.Min(_chargeTime, WeaponData.ChargeTime);
            }

            // Ready state
            if (CanAttack() && !_isCharging && _currentState != WeaponState.Ready)
            {
                _currentState = WeaponState.Ready;
                UpdateWeaponState();
                return;
            }

            // Cooldown state 
            if (_attackTimer <= 0 && _attackCooldownTimer > 0 &&
                !_isCharging && _currentState != WeaponState.Cooldown)
            {
                _currentState = WeaponState.Cooldown;
                UpdateWeaponState();
                return;
            }

            // Attack State
            if (_attackTimer > 0 && _currentState != WeaponState.Attack)
            {
                _currentState = WeaponState.Attack;
                UpdateWeaponState();
                return;
            }
        }

        private bool CanAttack()
        {
            return _attackCooldownTimer <= 0 && _attackTimer <= 0;
        }

        // Normal attack
        internal bool TryAttack(Vector2 direction)
        {
            if (!CanAttack()) return false;

            _attackCooldownTimer = _attackCooldown; // reset colodown timer

            if (WeaponData.IsChargeable)
            {
                // Start charging
                StartCharge();
            }
            else
            {
                // Normal attack (instant)
                Attack(direction);
            }

            return true;
        }

        private void Attack(Vector2 direction, bool isCharged = false)
        {
            var attackElement = GameObject.GetComponent<ElementState>().State;

            float damage = WeaponData.BaseDamage;
            if (isCharged)
            {
                damage *= GetChargeMultiplier();
            }
            Console.WriteLine(damage);

            AttackAction(direction, damage, attackElement);

            // Play audio
            if (_audioSource != null)
                _audioSource.Play("Attack");
     

            _attackTimer = _attackDuration; // reset attack timer 

            // Set attack state
            _currentState = WeaponState.Attack;
            UpdateWeaponState();
        }
        internal void EndAttack(Vector2 direction)
        {
            if (IsCharging)
            {
                // Charged attack - release
                Attack(direction, isCharged: true);
                _isCharging = false;
            }
        }

        internal abstract void AttackAction(Vector2 direction, float damage, Element element);

        // Charged attack
        internal void StartCharge()
        {
            _isCharging = true;
            _chargeTime = 0;

            // Set charge state
            _currentState = WeaponState.Charge;
            UpdateWeaponState();
        }

        private float GetChargePercentage()
        {
            return Math.Min(_chargeTime / WeaponData.ChargeTime, 1f);
        }

        private float GetChargeMultiplier()
        {
            float chargePercent = GetChargePercentage();

            // Could use curve here for non-linear scaling
            // Charge = 0% --> multipler = 1.0
            // Charge = 100% --> multipler = WeaponData.ChargeMultiplier
            return 1f + (WeaponData.ChargeMultiplier - 1f) * chargePercent;
        }

        // New method for charged attacks
        internal virtual void ChargedAttack(Vector2 direction, float chargeMultiplier)
        {
            // Default implementation - override in derived classes
            ///Attack(direction);  // Can use chargeMultiplier to modify damage/effects
        }

        internal void UpdateWeaponState()
        {
            var weaponState = _currentState;

            string animationName = weaponState switch
            {
                WeaponState.Idle => "Idle",
                WeaponState.Ready => "Ready",
                WeaponState.Charge => "Charge",
                WeaponState.Attack => "Attack",
                WeaponState.Cooldown => "Cooldown",
                _ => ""
            };

            // Set weapon animation
            if (_spriteAnimator != null && _spriteAnimator.HasAnimation(animationName))
            {
                _spriteAnimator.SetAnimation(animationName);
            }

            // Get chacrater aniamtion (weapon owner)
            string characterAnimation = 
                WeaponData.CharacterAnimations?.TryGetValue(weaponState, out string animation) == true
                ? animation
                : null;

            // Broadcast the state change
            OnWeaponStateChanged?.Invoke(weaponState, characterAnimation);
        }

        public void OnDamageDealt(GameObject target, Collision2D collision)
        {
            // Handle post-damage effects
            // VFX
            // SFX
            // status effects...

            // Element color
            var color = GameObject.GetComponent<ElementState>().Color;
            if (_vfxSource != null)
                _vfxSource.Play("OnHit", collision.Point, collision.Normal, 1.5f, color);

            if (_audioSource != null)
                _audioSource.Play("OnHit");

        }
    }
}
