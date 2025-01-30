using System.Collections.Generic;

using Eclipse.Engine.Core;
using Eclipse.Engine.Factories;
using Eclipse.Engine.Scenes;

namespace Eclipse.Engine.Managers
{
    public class FactoryManager : Singleton<FactoryManager>
    {
        // Poolable factories that need initialization
        private readonly List<IPoolableFactory> _poolableFactories = new();

        public FactoryManager()
        {
            // Regular factories
            WeaponFactory.CreateInstance();
            AbilityFactory.CreateInstance();
            PhysicsFactory.CreateInstance();


            // Poolable factories
            var enemyFactory = new EnemyFactory();
            EnemyFactory.Initialize(enemyFactory);
            _poolableFactories.Add(enemyFactory);

            var projectileFactory = new ProjectileFactory();
            ProjectileFactory.Initialize(projectileFactory);
            _poolableFactories.Add(projectileFactory);
        }

        internal void InitializeFactories(Scene targetScene)
        {
            // Only poolable factories need initialization
            foreach (var factory in _poolableFactories)
            {
                factory.InitializeObjects(targetScene);
            }
        }
    }
}
