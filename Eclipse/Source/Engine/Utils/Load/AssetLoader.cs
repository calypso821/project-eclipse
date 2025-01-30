using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using Eclipse.Components.Engine;
using Eclipse.Engine.Utils.Load.Assets;
using Microsoft.Xna.Framework.Media;

namespace Eclipse.Engine.Utils.Load
{

    internal class AssetLoader
    {
        private readonly ContentManager _content;

        internal AssetLoader(ContentManager content)
        {
            _content = content;
        }

        internal Dictionary<string, SoundEffect> LoadSoundEffects()
        {
            var audio = new Dictionary<string, SoundEffect>();

            string audioPath = Path.Combine("Audio", "SFX");
            string contentPath = Path.Combine(_content.RootDirectory, audioPath);

            // Find all XNB files in Fonts directory and subdirectories
            foreach (string audioFile in Directory.GetFiles(contentPath, "*.xnb", SearchOption.AllDirectories))
            {
                // Get relative path from Content folder and remove .xnb extension
                string relativePath = Path.GetRelativePath(_content.RootDirectory, audioFile)
                    .Replace(".xnb", "");

                // Get audio name without extension for dictionary key
                string audioName = relativePath.Substring(audioPath.Length).Trim('\\').Replace('\\', '/');

                SoundEffect soundEffect = _content.Load<SoundEffect>(relativePath);
                //Console.WriteLine($"Channels: {soundEffect.Format.Channels}");
                // Load font through content manager
                audio.Add(audioName, soundEffect);
            }

            return audio;
        }
        internal Dictionary<string, Song> LoadSongs()
        {
            var audio = new Dictionary<string, Song>();

            string musicPath = Path.Combine("Audio", "Music");
            string contentPath = Path.Combine(_content.RootDirectory, musicPath);

            // Find all XNB files in Fonts directory and subdirectories
            foreach (string audioFile in Directory.GetFiles(contentPath, "*.xnb", SearchOption.AllDirectories))
            {
                // Get relative path from Content folder and remove .xnb extension
                string relativePath = Path.GetRelativePath(_content.RootDirectory, audioFile)
                    .Replace(".xnb", "");

                // Get audio name without extension for dictionary key
                string audioName = relativePath.Substring(musicPath.Length).Trim('\\').Replace('\\', '/');

                Song song = _content.Load<Song>(relativePath);
                //Console.WriteLine($"Channels: {soundEffect.Format.Channels}");
                // Load font through content manager
                audio.Add(audioName, song);
            }

            return audio;
        }
        internal Dictionary<string, FontAsset> LoadFonts()
        {
            var fonts = new Dictionary<string, FontAsset>();

            string fontPath = Path.Combine("Font");
            string contentPath = Path.Combine(_content.RootDirectory, fontPath);

            // Find all XNB files in Fonts directory and subdirectories
            foreach (string fontFile in Directory.GetFiles(contentPath, "*.xnb", SearchOption.AllDirectories))
            {
                // Get relative path from Content folder and remove .xnb extension
                string relativePath = Path.GetRelativePath(_content.RootDirectory, fontFile)
                    .Replace(".xnb", "");

                // Get font name without extension for dictionary key
                string fontName = relativePath.Substring(fontPath.Length).Trim('\\').Replace('\\', '/');
                //string fontName = Path.GetFileNameWithoutExtension(fontFile);

                // Load font through content manager
                SpriteFont spriteFont = _content.Load<SpriteFont>(relativePath);
                fonts.Add(fontName, new FontAsset(spriteFont));
            }

            return fonts;
        }

        internal Dictionary<string, Effect> LoadShaders()
        {
            var shaders = new Dictionary<string, Effect>();

            string shaderPath = Path.Combine("Shader");
            string contentPath = Path.Combine(_content.RootDirectory, shaderPath);

            // Find all XNB files in Fonts directory and subdirectories
            foreach (string fontFile in Directory.GetFiles(contentPath, "*.xnb", SearchOption.AllDirectories))
            {
                // Get relative path from Content folder and remove .xnb extension
                string relativePath = Path.GetRelativePath(_content.RootDirectory, fontFile)
                    .Replace(".xnb", "");

                // Get font name without extension for dictionary key
                string fontName = relativePath.Substring(shaderPath.Length).Trim('\\').Replace('\\', '/');
                //string fontName = Path.GetFileNameWithoutExtension(fontFile);

                // Load font through content manager
                Effect shader = _content.Load<Effect>(relativePath);
                shaders.Add(fontName, shader);
            }

            return shaders;
        }

