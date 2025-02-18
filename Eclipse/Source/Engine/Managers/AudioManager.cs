using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using Eclipse.Components.Engine;
using Eclipse.Engine.Core;
using Eclipse.Engine.Systems.Audio;

namespace Eclipse.Engine.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        private readonly Dictionary<int, AudioEmitter> _activeEmitters = new();
        private readonly Queue<AudioEmitter> _emitterPool = new();
        internal IReadOnlyDictionary<int, AudioEmitter> ActiveEmitters => _activeEmitters;

        // Music-specific fields
        private Song _currentSong;
        private float _currentMusicVolume;

        private float _musicVolume = 1f;
        private float _masterVolume = 1f;

        internal Vector2 ListenerPosition => PlayerManager.Instance.GetPlayerPosition();
        private const int POOL_SIZE = 32;

        internal void Initialize()
        {
            // Audio playback objects
            for (int i = 0; i < POOL_SIZE; i++)
            {
                _emitterPool.Enqueue(new AudioEmitter());
            }
        }

        internal void PlayMusic(string songId, float volume, bool loop)
        {
            var song = AssetManager.Instance.GetSong(songId);
            if (song == null) return;

            _currentSong = song;
            _currentMusicVolume = volume;

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Volume = _musicVolume * _masterVolume;
        }

        internal void StopMusic()
        {
            MediaPlayer.Stop();
            _currentSong = null;
        }

        internal void PlaySound(
            SFXSource source,
            string soundId,
            AudioData audioData)
        {
            if (source == null || string.IsNullOrEmpty(soundId)) return;

            // Get audio
            var sound = AssetManager.Instance.GetSoundEffect(audioData.AudioId);

            if (sound == null)
            {
                Console.WriteLine($"Audio clip not found: {soundId}");
                return;
            }

            // Get emitter from pool (audio playback object)
            if (!_emitterPool.TryDequeue(out var emitter))
            {
                Console.WriteLine("Audio emitter pool exhausted");
                return;
            }

            // Setup emitter parameters
            var instanceId = IDManager.GetId();

            // Track active emitter
            _activeEmitters[instanceId] = emitter;


            var pitch = audioData.RandomPitch ?
                        audioData.Pitch + (float)(new Random().NextDouble() * 0.2 - 0.1) :
                        audioData.Pitch;

            var sfxData = new SFXData
            {
                Volume = MathHelper.Clamp(audioData.Volume * _masterVolume, 0f, 1f),
                Pitch = MathHelper.Clamp(pitch, -1f, 1f),
                Pan = audioData.Pan,
                Loop = audioData.Loop
            };
            
            // Set emitter config
            emitter.Configure(soundId, source, sfxData);

            // Start actual playback
            emitter.Play(sound);
        }

        internal void StopSound(SFXSource source, string soundId = null)
        {
            // Find all emitters for this source
            var emittersToStop = _activeEmitters
                .Where(kvp => kvp.Value.AudioSource == source &&
                        (soundId == null || kvp.Value.SoundId == soundId))
                .Select(kvp => kvp.Key)
                .ToList();

            // Release emitters to stop 
            foreach (var emitterId in emittersToStop)
            {
                ReleaseEmitter(emitterId);
            }
        }

        internal bool ReleaseEmitter(int emitterId)
        {
            if (_activeEmitters.TryGetValue(emitterId, out var emitter))
            {
                emitter.Stop();
                _activeEmitters.Remove(emitterId);
                _emitterPool.Enqueue(emitter);
                IDManager.ReleaseId(emitterId);
                return true;
            }
            return false;
        }
        internal void SetMusicVolume(float volume)
        {
            _musicVolume = Math.Clamp(volume, 0f, 1f);
            Console.WriteLine("Music volume: " + volume);
            UpdateVolumes();
        }

        internal void SetMasterVolume(float volume)
        {
            _masterVolume = Math.Clamp(volume, 0f, 1f);
            Console.WriteLine("Master volume: " + volume);
            UpdateVolumes();
        }

        private void UpdateVolumes()
        {
            // Update music volume
            MediaPlayer.Volume = _musicVolume * _masterVolume * _currentMusicVolume;
        }

        internal void Cleanup()
        {
            foreach (var kvp in _activeEmitters)
            {
                ReleaseEmitter(kvp.Key);
            }

            _activeEmitters.Clear();
            _emitterPool.Clear();
        }
    }
}
