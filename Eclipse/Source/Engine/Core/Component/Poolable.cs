namespace Eclipse.Engine.Core
{
    internal class Poolable : Component, IPoolable
    {
        public bool IsPooled { get; private set; } = true;
        public string PoolId { get; }

        internal Poolable(string poolId)
            : base()
        {
            PoolId = poolId;
        }

        public void OnSpawn() // Called by pool's actionOnGet
        {
            // Reset all components - OnReset()
            GameObject.Reset();

            // Clean object before use
            GameObject.ClearDirtyFlags();

            IsPooled = false;
            GameObject.SetActive(true);
        }

        public void OnDespawn() // Called by pool's actionOnRelease
        {
            IsPooled = true;
            GameObject.SetActive(false);
        }
    }
}
