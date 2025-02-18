using System;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;
using Eclipse.Engine.Factories;
using Eclipse.Components.Engine;
using Eclipse.Engine.Physics.Collision;

namespace Eclipse.Components.Combat
{
    internal class Projectile : DamageDealer
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.Projectile;
        internal ProjectileData ProjectileData { get; }
        private RigidBody2D _rigidBody;
        //private float _lifetime;

        private SFXSource _audioSource;
        private VFXSource _vfxSource;

        private Vector2 _spawnPosition;

        internal Projectile(ProjectileData projectileData)
            : base()
        {
            ProjectileData = projectileData;
            
        }
        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            // Required component validation 
            _rigidBody = GameObject.GetComponent<RigidBody2D>() ??
                throw new ArgumentException("Projectile requires a RigidBody2D component");

            // Not controlled by PhysicsSystem
            // Manually updated in PhysicsSystem
            _rigidBody.IsKinematic = true;

            // Optional component
            _audioSource = GameObject.GetComponent<SFXSource>();
            _vfxSource = GameObject.GetComponent<VFXSource>();
        }

        internal override void Configure(DamageData damageData)
        {
            base.Configure(damageData);

            // Set projectile element state
            GameObject.GetComponent<ElementState>().SetState(damageData.Element);

            //_lifetime = Data.Lifetime

            // Tranfrom into world position of source object (weapon)
            var worldFirePosition = Vector2.Transform(
                damageData.Position,
                damageData.Source.GameObject.Transform.WorldMatrix
            );

            GameObject.Transform.Position = worldFirePosition;
            _rigidBody.NextPosition = worldFirePosition;
            _spawnPosition = worldFirePosition;

            var direction = damageData.Direction;
            float projRotation = (float)Math.Atan2(direction.Y, direction.X);
            GameObject.Transform.Rotation = projRotation;
            _rigidBody.Velocity = direction * ProjectileData.Speed;
        }

        internal override void Update(GameTime gameTime)
        {
            //Console.WriteLine(GameObject.Name);
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var currentPosition = _rigidBody.GameObject.Transform.Position;
            //Console.WriteLine(currentPosition);
            _rigidBody.NextPosition = currentPosition + _rigidBody.Velocity * dt;


            // Position actually changing...

            // Check for max distance
            if (Vector2.DistanceSquared(
                GameObject.Transform.Position,
                _spawnPosition) > ProjectileData.MaxDistance * ProjectileData.MaxDistance)
            {
                Despawn();
            }
        }

        // TODO: For LifeTime update 
        // Creae LifeTimeSysstem (register, unregister
        // LifeTime component!!!

        internal override void OnHit(GameObject target, Collision2D collision)
        {
            // Apply Damage if Damagable
            // target.TakeDamage(), source.OnDamageDealt() 
            base.OnHit(target, collision);

            // Element color
            var color = GameObject.GetComponent<ElementState>().Color;
            // visuasl effect
            if (_vfxSource != null)
                _vfxSource.Play("OnHit", collision.Point, collision.Normal, 0.5f, color);
            // audio
            if (_audioSource != null)
                _audioSource.Play("OnHit");
       

            // Cleanup
            if (ProjectileData.DestroyOnImpact)
            {
                Despawn();
            }
        }

        private void Despawn()
        {
            ProjectileFactory.Instance.DespawnProjectile(GameObject);
        }
    }
}