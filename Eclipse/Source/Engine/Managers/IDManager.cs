
using System.Collections.Generic;
using System;

namespace Eclipse.Engine.Managers
{
    internal static class IDManager
    {
        private static int _nextId = 0;
        private static Queue<int> _releasedIds = new Queue<int>();

        internal static int GetId()
        {
            if (_releasedIds.Count > 0)
            {
                return _releasedIds.Dequeue();
            }

            if (_nextId >= int.MaxValue - 1000)
            {
                throw new InvalidOperationException("ID pool exhausted");
            }
            return _nextId++;
        }

        internal static void ReleaseId(int id)
        {
            _releasedIds.Enqueue(id);
        }
    }
}
