using System;

using Microsoft.Xna.Framework;

using Eclipse.Components.AI;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;

namespace Eclipse.Components.Controller
{
    internal enum AIState
    {
        Idle,
        Follow,
        Attack
    }
    internal class AIController : Controller
    {
        internal EnemyData EnemyData { get; }
        internal sealed override PassOrder PassOrder => PassOrder.Default;

        private CharacterController _characterController;
        private AbilityController _abilityController;
        private TargetDetector _targetDetector;

        // AI Configuration
        private float _detectionRange;
        private float _loseTargetRange;
        private float _preferredAttackRange; // Default range AI tries to maintain

        internal string _defaultAbility;

        // State tracking
        private AIState _currentState = AIState.Idle;

        internal AIController(EnemyData enemyData)
            : base()
        {
            EnemyData = enemyData;

            // AI Configuration
            _detectionRange = enemyData.DetectionRange;
            _loseTargetRange = enemyData.LoseTargetRange;
            _preferredAttackRange = enemyData.PreferredRange;

            _defaultAbility = enemyData.Abilities.Count > 0 ?
                              enemyData.Abilities[0] :
                              null;
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);

            // Get required components
            _characterController = GameObject.GetComponent<CharacterController>() ??
                throw new ArgumentException("AIController requires CharacterController component");

            _abilityController = GameObject.GetComponent<AbilityController>() ??
                throw new ArgumentException("AIController requires AbilityController component");

            _targetDetector = GameObject.GetComponent<TargetDetector>() ??
                throw new ArgumentException("AIController requires TargetDetector component");

            _currentState = AIState.Idle;

            _targetDetector.Configure(_detectionRange, _loseTargetRange);
        }

        internal override void OnReset()
        {
            // Reset motion state
            _currentState = AIState.Idle;

        }

        internal override void Update(GameTime gameTime)
        {
            // 1. First update detection to know about targets
            _targetDetector.Update(gameTime);

            // 2. Then update AI state based on detection results
            UpdateAIState(gameTime);

            // 3. Finally execute behavior based on current state
            // Idle, Follow, Attack
            //Console.WriteLine(_currentState);
            ExecuteCurrentState(gameTime);
        }

        private void UpdateAIState(GameTime gameTime)
        {
            if (!_targetDetector.HasTarget)
            {
                _currentState = AIState.Idle;
                return;
            }

            float distanceToPlayer = Vector2.Distance(
                _targetDetector.LastKnownPosition,
                GameObject.Transform.Position
            );

            bool hasUsableAbilityInRange = _abilityController.HasUsableAbilityInRange(distanceToPlayer);

            if (hasUsableAbilityInRange)
            {
                _currentState = AIState.Attack;
            }
            else
            {
                _currentState = AIState.Follow;
            }

            //switch (_currentState)
            //{
            //    case AIState.Idle:
            //        _currentState = AIState.Follow;
            //        break;

            //    case AIState.Follow:
            //        // If in range for any attack, transition to attack state
            //        if (hasUsableAbilityInRange)
            //        {
            //            _currentState = AIState.Attack;
            //        }
            //        break;

            //    case AIState.Attack:
            //        // Return to chase if out of range for all attacks
            //        if (!hasUsableAbilityInRange)
            //        {
            //            _currentState = AIState.Follow;
            //        }
            //        break;
            //}
        }

        private void ExecuteCurrentState(GameTime gameTime)
        {
            switch (_currentState)
            {
                case AIState.Idle:
                    HandleIdleState();
                    break;
                case AIState.Follow:
                    HandleFollowState();
                    break;
                case AIState.Attack:
                    HandleAttackState();
                    break;
            }
        }

        private void HandleIdleState()
        {
            _characterController.Move(Vector2.Zero);
        }

        private void HandleFollowState()
        {
            Vector2 directionToPlayer = Vector2.Subtract(
                _targetDetector.LastKnownPosition,
                GameObject.Transform.Position
            );

            float distanceToPlayer = directionToPlayer.Length();
            //Vector2 normDirection = Vector2.Normalize(directionToPlayer);

            // 1 unit of sapce where not moving (jittering)
            // If too close, move away, if too far, move closer
            if (distanceToPlayer < _preferredAttackRange - 0.5f)
            {
                _characterController.Move(-directionToPlayer); // Back away
            }
            else if (distanceToPlayer > _preferredAttackRange + 0.5f)
            {
                _characterController.Move(directionToPlayer); // Move closer
            }
            else
            {
                _characterController.Move(Vector2.Zero); // Stay in position
            }
        }

        private void HandleAttackState()
        {
            Vector2 directionToPlayer = Vector2.Subtract(
                _targetDetector.LastKnownPosition,
                GameObject.Transform.Position
            );

            float distanceToPlayer = directionToPlayer.Length();
            //Vector2 normDirection = Vector2.Normalize(directionToPlayer);

            // Choose attack based on distance and cooldowns
            if (_abilityController.InRange(_defaultAbility, distanceToPlayer) &&
                _abilityController.CanActivate(_defaultAbility))
            {
                _characterController.Move(Vector2.Zero); // Stop to attack
                _abilityController.TryActivate(_defaultAbility, directionToPlayer);
            }
            //else if (distanceToPlayer <= _areaRange && _attackController.CanUseAttack("area_stomp"))
            //{
            //    _motionController.Move(Vector2.Zero);
            //    _attackController.TryAttack("area_stomp", directionToPlayer);
            //}
            //else if (distanceToPlayer <= _projectileRange && _attackController.CanUseAttack("projectile_spit"))
            //{
            //    _motionController.Move(Vector2.Zero);
            //    _attackController.TryAttack("projectile_spit", directionToPlayer);
            //}
            //else
            //{
            //    // If no attacks available, return to folow state
            //    _currentState = AIState.Follow;
            //}
            _currentState = AIState.Follow;
        }
    }
}
