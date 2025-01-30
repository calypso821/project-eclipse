using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Newtonsoft.Json;

using Eclipse.Engine.Scenes;
using Eclipse.Engine.Config;
using Eclipse.Components.Combat;
using Eclipse.Engine.Core;
using Eclipse.Components.Engine;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Data;

namespace Eclipse.Engine.Factories
{
    // Maybe abstract class (Get,Return, Initizlaie)
    public class ProjectileFactory : PoolableFactory<ProjectileFactory>
    {
        // Separate pool per projectile type
        private readonly Dictionary<string, ObjectPool> _projectilePools;
        private readonly Dictionary<string, ProjectileConfig> _projectileConfigs;

        public ProjectileFactory()
        {
            _projectilePools = new Dictionary<string, ObjectPool>();

            // Loading the config
            string jsonPath = "Assets/Config/Objects/Projectiles.json";
            string jsonContent = File.ReadAllText(jsonPath);
            _projectileConfigs = JsonConvert.DeserializeObject<Dictionary<string, ProjectileConfig>>(jsonContent);
        }
        public override void InitializeObjects(Scene targetScene)
        {
            // Set target scene and clear pools
            base.InitializeObjects(targetScene);

            // Initialize each pool with specific capacity
            InitializePool("RifleBullet", 20);
            InitializePool("SniperBullet", 10);
            InitializePool("Rocket", 5);
            InitializePool("Arrow", 10);
            InitializePool("SlimeProjectile", 10);
        }

        internal GameObject SpawnProjectile(string id, DamageData damageData)
        {
            var projectile = Get(id);  // Use base Get

            var projComp = projectile.GetComponent<Projectile>();
            projComp.Configure(damageData);

            return projectile;
        }

        internal void DespawnProjectile(GameObject projectile)
        {
            Return(projectile);
        }
        internal override GameObject Instantiate(string id)
        {
            var config = _projectileConfigs[id];
            var obj = new GameObject(id);

            // Poolable component
            obj.AddComponent(new Poolable(id));

            // Add components
            var spriteAsset = AssetManager.Instance.GetSprite(config.SpriteId);
            obj.AddComponent(new Sprite(spriteAsset));

            var rigidBody = new RigidBody2D();
            obj.AddComponent(rigidBody);

            var size = spriteAsset.GetHalfSizeInUnits();
            size.X /= 2;
           
            var collider = new BoxCollider2D(
                halfSize: size,
                offset: new Vector2(size.X / 2, 0)
            );
            collider.Layer = CollisionLayer.Projectile;
            obj.AddComponent(collider);

            var elementState = new ElementState(Element.Neutral, true);
            obj.AddComponent(elementState);

            // Setup audio
            var audioSource = new SoundEffectSource();
            audioSource.Volume = 0.3f;
            obj.AddComponent(audioSource);

            var data = new ProjectileData(id, config);
            var projectileComp = new Projectile(data);
            obj.AddComponent(projectileComp);

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