using System.Collections.Generic;
using System;

using Microsoft.Xna.Framework;

using Eclipse.Components.UI;
using Eclipse.Engine.Core;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.UI;
using Eclipse.Components.Engine;
using Eclipse.Components.Controller;
using Eclipse.Engine.Systems;


namespace Eclipse.Engine.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<string, UICanvas> _canvases = new();
        private UICanvas _mainCanvas;  // Main/HUD canvas
        private Stack<UICanvas> _activeCanvases = new();  // For managing UI stack (menus, popups)

        private UICanvas _activeCanvas;

        // For DebugCanvas
        internal UICanvas ActiveCanvas => _activeCanvas;

        private GameManager _gameManager;

        // TODO: UIELementSYstems (SYstemMAnager) - system specific flags
        private CanvasRenderer _canvasRenderer;
        private UISystem _uiSystem;

        public UIManager()
        {
        }

        internal void Initialize(CanvasRenderer canvasRenderer, UISystem uiSystem)
        {
            _canvasRenderer = canvasRenderer;
            _uiSystem = uiSystem;
            _gameManager = GameManager.Instance;
        }

        // TODO: UIFactory
        // TODO: Loading from Tiled??
        //Define UI layouts in separate files/scenes("HUD.tmx", "MainMenu.tmx")
        //Load them as UI scenes/canvases

        public void LoadCanvas(string canvasName)
        {
            // Unload active canvas
            // Destry all objects + clear all elemnts from UI systems
            UnloadCanvas();

            var canvas = new UICanvas(_canvasRenderer, _uiSystem);

            switch (canvasName)
            {
                case "MainMenu":
                    CreateMainMenuCanvas(canvas);
                    break;
                case "HUD":
                    CreateHUDCanvas(canvas);
                    break;
                case "Help":
                    CreateHelpCanvas(canvas);
                    break;
                case "Settings":
                    CreateSettingsCanvas(canvas);
                    break;
            }
            _activeCanvas = canvas;
        }
        private void CreateMainMenuCanvas(UICanvas canvas)
        {
            // Title
            var titleObject = new UIObject();
            titleObject.Transform.Position = new Vector2(800, 200);

            var fontAsset = AssetManager.Instance.GetFont("ThaleahFat");
            var titleText = new UIText(fontAsset);
            titleText.Text = "Eclipse";
            titleText.Size = 2.0f;
            titleObject.AddComponent(titleText);
            canvas.AddObject(titleObject);

            // Play Button
            var playButton = CreateMenuButton("Play", new Vector2(800, 400),
                _gameManager.StartGame);
            canvas.AddObject(playButton);

            // Settings Button
            var settingsButton = CreateMenuButton("Settings", new Vector2(800, 500),
                _gameManager.ShowSettings);
            canvas.AddObject(settingsButton);

            // Help Button
            var helpButton = CreateMenuButton("Help", new Vector2(800, 600),
                _gameManager.ShowHelp);
            canvas.AddObject(helpButton);

            // Quit Button
            var quitButton = CreateMenuButton("Quit", new Vector2(800, 700),
                _gameManager.QuitGame);
            canvas.AddObject(quitButton);

            // Optional: Add apply button
            //var applyButton = CreateMenuButton("Apply", new Vector2(800, 350), () =>
            //{
            //    var input = textInput.GetComponent<UITextInput>();
            //    if (input != null)
            //    {
            //        Console.WriteLine($"Applied text: {input.Text}");
            //    }
            //});
            //canvas.AddObject(applyButton);
        }

        private UIObject CreateMenuButton(string text, Vector2 position, Action onClick)
        {
            var button = new UIObject();

            var buttonSprite = AssetManager.Instance.GetSprite("UI/Frames/FlatFrameBlue");
            var buttonImage = new UIImage(buttonSprite);
            button.AddComponent(buttonImage);

            button.Transform.Position = position;
            button.Transform.Size = buttonImage.Size; // button size -> Image size
            button.Transform.Scale = new Vector2(2.0f, 1f);

            var fontAsset = AssetManager.Instance.GetFont("ThaleahFat");
            var buttonText = new UIText(fontAsset);
            buttonText.Text = text;
            buttonText.Alignment = TextAlignment.Center;
            button.AddComponent(buttonText);

            var buttonComponent = new UIButton();
            buttonComponent.OnClick += onClick;
            button.AddComponent(buttonComponent);

            return button;
        }

        private UIObject CreateTextInput(
            string placeholder, string initialValue, 
            Vector2 position, Action<string> onSubmit)
        {
            var textInputObj = new UIObject();

            // Background frame
            var frameSprite = AssetManager.Instance.GetSprite("UI/Frames/FlatFrameGrey");
            var background = new UIImage(frameSprite);
            textInputObj.AddComponent(background);
            textInputObj.Transform.Position = position;
            textInputObj.Transform.Size = background.Size;
            textInputObj.Transform.Scale = new Vector2(0.65f, 0.5f);

            // Text display
            var fontAsset = AssetManager.Instance.GetFont("ThaleahFat");
            var textDisplay = new UIText(fontAsset);
            textDisplay.Size = 0.6f;
            textDisplay.Alignment = TextAlignment.Center;
            textInputObj.AddComponent(textDisplay);

            // TextInput component
            var textInput = new UITextInput(initialValue);
            textInput.Placeholder = placeholder;
            textInput.OnSubmit += (text) => Console.WriteLine($"Text submitted: {text}");
            textInput.OnSubmit += onSubmit;
            textInput.OnTextChanged += (text) => Console.WriteLine($"Text changed: {text}");
            textInputObj.AddComponent(textInput);

            return textInputObj;
        }
        private void CreateSettingsCanvas(UICanvas canvas)
        {
            // Master volume (slider)
            var masterVolume = new UIObject();
            masterVolume.Transform.Position = new Vector2(800, 450);

            var fontAsset = AssetManager.Instance.GetFont("ThaleahFat");
            var masterVolumeText = new UIText(fontAsset);
            masterVolumeText.Text = "Master volume:";
            masterVolume.AddComponent(masterVolumeText);
            canvas.AddObject(masterVolume);

            // Add TextInput
            var masterTextInput = CreateTextInput(
                "0-100",
                SettingsManager.Instance.CurrentSettings.MasterVolume.ToString(),
                new Vector2(1070, 440),
                (text) =>
                {
                    if (int.TryParse(text, out int volume))
                    {
                        SettingsManager.Instance.SetMasterVolume(volume);
                        Console.WriteLine($"Master volume set: {text}");
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number");
                    }
                });

            canvas.AddObject(masterTextInput);

            // TOOD: Slider here (Master volume) - UIWidget
            // AduioManager.Instance.MasterVolume()

            // Music volume (slider)
            var musicVolume = new UIObject();
            musicVolume.Transform.Position = new Vector2(800, 500);

            var musicVolumeText = new UIText(fontAsset);
            musicVolumeText.Text = "Music volume:";
            musicVolume.AddComponent(musicVolumeText);
            canvas.AddObject(musicVolume);

            var musicTextInput = CreateTextInput(
                "0-100",
                SettingsManager.Instance.CurrentSettings.MusicVolume.ToString(),
                new Vector2(1040, 490),
                (text) =>
                {
                    if (int.TryParse(text, out int volume))
                    {
                        SettingsManager.Instance.SetMusicVolume(volume);
                        Console.WriteLine($"Music volume set: {text}");
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number");
                    }
                });
            canvas.AddObject(musicTextInput);

            // TOOD: Slider here (Music volume) - UIWidget
            // AduioManager.Instance.MusicVolume()

            // Back Button
            var backButton = CreateMenuButton("Back", new Vector2(800, 650), () =>
            {
                LoadCanvas("MainMenu");
            });
            canvas.AddObject(backButton);
        }

        private void CreateHelpCanvas(UICanvas canvas)
        {
            // Background Panel
            var panel = new UIObject();
            panel.Transform.Position = new Vector2(800, 450);

            var fontAsset = AssetManager.Instance.GetFont("ThaleahFat");
            var helpText = new UIText(fontAsset);
            helpText.Text =
                "Move:    WASD\n" +
                "Jump:    Space\n" +
                "Weapon fire:    Left click\n" +
                "Weapon switch:    1, 2, 3";
            panel.AddComponent(helpText);

            // Back Button
            var backButton = CreateMenuButton("Back", new Vector2(800, 650), () =>
            {
                LoadCanvas("MainMenu");
            });
            canvas.AddObject(backButton);

            canvas.AddObject(panel);
        }

        private void CreateHUDCanvas(UICanvas canvas)
        {
            var player = PlayerManager.Instance.Player;
            // Get required components from player
            WeaponController weaponController = player.GetComponent<WeaponController>();
            Stats playerStats = player.GetComponent<Stats>();

            // Setup Ammo Display
            if (weaponController != null)
            {
               var ammoDisplay = CreateAmmoDisplay(weaponController);
                canvas.AddObject(ammoDisplay);
            }

            // Setup Health Display
            if (playerStats != null)
            {
                var healthDisplay = CreateHealthDisplay(playerStats);
                canvas.AddObject(healthDisplay);
            }
        }

        private UIObject CreateAmmoDisplay(WeaponController weaponController)
        {
            var uiObject = new UIObject();
            uiObject.Transform.Position = new Vector2(100, 950);

            var fontAsset = AssetManager.Instance.GetFont("ThaleahFat");
            var uiText = new UIText(fontAsset);
            ///uiText.Scale = 5.0f;
            uiObject.AddComponent(uiText);

            var ammoDisplay = new AmmoDisplay(uiText);
            uiObject.AddComponent(ammoDisplay);

            ammoDisplay.Configure(weaponController);

            return uiObject;
        }

        private UIObject CreateHealthDisplay(Stats playerStats)
        {
            var uiObject = new UIObject();
            uiObject.Transform.Position = new Vector2(100, 80);
            uiObject.Transform.Scale = new Vector2(0.25f, 0.25f);

            var _heartImages = new List<UIImage>();
            int numOfHearts = 4;

            for (int i = 0; i < numOfHearts; i++)
            {
                var heartImage = CreateHeartImage();
                heartImage.Offset = new Vector2(80 * i, 0);
                _heartImages.Add(heartImage);
                uiObject.AddComponent(heartImage);
            }

            var healthDisplay = new HealthDisplay(_heartImages);
            healthDisplay.Configure(playerStats);
            uiObject.AddComponent(healthDisplay);

            return uiObject;
        }

        private UIImage CreateHeartImage()
        {
            var spriteAsset = AssetManager.Instance.GetSprite("UI/Icons/HealthIcon");
            var uiElement = new UIImage(spriteAsset);
            return uiElement;
        }

        private void UnloadCanvas()
        {
            if (_activeCanvas != null)
            {
                // Destroy all objects and remove from systems
                _activeCanvas.Unload();
            }
        }

        // Optional: Method to clean up UI when scene changes
        //internal void CleanupUI()
        //{
        //    if (_ammoDisplay != null)
        //    {
        //        Destroy(_ammoDisplay.gameObject);
        //    }
        //    if (_healthDisplay != null)
        //    {
        //        Destroy(_healthDisplay.gameObject);
        //    }
        //}

        public void ShowUI(string canvasName)
        {
            // Show/stack specific UI canvas
        }

        public void HideUI(string canvasName)
        {
            // Hide/remove from stack
        }

    }
}
