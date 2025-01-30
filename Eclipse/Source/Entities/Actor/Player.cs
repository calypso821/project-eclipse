using System;

using Eclipse.Engine.Data;
using Eclipse.Components.Controller;
using Eclipse.Engine.Core;

namespace Eclipse.Entities.Characters
{

    internal class Player : Character
    {
        private PlayerController _playerController;
        internal override bool FlipWithAim { get; set; } = true;

        internal Player(PlayerData data, string name = "Player") 
            : base(data, name)
        {
        }

        internal override void Start()
        {
            // Actor start
            base.Start();

            // Required component validation 
            _playerController = GetComponent<PlayerController>() ??
                throw new ArgumentException("Player actor requires PlayerController component");
        }
    }
}
