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
                animator.AddAnimations(spriteAnimations);
                obj.AddComponent(animator);
            }

            // Visaul Effects
            var visualEffects = config.GetVisualEffects();
            if (visualEffects.Count > 0)
            {
                var vfxSoruce = new VFXSource();
                vfxSoruce.AddEffects(visualEffects);
                obj.AddComponent(vfxSoruce);
            }

            // Setup TranfromAnimations
            //var transfromAnimations = config.GetTransformAnimations();
            //var tweener = new Tweener(obj.Transform);
            //tweener.AddAnimations(transfromAnimations);
            //obj.AddComponent(tweener);

            // Sound Effects
            var soundEffects = config.SoundEffects;
            if (soundEffects.Count > 0)
            {
                var audioSource = new SFXSource();
                audioSource.Is3D = false;
                audioSource.AddEffects(soundEffects);
                obj.AddComponent(audioSource);
            }

            // Element state
            var elementState = new ElementState(element, false);
            obj.AddComponent(elementState);

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
