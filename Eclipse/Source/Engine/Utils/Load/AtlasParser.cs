using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Eclipse.Engine.Utils.Load.Assets;

namespace Eclipse.Engine.Utils.Load
{
    internal class AtlasParser
    {
        private readonly Dictionary<string, Texture2D> _textures;
        private readonly Dictionary<string, SpriteAsset> _spriteAssets;
        private readonly string _spritesPath;

        internal AtlasParser(
            string spritesPath,
            Dictionary<string, Texture2D> textures,
            Dictionary<string, SpriteAsset> spriteAssets)
        {
            _spritesPath = spritesPath;
            _textures = textures;
            _spriteAssets = spriteAssets;
        }

        internal void ParseAtlas(string filepath)
        {
            // Atlas Format (.atlas)
            //weapons_atlas.png         (texture file)
            //size:1024,1024            (size)
            //repeat: none              (??)

            //idle / wizard_idle        (asset name)
            //index: 1                  (frame index if multiple sprites)
            //bounds: 668,704,155,279   (x, y, width, hieght) - rectangle
            //offsets: 178,110,512,512  (if trimed - offset, original size)
            //rotate: true              (if rotated by 90 degrees CW)
            // If  '\n\n' string detected - multiple textures (pages)                 
            using (var reader = new StreamReader(filepath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    // Found new atlas page
                    if (line.EndsWith(".png"))
                    {
                        ParsePage(line, reader, filepath);
                    }
                }
            }
        }

        private void ParsePage(string textureLine, StreamReader reader, string atlasPath)
        {
            string relativeAtlasPath = atlasPath.Replace(_spritesPath + Path.DirectorySeparatorChar, "");
            string atlasDirectory = Path.GetDirectoryName(relativeAtlasPath);
            // Parse header
            var textureName = Path.GetFileNameWithoutExtension(textureLine);
            if (!_textures.TryGetValue(textureName, out Texture2D texture))
            {
                throw new KeyNotFoundException($"Texture not found: {textureName}");
            }

            // Read size
            string sizeLine = reader.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(sizeLine) || !sizeLine.StartsWith("size:"))
                throw new FormatException("Invalid size line");

            var parts = sizeLine.Split(':')[1].Trim().Split(',');
            Vector2 size = new(
                float.Parse(parts[0].Trim()),
                float.Parse(parts[1].Trim())
            );

            // Read repeat
            string repeatLine = reader.ReadLine()?.Trim();
            var repeat = repeatLine?.Split(':')[1].Trim();

            // Parse sprites
            ParseBody(reader, texture, atlasDirectory);
        }

        private void ParseBody(StreamReader reader, Texture2D texture, string atlasPath)
        {
            // Start with first sprite
            string filename = reader.ReadLine().Trim();

            int index = 0;
            Rectangle bounds = Rectangle.Empty;
            bool isRotated = false;

            // Read trough all lines of body
            while (true)
            {
                string line = reader.ReadLine()?.Trim();

                // End of page or empty line
                if (string.IsNullOrEmpty(line) || line.EndsWith(".png"))
                {
                    // Add last sprite if exists
                    AddSpriteToCollection(filename, atlasPath, index, texture, bounds, isRotated);

                    // Porcess next page if exist
                    if (line?.EndsWith(".png") == true)
                    {
                        // .png (texture name) line was read
                        // move current position of reader back to start of new line
                        // line.Length                  // "page2.png" = 9 characters
                        // +Environment.NewLine.Length  // "\r\n" = 2 characters on Windows
                        reader.BaseStream.Position -= line.Length + Environment.NewLine.Length;
                    }
                    break; // No break if next page??
                }

                // Parse sprite data
                if (line.StartsWith("index:"))
                {
                    index = int.Parse(line.Split(':')[1].Trim());
                }
                else if (line.StartsWith("bounds:"))
                {
                    var stringBounds = line.Split(':')[1].Trim().Split(',');
                    bounds = new Rectangle(
                        x: int.Parse(stringBounds[0].Trim()),       // x
                        y: int.Parse(stringBounds[1].Trim()),       // y
                        width: int.Parse(stringBounds[2].Trim()),   // width
                        height: int.Parse(stringBounds[3].Trim())   // height
                    );
                }
                else if (line.StartsWith("rotate:"))
                {
                    isRotated = true;
                }
                else if (line.StartsWith("offsets:"))
                {
                    // Offset: if srpite was trimmed (original size)
                    // Pass for now
                    continue;
                }
                else
                {
                    // Add sprite to collection
                    AddSpriteToCollection(filename, atlasPath, index, texture, bounds, isRotated);

                    // Start new sprite
                    filename = line;
                    index = 0;
                    bounds = Rectangle.Empty;
                    isRotated = false;
                }
            }
        }

        private void AddSpriteToCollection(
            string filename, string atlasPath,
            int index, Texture2D texture, 
            Rectangle bounds, bool isRotated)
        {
            var (spriteName, originOut) = ParseSpriteName(filename);

            // sprite -> frame part of animation
            if (Path.GetFileName(spriteName) == "frame")
            {
                // Animation name = directory name
                spriteName = Path.GetDirectoryName(spriteName);
            }
            string fullSpriteName = Path.Combine(atlasPath, spriteName).Replace('\\', '/');

            // Set origin to middle of sprite if not in filename
            Vector2 origin = originOut ?? new Vector2(bounds.Width / 2, bounds.Height / 2);

            // Check if entry for current name exist (if not create new one)
            if (!_spriteAssets.TryGetValue(fullSpriteName, out var spriteAsset))
            {
                _spriteAssets[fullSpriteName] = new SpriteAsset(texture, origin, false);
            }

            // Add Source rectangle (frame) into list
            _spriteAssets[fullSpriteName].AddFrame(index, bounds, isRotated);
        }

        private static (string spriteName, Vector2? origin) ParseSpriteName(string filename)
        {
            // No origin in filename, return original name and null origin
            if (!filename.Contains("_origin_"))
            {
                return (filename, null);
            }

            var parts = filename.Split("_origin_");

            string spriteName = parts[0];
            string originPart = parts[1];

            var coordinates = originPart.Split('x');

            if (int.TryParse(coordinates[0], out int x) &&
                int.TryParse(coordinates[1], out int y))
            {
                return (spriteName, new Vector2(x, y));
            }

            return (filename, null);
        }
    }
}