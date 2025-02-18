// System
using System;

// Third-party
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// dotnet add package Newtonsoft.Json
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// Engine
using Eclipse.Engine.Systems;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.Systems.Physics;
using Eclipse.Engine.Systems.Input;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Debug;
using Eclipse.Engine.Utils.Load;
using Eclipse.Engine.Core;
using Eclipse.Engine.Cameras;
using Eclipse.Engine.Systems.Audio;

//#if DEBUG
// Import debug
//#endif

namespace Eclipse.Engine
{
    public class GameMain : Game
    {
        // Core
        private GraphicsDeviceManager _graphics { get; }
        private SpriteBatch _spriteBatch;

        // Factories
        //private WeaponFactory _weaponFactory;
        //private ProjectileFactory _projectileFactory;
        //private EnvironmentFactory _environmentFactory;
        //private EnemyFactory _enemyFactory;

        // Scene
        private SceneManager _sceneManager;
        private SystemManager _systemManager;
        private UIManager _uiManager;

        // Debug (If DEBUG)
        //private DebugRenderer2D _debugRenderer2D;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // ============ Graphics =========== 

            // Set your desired window size
            _graphics.PreferredBackBufferWidth = 1920;  // Window width
            _graphics.PreferredBackBufferHeight = 1080;  // Window height

            // Optional: Make the window resizable
            //Window.AllowUserResizing = true;

            // Optional: Start in fullscreen
            //_graphics.IsFullScreen = true;

            // Optional: Handle window resize events
            //Window.ClientSizeChanged += Window_ClientSizeChanged;

            // Apply the changes
            _graphics.ApplyChanges();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // ============ Managers ===========
            GameManager.CreateInstance();
            GameManager.Instance.Initialize(this);
            // Assets
            var assetLoader = new AssetLoader(Content);
            AssetManager.CreateInstance();
            AssetManager.Instance.Initialize(assetLoader);

            // Input
            var inputSystem = new InputSystem();
            InputManager.CreateInstance();
            InputManager.Instance.Initialize(inputSystem);

            // Audio
            AudioManager.CreateInstance();
            AudioManager.Instance.Initialize();

            // VFX
            VFXManager.CreateInstance();
            VFXManager.Instance.Initialize();

            // Camera
            var camera = new Camera(new Vector2(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight
            ));
            var cameraSystem = new CameraSystem(camera);
            CameraManager.CreateInstance();
            CameraManager.Instance.Initialize(cameraSystem);

            // Gameplay
            // TODO: GameplayManager.Initialize(new GameplayManager(InputManager.Instance));

            // Player
            PlayerManager.CreateInstance();

            // Factories (initilize pools later)
            FactoryManager.CreateInstance();

            // Scene
            SceneManager.CreateInstance();
            _sceneManager = SceneManager.Instance;

            // ============ Systems ===========
            SystemManager.CreateInstance();
            _systemManager = SystemManager.Instance;

            var spriteRenderer = new SpriteRenderer(_spriteBatch);
            var canvasRenderer = new CanvasRenderer(_spriteBatch);
            var vfxSystem = new VFXSystem(_spriteBatch);
            var physicsSystem = new PhysicsSystem();
            var transformSystem = new TransformSystem();
            var controllerSystem = new ControllerSystem();
            var animationSystem = new AnimationSystem();
            var projectileSystem = new ProjectileSystem();
            var abilitySystem = new AbilitySystem();
            var destroySystem = new DestroySystem();
            var audioSystem = new AudioSystem();
            var uiSystem = new UISystem();

            // If debug
            var debugRenderer = new DebugRenderer(_spriteBatch, physicsSystem);
            var debugCanvas = new DebugCanvas(_spriteBatch);

            // Pre update
            _systemManager.RegisterSystem(inputSystem, SystemGroup.PreUpdate, 0);
            _systemManager.RegisterSystem(audioSystem, SystemGroup.PreUpdate, 1);
            _systemManager.RegisterSystem(uiSystem, SystemGroup.PreUpdate, 2);
            _systemManager.RegisterSystem(abilitySystem, SystemGroup.PreUpdate, 3);
            _systemManager.RegisterSystem(projectileSystem, SystemGroup.PreUpdate, 4);
            _systemManager.RegisterSystem(controllerSystem, SystemGroup.PreUpdate, 5);
            _systemManager.RegisterSystem(transformSystem, SystemGroup.PreUpdate, 6);

            // Physics
            _systemManager.RegisterSystem(physicsSystem, SystemGroup.PhysicsUpdate, 0);
            _systemManager.RegisterSystem(transformSystem, SystemGroup.PhysicsUpdate, 1);

            // Post Update 
            _systemManager.RegisterSystem(animationSystem, SystemGroup.PostUpdate, 0);
            _systemManager.RegisterSystem(vfxSystem, SystemGroup.PostUpdate, 1);
            _systemManager.RegisterSystem(cameraSystem, SystemGroup.PostUpdate, 2);
            _systemManager.RegisterSystem(spriteRenderer, SystemGroup.PostUpdate, 3);
            _systemManager.RegisterSystem(canvasRenderer, SystemGroup.PostUpdate, 4);
            _systemManager.RegisterSystem(destroySystem, SystemGroup.PostUpdate, 5);

            // Drawable systems
            _systemManager.RegisterDrawableSystem(spriteRenderer, 0);
            _systemManager.RegisterDrawableSystem(vfxSystem, 1);
            _systemManager.RegisterDrawableSystem(canvasRenderer, 2);

            // Debug drawable
            //_systemManager.RegisterDrawableSystem(debugRenderer, 3);
            //_systemManager.RegisterDrawableSystem(debugCanvas, 4);

            UIManager.CreateInstance();
            UIManager.Instance.Initialize(canvasRenderer, uiSystem);

            _uiManager = UIManager.Instance;

            // Json deserializer settings
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            // Graphics setting shoud be here??
            SettingsManager.CreateInstance();
            SettingsManager.Instance.Initialize(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            AssetManager.Instance.LoadContent();

            //_sceneManager.LoadScene("Forest");
            _uiManager.LoadCanvas("MainMenu");
            ///GameManager.Instance.StartGame();

            AudioManager.Instance.PlayMusic("Soundtrack2", 1.0f, true);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
                
            // Update all systems by grpup and priority (slowdown)
            var scaledGameTime = new GameTime(
                gameTime.TotalGameTime,
                TimeSpan.FromTicks((long)(gameTime.ElapsedGameTime.Ticks * 0.2f))
            );

            // Update all systems by grpup and priority
            _systemManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(7, 94, 64, 255));
            //GraphicsDevice.Clear(Color.Black);

            // Update all systems by grpup and priority
            _systemManager.Draw();

            base.Draw(gameTime);
        }
    }
}
