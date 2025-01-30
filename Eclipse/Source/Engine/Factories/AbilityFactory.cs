using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Eclipse.Components.Combat;
using Eclipse.Engine.Config;
using Eclipse.Engine.Core;
using Eclipse.Engine.Data;

namespace Eclipse.Engine.Factories
{
    public class AbilityFactory : Singleton<AbilityFactory>
    {
        private readonly Dictionary<string, AbilityConfig> _abilityConfigs = new();

        public AbilityFactory()
        {
            // Loading the config
            string jsonPath = "Assets/Config/Abilities/AttackAbilities.json";
            string jsonContent = File.ReadAllText(jsonPath);
            var attackConfigs = JsonConvert.DeserializeObject<Dictionary<string, AbilityConfig>>(jsonContent);

            foreach (var kvp in attackConfigs)
            {
                _abilityConfigs.Add(kvp.Key, kvp.Value);
            }
        }
        public GameObject CreateAbility(string id)
        {
            // Get entire config based on ID
            var config = _abilityConfigs[id];

            var obj = new GameObject(id);

            var data = new AbilityData(id, config);

            // Setup SpirteAnimations
            var spriteAnimations = config.GetSpriteAnimations();
            data.SpriteAnimations = spriteAnimations ?? new();

            // Setup TranfromAnimations
            var transfromAnimations = config.GetTransformAnimations();
            data.TransformAnimations = transfromAnimations ?? new();

            Ability ability = data.AbilityType switch
            {
                AbilityType.Melee => new MeleeAbility(data),
                AbilityType.Projectile => new ProjectileAbility(data),
                _ => throw new ArgumentException($"Ability type {data.AbilityType} not supported in factory")
            };

            obj.AddComponent(ability);

            // Set transform
            //obj.Transform.SetTransform(
            //    config.Offset.ToVector2(),
            //    config.Rotation,
            //    config.Scale.ToVector2()
            //);

            return obj;
        }
    }

}
