using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Eclipse.Engine.Physics.Collision;
using Eclipse.Engine.Core;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Systems.Physics
{
    internal class PhysicsSystem : ComponentSystem
    {
        private List<Collider2D> _colliders = new();
        private List<RigidBody2D> _rigidBodies = new();
        private List<Collision2D> _collisions = new();

        private bool _isDirtyColliders = false;
        private bool _isDirtyRigidbodies = false;

        // For debug
        internal IList<Collider2D> Colliders => _colliders;
        internal IList<RigidBody2D> RigidBodies => _rigidBodies;

        private List<Collision2D> _debugCollisions = new();
        internal IList<Collision2D> Collisions => _debugCollisions;

        public override void Register(GameObject gameObject)
        {
            // Check which Flag invoked register
            // Collider OR RigidBody OR Both

            if (gameObject.HasDirtyFlag(DirtyFlag.Collider))
            {
                // 1. Add colliders
                RegisterColliders(gameObject);
            }
            if (gameObject.HasDirtyFlag(DirtyFlag.RigidBody))
            {
                // 2. Add Rigidbody
                RegisterRigidBody(gameObject);
            }
        }

        public override void Unregister(GameObject gameObject)
        {
            if (gameObject.HasDirtyFlag(DirtyFlag.Collider))
                _isDirtyColliders = true;

            if (gameObject.HasDirtyFlag(DirtyFlag.RigidBody))
                _isDirtyRigidbodies = true;
        }
        public override void Cleanup()
        {
            // Remove component which are dirty
            if (_isDirtyColliders)
                CleanupColliders();

            if (_isDirtyRigidbodies)
                CleanupRigidBodies();
        }

        public override void Clear()
        {
            _colliders.Clear();
            _rigidBodies.Clear();
            _collisions.Clear();

            _isDirtyColliders = false;
            _isDirtyRigidbodies = false;
        }

        public override void Update(GameTime gameTime)
        {

            // 0. QuadTree Traverse

            // 1. Simulate physics (apply forces, integrate velocities)
            SimulatePhysics(gameTime); // (rigid bodies)
            // 2. Detect Collisions (colliders)
            DetectCollisions();
            // 3. Resolve Collisions (collisions)
            ResolveCollisions();
            // 4. Update final positions (actual transform)
            SetFinalPositions();
        }

        private void SimulatePhysics(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var rigidBody in _rigidBodies)
            {
                if (!rigidBody.IsEnabled || !rigidBody.GameObject.IsActive) continue;
                if (rigidBody.GameObject.IsStatic || rigidBody.IsKinematic) continue;

                // Update Forces, Velocity, Gravity...
                UpdatePhysics(rigidBody, dt);
            }
        }

        private void UpdatePhysics(RigidBody2D rigidBody, float dt)
        {
            // Reset grounded state at start of update
            // Will be set true if ground is detected in collision
            rigidBody.IsGrounded = false;

            // Apply gravity first
            Vector2 newVelocity = rigidBody.Velocity;
            newVelocity.Y += rigidBody.Gravity * dt;

            // Apply accumulated forces (F = ma -> a = F/m)
            Vector2 acceleration = rigidBody.AccumulatedForces / rigidBody.Mass;

            // Update velocity
            // v = v0 + at
            newVelocity += acceleration * dt;

            //Apply drag(air resistance) [0, 10]
            float dragFactor = MathF.Exp(-rigidBody.Drag * dt);
            newVelocity *= dragFactor;

            // Apply ground friction only when grounded and no forces are being applied
            //if (rigidBody.IsGrounded && rigidBody.AccumulatedForces == Vector2.Zero)
            //{
            //    // Apply friction as a deceleration
            //    float frictionDeceleration = rigidBody.GroundFriction * dt;
            //    float xSpeedReduction = MathF.Min(MathF.Abs(newVelocity.X), frictionDeceleration);
            //    newVelocity.X -= xSpeedReduction * MathF.Sign(newVelocity.X);
            //}


            if (rigidBody.AccumulatedForces == Vector2.Zero)
            {
                // Apply friction as a percentage reduction of velocity
                newVelocity.X *= 1f - rigidBody.GroundFriction * dt * 20;
            }



            // Limit only horizontal speed
            float horizontalSpeed = MathF.Abs(newVelocity.X);
            if (horizontalSpeed > rigidBody.MaxHorizontalSpeed)
            {
                newVelocity.X = MathF.Sign(newVelocity.X) * rigidBody.MaxHorizontalSpeed;
            }

            // Set very small velocities to zero to prevent continuous calculation
            if (newVelocity.Length() < 0.01f)
            {
                //Console.WriteLine("Zero");
                newVelocity = Vector2.Zero;
            }

            // Update state
            rigidBody.Velocity = newVelocity;

            // Initialize with current position
            rigidBody.NextPosition = rigidBody.GameObject.Transform.Position;

            // Update position
            // x = x0 + vt
            rigidBody.NextPosition += newVelocity * dt;

            // Clear forces
            rigidBody.ClearForces();
        }

        private void DetectCollisions()
        {
            foreach (var collider in _colliders)
            {
                if (!collider.IsEnabled || !collider.GameObject.IsActive) continue;

                foreach (var otherCollider in _colliders)
                {
                    if (collider == otherCollider) continue; // Skip self
                    // Skip already checked (A to B == B to A)
                    if (collider.InstanceId > otherCollider.InstanceId) continue;
                    if (!otherCollider.IsEnabled || !otherCollider.GameObject.IsActive) continue;

                    // Get world positions
                    collider.GameObject.Transform.GetWorldTransform(
                        out Vector2 position,
                        out float rotation,
                        out Vector2 scale
                    );

                    otherCollider.GameObject.Transform.GetWorldTransform(
                        out Vector2 otherPosition,
                        out float otherRotation,
                        out Vector2 otherScale
                    );
                    // + Transfrom size with scale
                    var posA = position + collider.Offset;
                    var posB = otherPosition + otherCollider.Offset;

                    // AABB intersection check
                    if (collider is BoxCollider2D boxA && otherCollider is BoxCollider2D boxB)
                    {
                        var collisionOccurred = CollisionDetection2D.AABBIntersects(
                            posA,
                            boxA.HalfSize * scale,
                            posB,
                            boxB.HalfSize * otherScale
                        );

                        if (collisionOccurred)
                        {
                            var collision = CollisionDetection2D.GetCollisionInfo(
                                posA, boxA.HalfSize * scale,
                                posB, boxB.HalfSize * otherScale,
                                collider, otherCollider
                            );

                            _collisions.Add(collision);
                            //Console.WriteLine("Collided!");
                        }
                    }
                }
            }
        }

        private void ResolveCollisions()
        {
            // TODO: IF DEBUG...
            // Debug collisons
            _debugCollisions.Clear();
            _debugCollisions.AddRange(_collisions); // Keep copy for debug drawing

            //Console.WriteLine(_collisions.Count);
            foreach (var collision in _collisions)
            {
                // Ignore collisions with same mask (light - light, dark - dark)
                if ((collision.ColliderA.Mask == ElementMask.Light && collision.ColliderB.Mask == ElementMask.Light) ||
                    (collision.ColliderA.Mask == ElementMask.Dark && collision.ColliderB.Mask == ElementMask.Dark))
                    continue;

                // Try to get special colliders (hitbox, projectile)
                var specialCollider = GetColliderWithLayer(collision, CollisionLayer.Hitbox) ??
                                    GetColliderWithLayer(collision, CollisionLayer.Projectile);

                if (specialCollider != null)
                {
                    // Only special collider handles the collision
                    specialCollider.GameObject.HandleCollision(collision);
                }
                else
                {
                    // Normal collision
                    // HandleCollision on both Colliders
                    collision.ColliderA.GameObject.HandleCollision(collision);
                    collision.ColliderB.GameObject.HandleCollision(collision);
                }
            }

            _collisions.Clear();
        }

        private Collider2D GetColliderWithLayer(Collision2D collision, CollisionLayer layer)
        {
            if (collision.ColliderA.Layer == layer)
                return collision.ColliderA;

            if (collision.ColliderB.Layer == layer)
                return collision.ColliderB;

            return null;
        }

        private void SetFinalPositions()
        {
            //Console.WriteLine("Setting final...");
            foreach (var rigidBody in _rigidBodies)
            {
                // Skip disabled/inactive objects
                if (!rigidBody.IsEnabled || !rigidBody.GameObject.IsActive) continue;

                // Update final position of objects
                // Only update to 4 decimal difference
                var diff = Vector2.Subtract(rigidBody.NextPosition, rigidBody.GameObject.Transform.Position);
                if (diff.Length() > 0.0001f)
                {
                    //Console.WriteLine("Tranfrom UPDATE!!!");
                    rigidBody.GameObject.Transform.Position = rigidBody.NextPosition;
                }
            }
        }

        private void RegisterColliders(GameObject gameObject)
        {
            foreach (var collider in gameObject.GetComponents<Collider2D>())
            {
                if (!collider.IsRegistered)
                {
                    _colliders.Add(collider);
                    collider.IsRegistered = true;
                }
                collider.ClearDirty();
            }
        }
        private void RegisterRigidBody(GameObject gameObject)
        {
            if (gameObject.GetComponent<RigidBody2D>() is RigidBody2D rigidBody)
            {
                if (!rigidBody.IsRegistered)
                {
                    _rigidBodies.Add(rigidBody);
                    rigidBody.IsRegistered = true;
                }
                rigidBody.ClearDirty();
            }
        }

        private void CleanupColliders()
        {
            _colliders.RemoveAll(component =>
            {
                if (component.IsDirty())
                {
                    // Cleanup component flags
                    component.ClearDirty();
                    // Cleanup component states
                    component.IsRegistered = false;

                    return true;
                }
                return false;
            });
            _isDirtyColliders = false;
        }

        private void CleanupRigidBodies()
        {
            _rigidBodies.RemoveAll(component =>
            {
                if (component.IsDirty())
                {
                    // Cleanup component flags
                    component.ClearDirty();
                    component.IsRegistered = false;

                    return true;
                }
                return false;
            });
            _isDirtyRigidbodies = false;
        }
    }
}