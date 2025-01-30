using Microsoft.Xna.Framework;

using Eclipse.Engine.Core;

namespace Eclipse.Engine.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private Game _game;

        public GameManager()
        {
        }

        internal void Initialize(Game game)
        {
            _game = game;
        }
        internal void StartGame()
        {
            SceneManager.Instance.LoadScene("Forest");
            UIManager.Instance.LoadCanvas("HUD");
        }

        internal void ShowSettings()
        {
            UIManager.Instance.LoadCanvas("Settings");
        }

        internal void ShowHelp()
        {
            UIManager.Instance.LoadCanvas("Help");
        }
        internal void QuitGame()
        {
            // Add any cleanup needed before quitting
            _game.Exit(); 
        }
    }
}
