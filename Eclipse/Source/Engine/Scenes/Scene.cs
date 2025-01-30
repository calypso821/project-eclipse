using Eclipse.Engine.Core;

namespace Eclipse.Engine.Scenes
{
    public class Scene
    {
        private readonly SceneGraph _sceneGraph;

        //private readonly SceneData _sceneData;
        //private readonly Dictionary<string, GameObject> _spawnPoints = new();

        internal string Name { get; }
        internal GameObject Player { get; set; }
        internal SceneGraph SceneGraph => _sceneGraph;

        internal Scene(string name) // SceneData sceneData
        {
            Name = name;
            //_sceneData = sceneData;
            _sceneGraph = new SceneGraph();
        }

        internal void AddObject(GameObject gameObject)
        {
            _sceneGraph.AddNode(gameObject);
        }

        internal void Unload()
        {
            // Factroy.Destryo()???

            // Destry all objects + OnDestry() all components
            // Clear all componts from systems
            _sceneGraph.Clear();

            // Clear any direct scene references
            Player = null;
        }
    }
}