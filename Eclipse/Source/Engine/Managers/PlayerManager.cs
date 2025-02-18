using System.IO;

using Newtonsoft.Json;
using Microsoft.Xna.Framework;

using Eclipse.Components.Controller;
using Eclipse.Engine.Config;
using Eclipse.Engine.Data;
using Eclipse.Engine.Core;
using Eclipse.Engine.Factories;
using Eclipse.Entities.Characters;
using Eclipse.Components.Animation;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Managers
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private Player _player;

        internal Player Player
        {
            get => _player;
            private set => _player = value;
        }
        public PlayerManager()
        {
            // Spawn Player -> in Level loader
        }

        internal Vector2 GetPlayerPosition()
        {
            if (_player == null) return Vector2.Zero;

            return _player.Transform.Position;
        }

        internal float GetDistanceToPlayer(Vector2 position)
        {
            if (_player == null) return float.MaxValue;

            return Vector2.Distance(position, _player.Transform.Position);
        }

        internal Player SpawnPlayer()
        {
            Element playerElement = Element.Dark;

            // ========= Player =========
            string jsonPath = "Assets/Config/Characters/Player.json";
            string jsonContent = File.ReadAllText(jsonPath);
            var playerConfig = JsonConvert.DeserializeObject<PlayerConfig>(jsonContent);  // Note: not a Dictionary

            var playerData = new PlayerData("player", playerConfig);
            var player = new Player(playerData);
            CameraManager.Instance.SetTarget(player);

            // Sprite
            var sprite = string.IsNullOrEmpty(playerConfig.SpriteId) ?
               new Sprite() : // Blank sprite for animator
               new Sprite(AssetManager.Instance.GetSprite(playerConfig.SpriteId));
            player.AddComponent(sprite);

            // Setup SpirteAnimations
            // Base Animations
            var baseAnimator = new SpriteAnimator(sprite);
            var spriteAnimations = playerConfig.GetSpriteAnimations();
            baseAnimator.AddAnimations(spriteAnimations);
            player.AddComponent(baseAnimator);

            // Overlay animations
            var overlaySprite = new OverlaySprite();
            player.AddComponent(overlaySprite);
            var overlayAnimator = new OverlayAnimator(overlaySprite);
            var overlayAnimations = playerConfig.GetOverlayAnimations();
            overlayAnimator.AddAnimations(overlayAnimations);
            player.AddComponent(overlayAnimator);

            player.SetAnimators(baseAnimator, overlayAnimator);

            // Setup TranfromAnimations
            //var transfromAnimations = playerConfig.GetTransformAnimations();
            //var tweener = new Tweener(player.Transform);
            //tweener.SetAnimations(transfromAnimations);
            //player.AddComponent(tweener);

            // TODO: Custom colliders exported from Tiled!!
            var blueWizardWalkAsset = AssetManager.Instance.GetSprite("Characters/Wizard/Walk");
            var collider = new BoxCollider2D(blueWizardWalkAsset.GetHalfSizeInUnits());
            player.AddComponent(collider);

            var rigidBody = new RigidBody2D();
            rigidBody.MaxHorizontalSpeed = playerData.Speed;
            // TODO: SetPhysicsProperties
            player.AddComponent(rigidBody);

            // Element state
            var elementState = new ElementState(playerElement, false);
            player.AddComponent(elementState);

            player.AddComponent(new CharacterController());

            // Weapon config (InitialWeapons)
            var weaponController = new WeaponController();
            player.AddComponent(weaponController);

            var weaponFactory = WeaponFactory.Instance;

            var weapon = weaponFactory.CreateWeapon("Blade", playerElement);
            var weapon2 = weaponFactory.CreateWeapon("Bow", playerElement, new Vector2(0.03f, 0.12f));
            
            //var weapon2 = weaponFactory.CreateWeapon("Sniper/Awp", playerElement);
            //weapon2.Transform.Translate(new Vector2(0f, 0.2f));

            //var weapon3 = weaponFactory.CreateWeapon("RocketLauncher/RPG", playerElement);
            //weapon3.Transform.Translate(nesw Vector2(0f, 0.2f));

            weaponController.AddWeapon(weapon);
            weaponController.AddWeapon(weapon2);
            //weaponController.AddWeapon(weapon3);
            weaponController.SetActiveWeapon("Blade");

            var playerController = new PlayerController();
            player.AddComponent(playerController);

            // Keep player refrence
            _player = player;

            return player;
        }
    }
}
