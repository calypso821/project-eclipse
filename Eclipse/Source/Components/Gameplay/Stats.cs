using System;
using System.Collections.Generic;

using Eclipse.Engine.Core;
using Eclipse.Engine.Data;

namespace Eclipse.Components.Engine
{
    internal class StatModifier
    {
        internal float HealthMod { get; set; }
        internal float AttackMod { get; set; }
        internal float DefenseMod { get; set; }
        internal float SpeedMod { get; set; }
        internal float Duration { get; set; } // Handled by life cycle system 
    }

    internal class Stats : Component
    {
        // Event for death
        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;

        // Max health (only health needs max)
        private float _maxHealth;
        // Current health
        private float _currentHealth;

        // Other stats (only need one value)
        private float _attack;
        private float _defense;
        private float _speed;

        private Dictionary<string, StatModifier> _modifiers = new Dictionary<string, StatModifier>();

        // Getters
        internal float CurrentHealth => _currentHealth;
        internal float MaxHealth => _maxHealth;
        internal float Attack => _attack;
        internal float Defense => _defense;
        internal float Speed => _speed;

        // Health methods
        internal bool IsDead => _currentHealth <= 0;
        internal bool IsFullHealth => _currentHealth >= _maxHealth;
        internal float HealthPercentage => _currentHealth / _maxHealth;

        internal Stats(CharacterData data)
            : base()
        {
            InitializeStats(data);
        }

        private void InitializeStats(CharacterData data)
        {
            _maxHealth = data.Health;
            _defense = data.Defense;
            _speed = data.Speed;

            if (data is EnemyData enemyData)
            {
                _attack = enemyData.Attack;
            }
            _currentHealth = _maxHealth;
        }

        internal override void OnReset()
        {
            // Clear all modifiers
            _modifiers.Clear();

            // Reset current health to max
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        internal void TakeDamage(float amount)
        {
            _currentHealth = Math.Max(0, _currentHealth - amount);

            // Notify UI and other listeners about health change
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (IsDead)
            {
                // Trigger the event
                OnDeath?.Invoke();
            }
        }

        internal void Heal(float amount)
        {
            _currentHealth = Math.Min(_maxHealth, _currentHealth + amount);

            // Notify UI and other listeners about health change
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        internal (float currentHealth, float maxHealth) GetHealthStatus()
        {
            return (_currentHealth, _maxHealth);
        }

        // Setter methods
        internal void SetAttack(float value)
        {
            _attack = Math.Max(0, value);
            RecalculateStats();
        }

        internal void SetDefense(float value)
        {
            _defense = Math.Max(0, value);
            RecalculateStats();
        }

        internal void SetSpeed(float value)
        {
            _speed = Math.Max(0, value);
            RecalculateStats();
        }

        internal void SetMaxHealth(float value)
        {
            _maxHealth = Math.Max(0, value);
        }

        // Modifiers
        internal void AddModifier(string id, StatModifier modifier)
        {
            _modifiers[id] = modifier;
            RecalculateStats();
        }

        internal void RemoveModifier(string id)
        {
            if (_modifiers.ContainsKey(id))
            {
                _modifiers.Remove(id);
                RecalculateStats();
            }
        }

        private void RecalculateStats()
        {

            foreach (var modifier in _modifiers.Values)
            {
                _currentHealth *= (1 + modifier.HealthMod);
                _attack *= (1 + modifier.AttackMod);
                _defense *= (1 + modifier.DefenseMod);
                _speed *= (1 + modifier.SpeedMod);
            }
        }
    }
}