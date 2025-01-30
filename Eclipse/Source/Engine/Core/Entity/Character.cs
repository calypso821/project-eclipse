using Microsoft.Xna.Framework;
using System;
using Eclipse.Components.Animation;
using Eclipse.Components.Engine;
using Eclipse.Components.Controller;
using Eclipse.Engine.Data;
using Eclipse.Components.Combat;

namespace Eclipse.Engine.Core
{

    internal abstract class Character : GameObject, IDamageable
    {
        // Controllers - RaiseStateCHanged()
        internal CharacterController _characterController;
        internal WeaponController _weaponController;

        // Animators - updated animtion based of state
        internal SpriteAnimator _spriteAnimator;

        private SpriteAnimator _baseAnimator;    // Movement
        private SpriteAnimator _overlayAnimator; // Weapon + Full attacks

        // True - Aim based flip
        // False - move direction based flip
        internal virtual bool FlipWithAim { get; set; } = false;

        internal CharacterData ActorData { get; }
        internal Stats Stats { get; }

        internal Character(CharacterData actorData, string name)
            : base(name) 
        {
            ActorData = actorData;

            Stats = new Stats(actorData);
            AddComponent(Stats);
            Stats.OnDeath += OnDeath;
        }

        internal override void Start()
        {
            // GameObject start
            base.Start();

            _characterController = GetComponent<CharacterController>();

            // Subscribe to events only if components exist
            if (_characterController != null)
            {
                _characterController.OnMotionStateChanged += HandleMotionStateChanged;
                _characterController.OnDirectionChanged += HandleDirectionChanged;
            }

            _weaponController = GetComponent<WeaponController>();

            // Weapon state update
            if (_weaponController != null)
            {
                _weaponController.OnWeaponStateChanged += HandleWeaponStateChanged;
                _weaponController.OnWeaponChanged += HandleWeaponChanged;
                _weaponController.InitializeWeaponState();
            }

            // By default 
            // MotionState --> SpriteAnimtor animtions
            _spriteAnimator = GetComponent<SpriteAnimator>();

            // Can be overriden in child classes
        }
        internal void SetAnimators(SpriteAnimator baseAnimator, SpriteAnimator overlayAnimator = null)
        {
            _baseAnimator = baseAnimator;
            _overlayAnimator = overlayAnimator;
        }
        public void TakeDamage(DamageData data)
        {
            Stats.TakeDamage(data.Amount);
        }

        protected virtual void OnDeath()
        {
            // Added to _toDestroy() - scene (cleans events, components...)
            MarkForDestroy();
        }

        internal override void Destroy()
        {
            Stats.OnDeath -= OnDeath;

            if (_characterController != null)
            {
                _characterController.OnMotionStateChanged -= HandleMotionStateChanged;
                _characterController.OnDirectionChanged -= HandleDirectionChanged;
            }
            if(_weaponController != null)
            {
                _weaponController.OnWeaponStateChanged -= HandleWeaponStateChanged;
                _weaponController.OnWeaponChanged -= HandleWeaponChanged;
            }

            // GameObject OnDestroy()
            base.Destroy();
        }

        // Invoked by CharacterController
        internal virtual void HandleDirectionChanged(Vector2 direction)
        {
            //Console.WriteLine("new Direction" + FlipWithAim);
            // Used only in direction based flip
            if (!FlipWithAim)
            {
                var flipX = direction.X < 0;

                if (_spriteAnimator != null) _spriteAnimator.FlipX = flipX;
                if (_overlayAnimator != null) _overlayAnimator.FlipX = flipX;
            }
 
        }
        
        internal virtual void HandleMotionStateChanged(MotionState motionState)
        {
            string animationName = motionState switch
            {
                //MotionState.Move => velocity > _runThreshold ? "run" : "walk",
                MotionState.Move => "Walk",
                MotionState.Idle => "Idle",
                MotionState.Jump => "Jump",
                _ => ""
            };

            if (_spriteAnimator != null && _spriteAnimator.HasAnimation(animationName))
            {
                _spriteAnimator.SetAnimation(animationName);
            }
        }
        // Invoked by WeaponController

        private void HandleWeaponStateChanged(WeaponState state, string animation)
        {
            if (state == WeaponState.Attack)
            {
                // Priority animation on overlay, hide base
                // Disable base sprite
                _baseAnimator.HideSprite();
            }
            else
            {
                // Normal state - both layers active
                _baseAnimator.ShowSprite();
            }

            // Set animation 
            if (_overlayAnimator != null)
            {
                if (animation == null)
                {
                    // No animation corresponding current weapon state
                    // Disable overlay animator + hide sprite
                    _overlayAnimator.Disable();
                }
                else if (_overlayAnimator.HasAnimation(animation))
                {
                    // Set new overlay animation
                    _overlayAnimator.SetAnimation(animation);
                    _overlayAnimator.Enable();
                }
            }
        }
        private void HandleWeaponChanged(Weapon weapon)
        {
            // Ranged - true
            // Meleee - false
            if (weapon.WeaponData.WeaponType == WeaponType.Ranged)
            {
                if (_spriteAnimator != null) _spriteAnimator.FlipX = false;
                if (_overlayAnimator != null) _overlayAnimator.FlipX = false;
                FlipWithAim = true;
            }
            else
            {
                _characterController.NotifyDirectionChange();
                FlipWithAim = false;
            }
        }
    }
}