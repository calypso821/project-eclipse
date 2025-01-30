
using Eclipse.Engine.Scenes;

namespace Eclipse.Engine.Core
{
    public interface IPoolableFactory
    {
        void InitializeObjects(Scene targetScene);
        GameObject Get(string id);
        void Return(GameObject obj);
        void Dispose();
    }
}
