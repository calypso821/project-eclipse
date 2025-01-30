using System;
using System.Collections.Generic;
using Eclipse.Engine.Managers;

namespace Eclipse.Engine.Core
{
    internal class ObjectPool
    {
        private readonly Stack<GameObject> _pool;
        private readonly Func<GameObject> _createFunc;

        internal ObjectPool(
            Func<GameObject> createFunc,
            int defaultCapacity = 5)
        {
            _pool = new Stack<GameObject>(defaultCapacity);
            _createFunc = createFunc;

            // Pre-populate pool
            for (int i = 0; i < defaultCapacity; i++)
            {
                var item = _createFunc();

                var poolable = item.GetComponent<Poolable>();
                if (poolable == null)
                    throw new ArgumentException($"{item.Name} in ObjectPool requires Poolable component");

                poolable.OnDespawn(); // Reset + Disable
                _pool.Push(item);
            }
        }

        internal GameObject Get()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : _createFunc();

            item.GetComponent<Poolable>().OnSpawn();
            return item;
        }

        internal void Release(GameObject item)
        {
            item.GetComponent<Poolable>().OnDespawn();
            _pool.Push(item);
        }

        internal void Destroy()
        {
            while (_pool.Count > 0)
            {
                var item = _pool.Pop();
                // Add to DestroySystem()
                item.MarkForDestroy();
            }
            _pool.Clear();
        }
    }
}
