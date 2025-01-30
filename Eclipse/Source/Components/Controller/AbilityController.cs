using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Eclipse.Components.Animation;
using Eclipse.Components.Combat;
using Eclipse.Components.Engine;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Controller
{
    internal enum AbilityState
    {
        Idle,
        Active,
        //Cooldown
    }

    internal class AbilityController : Controller
    {
        internal sealed override PassOrder PassOrder => PassOrder.Early;

        internal event Action<AbilityState> OnAbilityStateChanged;

        private readonly Dictionary<string, Ability> _abilities = new();
        private AbilityState _currentState = AbilityState.Idle;
        private string _currentAbility;

        private Tweener _tweener;

        internal AbilityController()
            : base()
        {
        }

        internal override void OnInitialize(GameObject gameObject)
        {
            base.OnInitialize(gameObject);
            _tweener = GameObject.GetComponent<Tweener>();
        }

        internal void AddAbility(string id, Ability ability)
        {
            _abilities.Add(id, ability);

            // Setup Transform animations
            if (_tweener != null)
            {
                foreach (var kvp in ability.AbilityData.TransformAnimations)
                {
                    _tweener.AddAnimation(kvp.Key, kvp.Value);
                }
            }
        }

        internal override void Update(GameTime gameTime)
        {
            // Update current ability state
            if (_currentState == AbilityState.Active)
            {
                // Check if ability is still active
                var currentAbility = _abilities[_currentAbility];
                if (!currentAbility.IsActive)
                {
                    _currentState = AbilityState.Idle;
                    OnAbilityStateChanged?.Invoke(_currentState);
                }
            }
            //Console.WriteLine(_currentState);
        }
        internal bool InRange(string abilityId, float distance)
        {
            ///Console.WriteLine(distance);
            return _abilities.TryGetValue(abilityId, out var ability) &&
                       distance <= ability.AbilityData.Range;
        }

        internal bool CanActivate(string abilityId)
        {
            return _abilities.TryGetValue(abilityId, out var ability) &&
                   ability.CanActivate() &&
                   _currentState == AbilityState.Idle;
        }
        internal bool HasUsableAbilityInRange(float distance)
        {
            return _abilities.Any(kvp => InRange(kvp.Key, distance) && CanActivate(kvp.Key));
        }

        internal bool HasAbilityInRange(float distance)
        {
            return _abilities.Any(kvp => InRange(kvp.Key, distance));
        }

        internal void TryActivate(string abilityId, Vector2 direction)
        {
            if (!CanActivate(abilityId)) return;

            // Set object direction
            var normDirection = Vector2.Normalize(direction);
            //GameObject.Transform.Direction = normDirection;

            // Get owner element, or could be manually set
            var attackElement = GameObject.GetComponent<ElementState>().State;

            _currentAbility = abilityId;
            var ability = _abilities[abilityId];
            ability.TryActivate(normDirection, attackElement);

            RaiseAbilityStateChanged(AbilityState.Active);
        }

        private void RaiseAbilityStateChanged(AbilityState newState)
        {
            _currentState = newState;
            OnAbilityStateChanged?.Invoke(_currentState);

            switch (newState)
            {
                case AbilityState.Active:
                    PlayAbilityAnimation(_currentAbility);
                    break;
            }
        }
        private void PlayAbilityAnimation(string abilityId)
        {
            var animationId = _abilities[abilityId].AbilityData.AnimationId;
            if (_tweener != null && animationId != null)
            {
                _tweener.SetAnimation(animationId, true);
            }
        }
    }
}
