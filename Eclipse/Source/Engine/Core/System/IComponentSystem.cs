namespace Eclipse.Engine.Core
{
    internal interface IComponentSystem : ISystem
    {
        void Register(GameObject gameObject);
        void Unregister(GameObject gameObject);
        void Cleanup(); // Runtime cleanup (drity components)
        void Clear(); // System reset (clear all components)
    }

}
