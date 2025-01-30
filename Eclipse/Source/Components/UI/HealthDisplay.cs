using System.Collections.Generic;

using Eclipse.Components.Engine;
using Eclipse.Components.Controller;
using Eclipse.Engine.Core;

namespace Eclipse.Components.UI
{
    internal class HealthDisplay : UIWidget
    {
        private List<UIImage> _heartImages;
        private Stats _playerStats; // Reference to player stats

        internal HealthDisplay(List<UIImage> heartImages)
        {
            _heartImages = heartImages;
        }

        internal void Configure(Stats playerStats)
        {
            _playerStats = playerStats;

            // Initial values
            var healthStatus = playerStats.GetHealthStatus();
            UpdateHealthDisplay(healthStatus.currentHealth, healthStatus.maxHealth);

            // Subscribe to health changes
            _playerStats.OnHealthChanged += UpdateHealthDisplay;
            _playerStats.OnDeath += HandlePlayerDeath;
        }

        private void UpdateHealthDisplay(float currentHealth, float maxHealth)
        {
            // Update heart images based on current health
            var delta = maxHealth / _heartImages.Count;

            for (int i = 0; i < _heartImages.Count; i++)
            {
                if (delta * i > currentHealth)
                {
                    _heartImages[i].Disable();
                }
            }
        }

        private void HandlePlayerDeath()
        {
            // Handle death UI updates
        }

        internal override void OnDestroy()
        {
            // Important: Unsubscribe when UI is destroyed
            if (_playerStats != null)
            {
                _playerStats.OnHealthChanged -= UpdateHealthDisplay;
                _playerStats.OnDeath -= HandlePlayerDeath;
            }
            base.OnDestroy();
        }
    }
}
