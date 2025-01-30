using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Eclipse.Components.Engine;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Engine.Factories;
using Eclipse.Engine.Systems.Render;

namespace Eclipse.Components.Combat
{
    internal class RangedWeapon : Weapon, IProjectileDamageSource
    {
        // For FirePositon calcualtion
        private Sprite _weaponSprite;

        // Where weapon sits relative to character
        private Vector2 _baseOffset;

        internal RangedWeapon(WeaponData weaponData, Sprite weaponSprite, Vector2 offset = default)
            : base(weaponData)
        {
            _baseOffset = offset;
            _weaponSprite = weaponSprite;
        }
        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
        }

        internal override void AttackAction(Vector2 aimDirection, float damage, Element element)
        {
            var damageData = new DamageData
            {
                Amount = damage,
                Source = this,
                Element = element,
                Direction = aimDirection,
                Position = GetFirePosition()
            };

            ProjectileFactory.Instance.SpawnProjectile(
                WeaponData.ProjectileId,
                damageData
            );
        }

        public Vector2 GetFirePosition()
        {
            var (width, height) = _weaponSprite.Size;

            float widthOffset = width * 0.15f;
            var x = width + widthOffset;
            // not flieped (y) -> 1/4 height
            // flipped (y) -> 3/4 height
            //var y = _sprite.FlipY ? 3 * height / 4.0f : height / 4.0f;
            var y = height / 2.0f;

            var center = _weaponSprite.Center;

            var edgePoint = new Vector2(x, y);
            // Origin to edgePoint
            var originToEdge = Vector2.Subtract(edgePoint, _weaponSprite.Center);

            // Convert to units (from pixels - sprite)
            var originToEdgeUnits = PPU.ToUnits(originToEdge);

            return originToEdgeUnits;
        }

        internal void UpdateRotation(float angle)
        {
            // Handle weapon rotation
            GameObject.Transform.Rotation = angle;

            // Handle flipping based on angle
            bool shouldFlip = Math.Abs(angle) > MathHelper.Pi / 2;

            // Flip X - axis offset
            // Only flip X component, keep Y as is
            GameObject.Transform.Position = new Vector2(
                shouldFlip ? -_baseOffset.X : _baseOffset.X,  // Flip X
                _baseOffset.Y                                 // Keep Y unchanged
            );

            var weaponSprites = GameObject.GetComponents<Sprite>();
            foreach (var sprite in weaponSprites)
            {
                sprite.FlipY = shouldFlip;
            }

            // Notify parent actor to flip if needed
            var parentSprites = GameObject.Parent.GetComponents<Sprite>();
            foreach (var sprite in parentSprites)
            {
                sprite.FlipX = shouldFlip;
            }
        }
    }
}
