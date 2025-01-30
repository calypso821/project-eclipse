using System;
using System.Collections.Generic;

using Eclipse.Engine.Scenes;

namespace Eclipse.Engine.Core
{
    public abstract class PoolableFactory<T> : IPoolableFactory where T : PoolableFactory<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception($"{typeof(T)} instance not initialized");
                return _instance;
            }
        }

        public static void Initialize(T instance)
        {
            if (_instance != null)
                throw new Exception($"{typeof(T)} instance already initialized");
            _instance = instance;
        }

        internal readonly Dictionary<string, ObjectPool> _pools;
        internal Scene TargetScene { get; set; }

        internal PoolableFactory()
        {
            _pools = new Dictionary<string, ObjectPool>();
        }

        public virtual void InitializeObjects(Scene targetScene)
        {
            TargetScene = targetScene;

            // Clear all existing pools
            Dispose();
        }

        internal virtual void InitializePool(string id, int capacity)
        {
            if (!_pools.ContainsKey(id))
            {
                _pools[id] = new ObjectPool(
                    createFunc: () => Instantiate(id),
                    defaultCapacity: capacity
                );
            }
        }
        internal abstract GameObject Instantiate(string id);

        public virtual GameObject Get(string id)
        {
            var pool = _pools[id];
            var obj = pool.Get();
            return obj;
        }

        public virtual void Return(GameObject obj)
        {
            var poolId = obj.GetComponent<Poolable>().PoolId;
            var pool = _pools[poolId];
            pool.Release(obj);
        }

        public virtual void Dispose()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Destroy();
            }
            _pools.Clear();
        }
    }
}
