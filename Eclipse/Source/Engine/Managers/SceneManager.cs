using System;

using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;
using Eclipse.Engine.Scenes;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.UI;

using Eclipse.Components.UI;

namespace Eclipse.Engine.Managers
{
    public class SceneManager : Singleton<SceneManager>
    {
        private readonly LevelLoader _levelLoader;

        // TODO: Preloaded scenes??
        //private readonly Dictionary<string, Scene> _scenes = new();

        private Scene _activeScene;

        internal Scene ActiveScene
        {
            get
            {
                if (_activeScene == null)
                    throw new InvalidOperationException("No active scene set");
                return _activeScene;
            }
        }

        public SceneManager()
        {
            _levelLoader = new LevelLoader();
        }

        internal void LoadScene(string sceneName)
        {
            var loadingScene = new Scene(sceneName);

            // Create pools of objects on new scene
            FactoryManager.Instance.InitializeFactories(loadingScene);

            // Load all content into loading scene
            _levelLoader.LoadLevel(sceneName, loadingScene);

            // Unload current scene if exists
            if (_activeScene != null)
            {
                _activeScene.Unload();
            }

            // Switch scenes
            _activeScene = loadingScene;
        }

        //internal void AddScene(Scene scene)
        //{
        //    _scenes[scene.Name] = scene;
        //}
    }
}
