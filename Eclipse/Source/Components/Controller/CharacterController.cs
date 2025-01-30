using System;

using Microsoft.Xna.Framework;
using Eclipse.Engine.Core;
using Eclipse.Components.Engine;

namespace Eclipse.Components.Controller
{
    public enum MotionState
    { 
        Idle,
        Move,
        Jump,
        Fall,
        Dash
    }
    internal sealed class CharacterController : Controller
    {
        internal sealed override PassOrder PassOrder => PassOrder.Late;

        // Declares an event that other components can subscribe to
        // Action<Vector2> means this event will
        // pass a Vector2 parameter to subscribers
        internal event Action<Vector2> OnDirectionChanged;
        internal event Action<MotionState> OnMotionStateChanged;

        private const float MOVE_FORCE = 50f;
        private const float DASH_FORCE = 10f;
        private const float AIR_CONTROL_MULTIPLIER = 0.3f;
        private const float JUMP_HEIGHT = 1.5f;

        private Vector2 _direction;
        private MotionState _currentState = MotionState.Idle;
        private bool _isGrounded = false;

        private RigidBody2D _rigidBody;
        private ElementState _colorModifier;

        internal CharacterController()
            : base()
        {
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            // Required component validation 
            _rigidBody = GameObject.GetComponent<RigidBody2D>() ??
                throw new ArgumentException("PlayerController requires RigidBody2D component");

            // Optional component
            _colorModifier = GameObject.GetComponent<ElementState>();
        }
        internal override void OnReset()
        {
            // Reset motion state
            _direction = Vector2.Zero;
            _currentState = MotionState.Idle;
            _isGrounded = false;
        }

        internal override void Update(GameTime gameTime)
        {
            UpdateGroundedState();
        }

        // Called every frame 
        private void UpdateGroundedState()
        {
            // Check if there is change in grounded state
            if (_isGrounded != _rigidBody.IsGrounded)
            {
                _isGrounded = _rigidBody.IsGrounded;
                var newState = !_isGrounded ?
                    MotionState.Jump :
                    _direction.Length() > 0 ? MotionState.Move : MotionState.Idle;
                RaiseMotionStateChanged(newState);
            }
        }

        // Methodes called by Player/Enemy controllers 
        // Based of input actions
        internal void Move(Vector2 newDirection)
        {
            // 1. Update direction if changed
            if (_direction != newDirection)
            {
                // Update direction
                // Only raise direction event
                // - direction.length > 0 
                // - facing changed (left/right)
                if (newDirection.Length() > 0 &&
                    Math.Sign(_direction.X) != Math.Sign(newDirection.X))
                {
                    RaiseDirectionChanged(newDirection);
                }

                _direction = newDirection;   
            }

            // 2. Update motion state
            if (_isGrounded)
            {
                var newState = newDirection.X == 0 ? MotionState.Idle : MotionState.Move;
                if (_currentState != newState)
                {
                    _currentState = newState;
                    RaiseMotionStateChanged(newState);
                }
            }

            ApplyMovement();
        }

        private void ApplyMovement()
        {
            if (_direction == Vector2.Zero) return;

            // Only apply horizontal movement force
            Vector2 moveForce = new Vector2(_direction.X, 0) * MOVE_FORCE;

            if (!_rigidBody.IsGrounded)
            {
                moveForce *= AIR_CONTROL_MULTIPLIER;
            }

            _rigidBody.AddForce(moveForce);
        }

        internal void Jump()  // Public command that can be called
        {
            if (_rigidBody.IsGrounded)
            {
                ApplyJump();
            }
        }

        private void ApplyJump()  // Internal physics application
        {
            float jumpVelocity = MathF.Sqrt(2 * JUMP_HEIGHT * _rigidBody.Gravity);
            _rigidBody.AddImpulse(new Vector2(0, -jumpVelocity));
        }

        internal void Dash(Vector2 direction)
        {
            // TODO
            _rigidBody.AddImpulse(Vector2.Normalize(direction) * DASH_FORCE);
        }

        internal void ToggleColor()
        {
            _colorModifier?.ToggleState();
        }

        internal void NotifyDirectionChange()
        {
            RaiseDirectionChanged(_direction);
        }

        private void RaiseDirectionChanged(Vector2 direction)
        {
            // '?' checks if anyone is subscribed before invoking
            // Invoke calls all subscribed methods, passing _direction as parameter
            OnDirectionChanged?.Invoke(direction);
        }

        private void RaiseMotionStateChanged(MotionState motionState)
        {
            OnMotionStateChanged?.Invoke(motionState);
        }
    }
}