        internal Dictionary<string, SpriteAsset> LoadAtlases()
        {
            string spritesPath = Path.Combine(_content.RootDirectory, "Atlas");

            Dictionary<string, Texture2D> textures = new();
            Dictionary<string, SpriteAsset> spriteAssets = new();

            var atlasParser = new AtlasParser(spritesPath, textures, spriteAssets);

            // First load all atlases and their textures
            foreach (string atlasFile in Directory.GetFiles(spritesPath, "*.atlas", SearchOption.AllDirectories))
            {
                string atlasDirectory = Path.GetDirectoryName(atlasFile);
                var textureRefs = ParseAtlasTextureReferences(atlasFile);

                // Load atlas textures
                foreach (string textureRef in textureRefs)
                {
                    if (!textures.ContainsKey(textureRef))
                    {
                        // Create path relative to atlas location
                        string texturePath = Path.Combine(
                            GetRelativePath(atlasDirectory),
                            textureRef
                        );
                        textures[textureRef] = _content.Load<Texture2D>(texturePath);
                    }
                }
                atlasParser.ParseAtlas(atlasFile);
            }

            // Clear temporary texture references after all parsing is done
            textures.Clear();

            ValidateSprites(spriteAssets);
            return spriteAssets;
        }

        internal Dictionary<string, SpriteAsset> LoadSprites()
        {
            string spritesPath = Path.Combine(_content.RootDirectory, "Sprite");

            Dictionary<string, SpriteAsset> spriteAssets = new();

            // Load standalone sprites
            foreach (string file in Directory.GetFiles(spritesPath, "*.xnb", SearchOption.AllDirectories))
            {
                string relativePath = file.Replace(spritesPath + Path.DirectorySeparatorChar, "");

                string name = Path.GetFileNameWithoutExtension(relativePath);
                // Custom origin???
                string assetName = relativePath.Replace(".xnb", "").Replace('\\', '/'); ;
                spriteAssets[assetName] = new SpriteAsset(_content.Load<Texture2D>(GetRelativePath(file)), true);
            }

            ValidateSprites(spriteAssets);
            return spriteAssets;
        }


        private List<string> ParseAtlasTextureReferences(string atlasFile)
        {
            // Read and parse atlas file to extract texture references
            var textureRefs = new List<string>();
            using (var reader = new StreamReader(atlasFile))
            {
                // Parse your atlas format here and extract texture references
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    // Found new atlas page
                    if (line.EndsWith(".png"))
                    {
                        textureRefs.Add(Path.GetFileNameWithoutExtension(line));
                    }
                }
            }
            return textureRefs;
        }

        private string GetRelativePath(string fullPath)
        {
            // Get relative path 
            // "Content\\Sprites\\VFX\\Eletric A-Sheet.xnb" ...
            // "Sprites\\VFX\\Eletric A-Sheet"

            return fullPath
                .Replace(_content.RootDirectory + Path.DirectorySeparatorChar, "")
                .Replace(".xnb", "")
                .Replace(".png", "");
        }

        private void ValidateSprites(Dictionary<string, SpriteAsset> spriteAssets)
        {
            foreach (var kvp in spriteAssets)
            {
                var sprite = kvp.Value;
                var name = kvp.Key;

                // Check if frames collection exists
                if (sprite.Frames == null)
                    throw new InvalidOperationException($"Sprite {name} has null frames collection");

                // Check for null frames within collection
                for (int i = 0; i < sprite.Frames.Count; i++)
                {
                    if (sprite.Frames[i].Equals(default(Frame)))
                        throw new InvalidOperationException($"Sprite {name} has null frame at index {i}");
                }
            }
        }
    }
}