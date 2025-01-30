using System;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using TiledSharp;

using Eclipse.Engine.Factories;
using Eclipse.Engine.Managers;
using Eclipse.Engine.Systems.Render;
using Eclipse.Engine.Config;
using Eclipse.Components.Engine;

namespace Eclipse.Engine.Scenes
{
    internal class LevelLoader
    {
        private readonly TiledObjectFactory _tiledObjectFactory;


        internal LevelLoader()
        {
            _tiledObjectFactory = new TiledObjectFactory();
        }

        internal void LoadLevel(string levelName, Scene scene)
        {
            string fullPath = Path.Combine("Assets", "Game", "Maps", $"{levelName}.tmx");
            var level = new TmxMap(fullPath);

            // Load all object layers
            var objectLayers = level.ObjectGroups;
            var tilesets = level.Tilesets;

            // 1. Core world/environment first
            LoadEnvironment(scene, objectLayers["Environment"], tilesets);
            //LoadForeground(map.GetLayer("Foreground"));

            // 2. Level systems/triggers
            //LoadTriggers(map.GetLayer("Triggers"));

            // 3. Player (needs environment to be ready)
            LoadPlayer(scene, objectLayers["Player"]);

            // 4. Other entities (need player to be ready)
            LoadEnemies(scene, objectLayers["Enemies"]);
        }
        void LoadPlayer(Scene scene, TmxObjectGroup playerLayer)
        {
            var tiledObj = playerLayer.Objects[0];

            int id = tiledObj.Id;
            int gid = tiledObj.Tile.Gid;

            string name = tiledObj.Name;
            string className = tiledObj.Type;

            var (offset, rotation) = GetTiledObjectTransform(tiledObj);

            var player = PlayerManager.Instance.SpawnPlayer();
            player.Transform.Position = offset;

            scene.AddObject(player);
            scene.Player = player;
        }

        void LoadEnemies(Scene scene, TmxObjectGroup enemyLayer)
        {
            foreach (var tiledObj in enemyLayer.Objects)
            {
                int id = tiledObj.Id;
                int gid = tiledObj.Tile.Gid;

                string name = tiledObj.Name;
                string className = tiledObj.Type;

                var element = tiledObj.Properties.TryGetValue("Element", out var elementValue) ?
                    ParseElement(elementValue) :
                    Element.Neutral; // Default platfrom color

                var (offset, rotation) = GetTiledObjectTransform(tiledObj);

                var enemy = EnemyFactory.Instance.SpawnEnemy(name, offset, element);

                scene.AddObject(enemy);
            }
        }

        void LoadEnvironment(Scene scene, TmxObjectGroup environment, TmxList<TmxTileset> tilesets)
        {
            foreach (var tiledObj in environment.Objects)
            {
                // Access object properties

                int id = tiledObj.Id;
                int gid = tiledObj.Tile.Gid;

                string name = tiledObj.Name;
                string className = tiledObj.Type;

                // Get the tileset for this GID
                var tileset = tilesets.FirstOrDefault(ts =>
                    gid >= ts.FirstGid &&
                    gid < ts.FirstGid + ts.TileCount);


                // Calculate the local tile ID within the tileset
                int localId = gid - tileset.FirstGid;

                // Get the tile data from the tileset
                var tileData = tileset.Tiles[localId];

                var (offset, rotation) = GetTiledObjectTransform(tiledObj);

                // Flips status encoded in GID 
                bool isFlippedVertically = tiledObj.Tile.VerticalFlip;
                bool isFlippedHorizontally = tiledObj.Tile.HorizontalFlip;

                var objImagePath = tileData.Image.Source;
                float scaleX = (float)tiledObj.Width / (float)tileData.Image.Width;
                float scaleY = (float)tiledObj.Height / (float)tileData.Image.Height;
                var element = tiledObj.Properties.TryGetValue("Element", out var elementValue) ?
                    ParseElement(elementValue) :
                    Element.Neutral; // Default platfrom color

                var tiledObjectConfig = new TiledObjectConfig(
                    name,
                    className,
                    spriteId: GetSpriteId(objImagePath),
                    horizontalFlip: isFlippedHorizontally,
                    verticalFlip: isFlippedVertically,
                    element: element,
                    offset: new Vector2Config(offset.X, offset.Y),
                    rotation: rotation,
                    scale: new Vector2Config(scaleX, scaleY)
                );

                var obj = _tiledObjectFactory.CreateObject(tiledObjectConfig);
                scene.AddObject(obj);

            }
        }

        private Element ParseElement(string element)
        {
            return element switch
            {
                "Dark" => Element.Dark,
                "Light" => Element.Light,
                "Dual" => Element.Neutral,
                _ => throw new ArgumentException($"Unsupported element: {element}")
            };
        }
        private Color ParseHexColor(string hex)
        {
            hex = hex.TrimStart('#');
            return new Color(
                byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),  // R
                byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),  // G
                byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),  // B
                byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber)   // A
            );
        }

        private (Vector2 offset, float rotation) GetTiledObjectTransform(TmxObject tiledObj)
        {
            // x and y in pixels!! use PPU.ToUnits()
            // Tiled origin 0, height (bottom left)
            // Target origin: width/2, height/2 (center)

            float rotation = (float)(tiledObj.Rotation * Math.PI / 180.0f);
            float x = GetRotationAdjustedX(tiledObj.X, tiledObj.Width, tiledObj.Height, rotation);
            float y = GetRotationAdjustedY(tiledObj.Y, tiledObj.Width, tiledObj.Height, rotation);

            // Convert to units (/ 100)
            return (new Vector2(PPU.ToUnits(x), PPU.ToUnits(y)), rotation);
        }

        float GetRotationAdjustedX(double tiledX, double width, double height, float rotation)
        {
            double horizontalOffset = width * Math.Cos(rotation) / 2;
            double verticalOffset = height * Math.Sin(rotation) / 2;
            return (float)(tiledX + horizontalOffset + verticalOffset);
        }

        float GetRotationAdjustedY(double tiledY, double width, double height, float rotation)
        {
            double horizontalOffset = width * Math.Sin(rotation) / 2;
            double verticalOffset = height * Math.Cos(rotation) / 2;
            return (float)(tiledY - verticalOffset + horizontalOffset);
        }

        string GetSpriteId(string objImagePath)
        {
            // Remove extension
            var path = Path.ChangeExtension(objImagePath, null);
            // Get relative path
            var parts = path.Split("AtlasSource/Build/");
            return parts.Length > 1 ? parts[1] : path;
        }

    }
}
