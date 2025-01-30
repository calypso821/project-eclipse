using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.Xna.Framework;

using Eclipse.Components.Combat;
using Eclipse.Engine.Core;
using Eclipse.Components.Animation;

namespace Eclipse.Components.Controller
{
    internal class WeaponController : Controller
    {
        internal event Action<WeaponState, string> OnWeaponStateChanged;
        internal event Action<Weapon> OnWeaponChanged;

        private Dictionary<string, Weapon> _weapons = new();
        private Weapon _activeWeapon;

        // TODO:
        // MeleeWeapon -> Movement direction
        // RangedWeapon -> Aim direction

        private Vector2 _aimDirection;
        private Vector2 _moveDirection;

        internal WeaponController()
            : base()
        {
        }

        internal void InitializeWeaponState()
        {
            // Initial state
            OnWeaponChanged?.Invoke(_activeWeapon);
            _activeWeapon.UpdateWeaponState();
        }

        internal override void Update(GameTime gameTime)
        {
            if (_activeWeapon == null) return;

            // Let weapon handle its own state
            _activeWeapon.Update(gameTime);
        }

        internal void AddWeapon(GameObject weaponObject)
        {
            var weapon = weaponObject.GetComponent<Weapon>() ??
                throw new ArgumentException($"Weapon Object must have Weapon component attached");

            weaponObject.SetActive(false);
            GameObject.AddChild(weaponObject);
            _weapons.Add(weapon.WeaponData.Id, weapon);
        }

        internal void SetActiveWeapon(string weaponId)
        {
            // Disable current active weapon
            if (_activeWeapon != null)
            {
                _activeWeapon.OnWeaponStateChanged -= HandleWeaponStateChanged;
                _activeWeapon.GameObject.SetActive(false);
            }

            // Enable new weapon
            if (_weapons.TryGetValue(weaponId, out var weapon))
            {
                _activeWeapon = weapon;
                _activeWeapon.OnWeaponStateChanged += HandleWeaponStateChanged;
                _activeWeapon.GameObject.SetActive(true);

                OnWeaponChanged?.Invoke(_activeWeapon);
                _activeWeapon.UpdateWeaponState();
            }
        }
        internal void SetActiveWeapon(int index)
        {
            if (index >= _weapons.Count) return;

            var weaponId = _weapons.Keys.ElementAt(index);
            SetActiveWeapon(weaponId);  // Reuse existing logic
        }

        internal void RemoveWeapon(string weaponId)
        {
            if (_weapons.TryGetValue(weaponId, out var weapon))
            {
                // Clear active reference
                if (_activeWeapon == weapon)
                {
                    _activeWeapon.OnWeaponStateChanged -= HandleWeaponStateChanged;
                    _activeWeapon = null;
                }          

                weapon.GameObject.MarkForDestroy();
                _weapons.Remove(weaponId);
            }
        }

        // Called from Player/Enemy controller
        internal void UpdateAimDirection(Vector2 mousePosition)
        { 
            Vector2 direction = Vector2.Subtract(
                mousePosition,
                GameObject.Transform.Position
            );
            var normDirection = Vector2.Normalize(direction);
            //GameObject.Transform.Direction = normDirection;
            _aimDirection = normDirection;

            // Update Objects rotation
            float angle = (float)Math.Atan2(normDirection.Y, normDirection.X);
            UpdateWeaponRotation(angle);
        }

        internal void UpdateMoveDirection(Vector2 mousePosition)
        {
            //Vector2 direction = Vector2.Subtract(
            //    mousePosition,
            //    GameObject.Transform.Position
            //);
            //direction.Normalize();
            //GameObject.Transform.Direction = direction;
            //_aimDirection = direction;

            //// Update Objects rotation
            //float angle = (float)Math.Atan2(direction.Y, direction.X);
            //UpdateWeaponRotation(angle);
        }

        internal void UpdateWeaponRotation(float angle)
        {
            if (_activeWeapon == null) return;

            if (_activeWeapon is RangedWeapon rangedWeapon)
                rangedWeapon.UpdateRotation(angle);
        }

        internal void StartAttack()
        {
            _activeWeapon?.TryAttack(_aimDirection);
        }

        internal void EndAttack()
        {
            _activeWeapon?.EndAttack(_aimDirection);
        }

        // Needed? 
        //internal void CancelChargeAttack()
        //{
        //    if (_activeWeapon == null) return;
        //    _activeWeapon.CancelCharge();
        //}

        private void HandleWeaponStateChanged(WeaponState weaponState, string animation)
        {
            // Update subscribers
            OnWeaponStateChanged?.Invoke(weaponState, animation);
        }
    }
}
