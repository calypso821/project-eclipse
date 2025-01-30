using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;

namespace Eclipse.Components.AI
{
    internal class TargetDetector : Component
    {
        private float _detectionRange = 5f;
        private float _loseTargetRange = 7f;

        private GameObject _detectedTarget;
        private Vector2 _lastKnownPosition;

        internal bool HasTarget => _detectedTarget != null;
        internal Vector2 LastKnownPosition => _lastKnownPosition;
        internal GameObject CurrentTarget => _detectedTarget;

        internal void Configure(float detectionRange, float loseTargetRange)
        {
            _detectionRange = detectionRange;
            _loseTargetRange = loseTargetRange;
        }

        internal override void OnReset()
        {
            base.OnReset();
            _detectedTarget = null;
        }

        internal override void Update(GameTime gameTime)
        {
            UpdateDetection();
        }

        private void UpdateDetection()
        {
            // Find closest target (in your case, probably the player)
            GameObject target = FindNearestTarget();

            // No target detected, stay on current position
            if (target == null)
            {
                _detectedTarget = null;
                return;
            }

            float distanceToTarget = Vector2.Distance(
                target.Transform.Position,
                GameObject.Transform.Position
            );

            // Update detection status
            if (_detectedTarget == null)
            {
                // If no target detected, check if in detection range
                // First detection
                if (distanceToTarget <= _detectionRange)
                {
                    _detectedTarget = target;
                    _lastKnownPosition = target.Transform.Position;
                }
            }
            else
            {
                // If target already detected, check if beyond lose range
                // Continious detection
                if (distanceToTarget > _loseTargetRange)
                {
                    _detectedTarget = null;
                }
                else
                {
                    _lastKnownPosition = target.Transform.Position;
                }
            }
        }

        private GameObject FindNearestTarget()
        {
            // Get player for now
            var target = PlayerManager.Instance.Player;
            return target;
        }
    }
}
