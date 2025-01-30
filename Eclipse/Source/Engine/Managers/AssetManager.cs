
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

using Eclipse.Engine.Core;
using Eclipse.Engine.Utils.Load;
using Eclipse.Engine.Utils.Load.Assets;

namespace Eclipse.Engine.Managers
{
    public class AssetManager : Singleton<AssetManager>
    {
        // TODO: 
        // AssetManager(high-level coordination)
        //├── SpriteManager(handles sprites/textures)
        //├── AudioManager(handles sound/music)
        //└── FontManager(handles fonts/text)

        private AssetLoader _assetLoader;

        private Dictionary<string, SpriteAsset> _sprites;
        private Dictionary<string, SpriteAsset> _atlasSprites;

        private Dictionary<string, SoundEffect> _soundEffects;
        private Dictionary<string, Song> _songs;
        private Dictionary<string, FontAsset> _fonts;

        private Dictionary<string, Effect> _shaders;

        public AssetManager()
        {
        }
        internal void Initialize(AssetLoader assetLoader)
        {
            _assetLoader = assetLoader;

            // Load before system initialization
            _shaders = _assetLoader.LoadShaders();
        }

        // Load assets through loader when needed
        internal void LoadContent()
        {
            _sprites = _assetLoader.LoadSprites();
            _atlasSprites = _assetLoader.LoadAtlases();

            _soundEffects = _assetLoader.LoadSoundEffects();
            _songs = _assetLoader.LoadSongs();

            _fonts = _assetLoader.LoadFonts();
        }

        // 
        internal SpriteAsset GetSprite(string name)
        {
            SpriteAsset asset;
            // Check sprites from atlases
            if (_atlasSprites.TryGetValue(name, out asset)) return asset;
            // Check standalone sprites
            if (_sprites.TryGetValue(name, out asset)) return asset;

            throw new KeyNotFoundException($"Sprite asset not found: {name}");
        }
        internal SoundEffect GetSoundEffect(string name)
        {
            if (!_soundEffects.TryGetValue(name, out SoundEffect asset))
            {
                throw new KeyNotFoundException($"SoundEffect asset not found: {name}");
            }
            return asset;
        }

        internal Song GetSong(string name)
        {
            if (!_songs.TryGetValue(name, out Song asset))
            {
                throw new KeyNotFoundException($"Song asset not found: {name}");
            }
            return asset;
        }

        // Add getter for fonts
        internal FontAsset GetFont(string name)
        {
            if (!_fonts.TryGetValue(name, out FontAsset asset))
            {
                throw new KeyNotFoundException($"Font asset not found: {name}");
            }
            return asset;
        }

        internal Effect GetShader(string name)
        {
            if (!_shaders.TryGetValue(name, out Effect shader))
            {
                throw new KeyNotFoundException($"Shader not found: {name}");
            }
            return shader;
        }
    }
}
