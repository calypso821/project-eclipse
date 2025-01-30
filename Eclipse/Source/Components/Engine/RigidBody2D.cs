using Microsoft.Xna.Framework;

using Eclipse.Engine.Physics.Collision;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Engine
{
    internal sealed class RigidBody2D : Component
    {
        internal sealed override DirtyFlag DirtyFlag => DirtyFlag.RigidBody;

        private static float GRAVITY = 9.81f;
        public float GravityScale { get; set; } = 3.5f;
        public float Gravity => GRAVITY * GravityScale;

        private Vector2 _nextPosition;
        public Vector2 NextPosition
        {
            get => _nextPosition;
            set => _nextPosition = value;
        }


        private Vector2 _velocity = Vector2.Zero;
        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        private Vector2 _accumulatedForces = Vector2.Zero;
        public Vector2 AccumulatedForces => _accumulatedForces;

        private float _groundCheckDistance = 0.1f;

        // Physical properties that affect motion
        public float Mass { get; set; } = 1.0f; // Mass of object
        public float Drag { get; set; } = 1.0f; // Air resistance coefficient
        public float MaxHorizontalSpeed { get; set; } = 5.0f; // Speed limit
        public float GroundFriction { get; set; } = 0.9f; // Friction of surface (1 - max, 0 - min)

        public bool IsKinematic { get; set; } = false; // If true, not affected by forces
        public bool IsGrounded { get; set; } = false;

        // Regular RigidBody (Dynamic):
        // - IsKinematic = false
        // - IsStatic = false (gameObject)
        // - Moved by physics

        // Kinematic RigidBody:
        // - IsKinematic = true
        // - IsStatic = false (gameObject)
        // - Moved by code (like moving platforms)

        // Static objects:
        // - Don't have RigidBody
        // - Or IsStatic = true (gameObject)
        // - Never move

        internal RigidBody2D()
            : base()
        {
        }
        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
            gameObject.OnCollision += OnCollision;
        }

        internal override void OnReset()
        {
            // Reset motion state
            _velocity = Vector2.Zero;
            _accumulatedForces = Vector2.Zero;
            IsGrounded = false;
        }

        internal override void OnDestroy()
        {
            GameObject.OnCollision -= OnCollision;
            base.OnDestroy();
        }

        internal void ClearForces()
        {
            _accumulatedForces = Vector2.Zero;
        }

        // For continuous forces (like pushing)
        public void AddForce(Vector2 force)
        {
            if (IsKinematic) return;

            _accumulatedForces += force;
        }

        // For instant forces (like jumping or impacts)
        public void AddImpulse(Vector2 impulse)
        {
            if (IsKinematic) return;

            _velocity += impulse;
        }

        // Called by event
        private void OnCollision(Collision2D collision)
        {
            if (GameObject.IsStatic || IsKinematic) return;

            // Move this object out of collision

            // 1. When ground is B and player is A:
            // Normal points from ground (B)to player(A) - positive
            // When player is above ground, normal points UP(Y > 0)
            // When player is below ground (grounded), normal points DOWN(Y < 0)
            // Ground to player, if negative (Player is below)

            // 2. When player is B and ground is A:
            // Normal points from player (B)to ground(A) - negative
            // When player is above ground, normal points DOWN(Y < 0)
            // When player is below ground (grounded), normal points UP(Y > 0)
            // Player to ground, if positive (Player is below)

            // Check if colliderA is objectA
            // True -> order (B to A)
            // False -> order (A to B) need to negate to match correct oder
            bool isColliderA = collision.ColliderA == GameObject.GetComponent<Collider2D>();

            // If this is object A, move in normal direction (since normal points to A)
            // If this is object B, move against normal direction
            float moveScale = isColliderA ? 1f : -1f;
            _nextPosition += collision.Normal * collision.Depth * moveScale;

            if (collision.Normal.Y != 0)
            {
                bool isGroundCollision = isColliderA ?
                    collision.Normal.Y < 0 :  // If we're A, grounded when normal points down
                    collision.Normal.Y > 0;   // If we're B, grounded when normal points up

                if (isGroundCollision)
                {
                    IsGrounded = true;
                    if (_velocity.Y > 0)
                    {
                        _velocity.Y = 0;
                    }
                }
            }

            // Bounce physics
            float bounceCoefficient = 0.5f; // Adjust for bounciness
            float minBounceVelocity = 0.1f; // Threshold to prevent tiny bounces

            Vector2 relativeVelocity = _velocity;
            float normalVelocity = Vector2.Dot(relativeVelocity, collision.Normal);

            // Only bounce if moving fast enough towards the surface
            if (normalVelocity < -minBounceVelocity)
            {
                Vector2 bounceVelocity = -normalVelocity * collision.Normal * bounceCoefficient;
                _velocity += bounceVelocity;
            }
        }

        // Properties setters for configuration
        public void SetPhysicsProperties(float mass, float drag, float groundFriction, float maxSpeed)
        {
            Mass = mass;
            Drag = drag;
            GroundFriction = groundFriction;
            MaxHorizontalSpeed = maxSpeed;
        }
    }
}
