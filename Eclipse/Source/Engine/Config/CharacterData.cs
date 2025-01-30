
using System.Collections.Generic;

namespace Eclipse.Engine.Config
{

    public class CharacterData : ObjectConfig
    {
        public int Health { get; set; } = 100;
        public int Defense { get; set; } = 50;
        public int Attack { get; set; } = 10;
        public float Speed { get; set; } = 8.0f;
        public List<string> Abilities { get; set; } = new();
    }

    public class PlayerConfig : CharacterData
    {
        // Other player-specific config properties
        //public float StaminaMax { get; set; }
        //public float ManaMax { get; set; }
    }

    public class EnemyConfig : CharacterData
    {
        public bool IsBoss { get; set; } = false;
        public float DetectionRange { get; set; } = 10f;
        public float LoseTargetRange { get; set; } = 12f;
        public float PreferredRange { get; set; } = 3.0f;
    }

    public class NpcConfig : CharacterData
    {
        //public string DialogueId { get; set; }
        //public string IsQuestGiver { get; set; }
    }
}
