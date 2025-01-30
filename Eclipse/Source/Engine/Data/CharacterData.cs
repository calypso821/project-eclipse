using System.Collections.Generic;
using Eclipse.Engine.Config;

namespace Eclipse.Engine.Data
{
    public class CharacterData : ObjectData
    {
        public int Health { get; }
        public int Defense { get; }
        public int Attack { get; }
        public float Speed { get; }

        public List<string> Abilities { get; }

        public CharacterData(string id, Config.CharacterData config)
        {
            Id = id;
            Health = config.Health;
            Defense = config.Defense;
            Attack = config.Attack;
            Speed = config.Speed;
            Abilities = config.Abilities;
        }
    }

    public class PlayerData : CharacterData
    {

        public PlayerData(string id, PlayerConfig config)
            : base(id, config)  // Pass to parent constructor
        {
        }
    }

    public class EnemyData : CharacterData
    {
        public bool IsBoss { get; }
        public float DetectionRange { get; }
        public float LoseTargetRange { get; }
        public float PreferredRange { get; }

        public EnemyData(string id, EnemyConfig config)
            : base(id, config)
        {
            IsBoss = config.IsBoss;
            DetectionRange = config.DetectionRange;
            LoseTargetRange = config.LoseTargetRange;
            PreferredRange = config.PreferredRange;
        }
    }

    public class NpcData : CharacterData
    {
        public NpcData(string id, NpcConfig config)
            : base(id, config)
        {
            // Initialize NPC specific properties when you add them
            //DialogueId = config.DialogueId;
            //IsQuestGiver = config.IsQuestGiver;
        }
    }

}

