using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;

using Eclipse.Engine.Core;
using Eclipse.Components.Combat;
using Eclipse.Components.Engine;
using Eclipse.Engine.Config;
using Eclipse.Components.Animation;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Data;
using System;

namespace Eclipse.Engine.Factories
{
    public class WeaponFactory : Singleton<WeaponFactory>
    {
        private readonly Dictionary<string, WeaponConfig> _weaponConfigs;

        public WeaponFactory()
        {
            // Loading the config
            string jsonPath = "Assets/Config/Objects/Weapons.json";
            string jsonContent = File.ReadAllText(jsonPath);
            _weaponConfigs = JsonConvert.DeserializeObject<Dictionary<string, WeaponConfig>>(jsonContent);
        }
        public GameObject CreateWeapon(string id, Element element, Vector2 offset = default)
        {
            // Get entire config based on ID
            var config = _weaponConfigs[id];

            var obj = new GameObject(id);
            var data = new WeaponData(id, config);

            // Sprite
            var sprite = string.IsNullOrEmpty(config.SpriteId) ?
               new Sprite() : // Blank sprite for animator
               new Sprite(AssetManager.Instance.GetSprite(config.SpriteId));
            obj.AddComponent(sprite);

            // Setup SpirteAnimations
            var spriteAnimations = config.GetSpriteAnimations();
            if (spriteAnimations.Count > 0)
            {
                var animator = new SpriteAnimator(sprite);
                animator.SetAnimations(spriteAnimations);
                obj.AddComponent(animator);
            }

            // Setup TranfromAnimations
            var transfromAnimations = config.GetTransformAnimations();
            var tweener = new Tweener(obj.Transform);
            tweener.SetAnimations(transfromAnimations);
            obj.AddComponent(tweener);
            
            // Element state
            var elementState = new ElementState(element, false);
            obj.AddComponent(elementState);

            // Setup audio
            var audioSource = new SoundEffectSource();
            audioSource.Volume = 0.3f;
            audioSource.Is3D = false;
            obj.AddComponent(audioSource);

            Weapon weapon = data.WeaponType switch
            {
                WeaponType.Melee => new MeleeWeapon(data),
                WeaponType.Ranged => new RangedWeapon(data, sprite, offset),
                _ => throw new ArgumentException($"Weapon type {data.WeaponType} not supported in factory")
            };

            obj.AddComponent(weapon);

            // Set transform
            obj.Transform.SetTransform(
                config.Offset.ToVector2(),
                config.Rotation,
                config.Scale.ToVector2()
            );

            return obj;
        }
    }

}
