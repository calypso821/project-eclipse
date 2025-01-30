
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;

using Eclipse.Engine.Core;
using Eclipse.Engine.Config;
using Eclipse.Engine.Scenes;
using Eclipse.Engine.Managers;
using Eclipse.Entities.Characters;
using Eclipse.Components.Animation;
using Eclipse.Components.Controller;
using Eclipse.Components.Engine;
using Eclipse.Engine.Data;
using Eclipse.Components.AI;
using Eclipse.Components.Combat;

namespace Eclipse.Engine.Factories
{
    enum EnemyType
    {
        Slime
    }
    public class EnemyFactory : PoolableFactory<EnemyFactory>
    {
        // Separate pool per projectile type
        private readonly Dictionary<string, ObjectPool> _enemyPools;
        private readonly Dictionary<string, EnemyConfig> _enemyConfigs;

        public EnemyFactory()
        {
            _enemyPools = new Dictionary<string, ObjectPool>();

            // Loading the config
            string jsonPath = "Assets/Config/Characters/Enemies.json";
            string jsonContent = File.ReadAllText(jsonPath);

            _enemyConfigs = JsonConvert.DeserializeObject<Dictionary<string, EnemyConfig>>(jsonContent);
        }

        public override void InitializeObjects(Scene targetScene)
        {
            // Set target scene and clear pools
            base.InitializeObjects(targetScene);

            // Initialize each pool with specific capacity
            InitializePool("Slime/Green", 5);
            InitializePool("Slime/Orange", 5);
        }

        internal GameObject SpawnEnemy(string id, Vector2 position, Element elemente)
        {
            var enemy = Get(id);

            enemy.Transform.Position = position;
            enemy.GetComponent<ElementState>().SetState(elemente);

            return enemy;
        }
        internal void DespawnEnemy(GameObject enemy)
        {
            Return(enemy);
        }

        internal override GameObject Instantiate(string id)
        {
            var config = _enemyConfigs[id];
            var data = new EnemyData(id, config);
            var obj = new Enemy(data, id);

            obj.AddComponent(new Poolable(id));

            // Add components

            // Sprite
            var sprite = string.IsNullOrEmpty(config.SpriteId) ?
               new Sprite() : // Blank sprite for animator
               new Sprite(AssetManager.Instance.GetSprite(config.SpriteId));
            obj.AddComponent(sprite);

            // Setup SpirteAnimations
            // Base Animations
            //var baseAnimator = new SpriteAnimator(sprite);
            //var spriteAnimations = config.GetSpriteAnimations();
            //baseAnimator.SetAnimations(spriteAnimations);
            //obj.AddComponent(baseAnimator);

            //// Overlay animations
            //var overlaySprite = new OverlaySprite();
            //obj.AddComponent(overlaySprite);
            //var overlayAnimator = new OverlayAnimator(overlaySprite);
            //var overlayAnimations = config.GetOverlayAnimations();
            //overlayAnimator.SetAnimations(overlayAnimations);
            //obj.AddComponent(overlayAnimator);

            //obj.SetAnimators(baseAnimator, overlayAnimator);

            // Setup TranfromAnimations
            var transfromAnimations = config.GetTransformAnimations();
            var tweener = new Tweener(obj.Transform);
            tweener.SetAnimations(transfromAnimations);
            obj.AddComponent(tweener);

            var rigidBody = new RigidBody2D();
            rigidBody.MaxHorizontalSpeed = data.Speed;
            // TODO: SetPhysicsProperties
            obj.AddComponent(rigidBody);

            var spriteAssetBox = AssetManager.Instance.GetSprite(config.SpriteId);
            var collider = new BoxCollider2D(spriteAssetBox.GetHalfSizeInUnits());
            obj.AddComponent(collider);

            // Element state
            var elementState = new ElementState(Element.Neutral, false);
            obj.AddComponent(elementState);

            obj.AddComponent(new TargetDetector());
            obj.AddComponent(new CharacterController());
            var abilityController = new AbilityController();
            obj.AddComponent(abilityController);
            obj.AddComponent(new AIController(data));

            // Abilities
            foreach (var abilityId in data.Abilities)
            {
                var ability = AbilityFactory.Instance.CreateAbility(abilityId);
                obj.AddChild(ability);
                abilityController.AddAbility(abilityId, ability.GetComponent<Ability>());
            }

            obj.Transform.SetTransform(
                config.Offset.ToVector2(),
                config.Rotation,
                config.Scale.ToVector2()
            );

            // Add to scene
            TargetScene.AddObject(obj);

            return obj;
        }
    }
}