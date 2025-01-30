using Microsoft.Xna.Framework;
using System;
using System.IO;
using Newtonsoft.Json;

using Eclipse.Engine.Core;
using static System.Net.Mime.MediaTypeNames;
using Eclipse.Engine.Data;

namespace Eclipse.Engine.Managers
{
    public class SettingsData
    {
        public int MasterVolume { get; set; } = 100;
        public int MusicVolume { get; set; } = 100;
        public int SfxVolume { get; set; } = 100;
        public bool IsFullscreen { get; set; } = false;
        public int ScreenWidth { get; set; } = 1920;
        public int ScreenHeight { get; set; } = 1080;
    }

    //Common file names:
    //settings.json
    //config.json

    //Common locations:
    //Windows: %AppData%/[YourGameName]/
    //macOS: ~/Library/Application Support/[YourGameName]/
    //Linux: ~/.config/[YourGameName]/

    public class SettingsManager : Singleton<SettingsManager>
    {
        internal SettingsData CurrentSettings { get; private set; }

        private readonly string _settingsPath;
        private Game _game;

        public SettingsManager()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gameFolder = Path.Combine(appData, "TwoDDTwo");
            Directory.CreateDirectory(gameFolder);
            _settingsPath = Path.Combine(gameFolder, "settings.json");

            LoadSettings();
        }

        internal void Initialize(Game game)
        {
            _game = game;
            ApplySettings();
        }

        internal void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    CurrentSettings = JsonConvert.DeserializeObject<SettingsData>(json);
                }
                else
                {
                    CurrentSettings = new SettingsData();
                    SaveSettings();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading settings: {e.Message}");
                CurrentSettings = new SettingsData();
            }
        }

        internal void SaveSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(CurrentSettings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving settings: {e.Message}");
            }
        }

        private void ApplySettings()
        {
            // Set audio settings

            SetMasterVolume(CurrentSettings.MasterVolume);
            SetMusicVolume(CurrentSettings.MusicVolume);

            // Set video settings 
            //if (_game?.Window == null) return;

            //// Apply resolution and fullscreen
            //if (CurrentSettings.IsFullscreen != _game.Window.IsBorderless)
            //{
            //    _game.Window.IsBorderless = CurrentSettings.IsFullscreen;
            //}

            //if (_game.Window.ClientBounds.Width != CurrentSettings.ScreenWidth ||
            //    _game.Window.ClientBounds.Height != CurrentSettings.ScreenHeight)
            //{
            //    _game.Window.ClientBounds = new Rectangle(
            //        _game.Window.ClientBounds.X,
            //        _game.Window.ClientBounds.Y,
            //        CurrentSettings.ScreenWidth,
            //        CurrentSettings.ScreenHeight
            //    );
            //}
        }

        internal void SetMasterVolume(int volume)
        {
            volume = Math.Clamp(volume, 0, 100);
            AudioManager.Instance.SetMasterVolume(volume / 100f);
            CurrentSettings.MasterVolume = volume;
            SaveSettings();
        }

        internal void SetMusicVolume(int volume)
        {
            volume = Math.Clamp(volume, 0, 100);
            AudioManager.Instance.SetMusicVolume(volume / 100f);
            CurrentSettings.MusicVolume = volume;
            SaveSettings();
            
        }

        internal void SetSfxVolume(int volume)
        {
            CurrentSettings.SfxVolume = volume;
            SaveSettings();
        }

        internal void SetResolution(int width, int height)
        {
            CurrentSettings.ScreenWidth = width;
            CurrentSettings.ScreenHeight = height;
            SaveSettings();
            ApplySettings();
        }

        internal void SetFullscreen(bool fullscreen)
        {
            CurrentSettings.IsFullscreen = fullscreen;
            SaveSettings();
            ApplySettings();
        }
    }
}
