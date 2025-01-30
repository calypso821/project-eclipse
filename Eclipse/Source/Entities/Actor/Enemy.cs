
using System;

using Eclipse.Components.Controller;
using Eclipse.Engine.Core;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Data;

namespace Eclipse.Entities.Characters
{
    internal class Enemy : Character
    {
        private AIController _aiController;
        internal override bool FlipWithAim { get; set; } = true;

        internal Enemy(EnemyData data, string name = "Enemy")
            : base(data, name)
        {
        }

        internal override void Start()
        {
            // Actor start
            base.Start();

            // Required component validation 
            _aiController = GetComponent<AIController>() ??
                throw new ArgumentException("Enemy actor requires AIController component");
        }

        internal float GetDistanceToPlayer()
        {
            return PlayerManager.Instance.GetDistanceToPlayer(Transform.Position);
        }
    }
}
