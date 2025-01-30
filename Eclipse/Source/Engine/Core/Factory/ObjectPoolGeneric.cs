using System;
using System.Collections.Generic;


namespace Eclipse.Engine.Core
{
    public class ObjectPoolGeneric<T>
    {
        // Stack to store inactive/available objects
        private readonly Stack<T> _pool;

        // Functions passed in constructor:
        private readonly Func<T> _createFunc;         // How to create new object
        private readonly Action<T> _actionOnGet;      // What to do when object is taken from pool
        private readonly Action<T> _actionOnRelease;  // What to do when object is returned to pool
        private readonly Action<T> _actionOnDestroy;  // What to do when object is destroyed

        public ObjectPoolGeneric(
            Func<T> createFunc,
            Action<T> actionOnGet,
            Action<T> actionOnRelease,
            Action<T> actionOnDestroy,
            int defaultCapacity = 10)
        {
            _pool = new Stack<T>(defaultCapacity);
            _createFunc = createFunc;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;

            // Pre-populate pool
            for (int i = 0; i < defaultCapacity; i++)
            {
                // Create object + add to scene
                var obj = _createFunc();
                // Disable (deactivate)
                _actionOnRelease(obj);
                // Add to pool
                _pool.Push(obj);
            }
        }
            
        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : _createFunc();
            // If pool has objects - take one
            // If pool empty - create new object using createFunc

            _actionOnGet(item); // Activate/initialize object
            return item;
        }

        public void Release(T item)
        {
            _actionOnRelease(item);  // Deactivate/reset object
            _pool.Push(item);        // Put back in pool for reuse
        }

        public void Destroy()
        {
            while (_pool.Count > 0)
            {
                var item = _pool.Pop();
                _actionOnDestroy(item);
            }
            _pool.Clear();
        }
    }
}
