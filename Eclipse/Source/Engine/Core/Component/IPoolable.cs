
namespace Eclipse.Engine.Core
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
        bool IsPooled { get; }
        string PoolId { get; }
    }
}
