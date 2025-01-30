using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using Eclipse.Components.Engine;
using Eclipse.Engine.Cameras;
using Eclipse.Engine.Core;
using Eclipse.Engine.Systems.Audio;

namespace Eclipse.Engine.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        private readonly Dictionary<int, AudioEmitter> _activeEmitters = new();
        private readonly Queue<AudioEmitter> _emitterPool = new();

        // Music-specific fields
        private Song _currentSong;
        private float _currentMusicVolume;

        private float _musicVolume = 1f;
        private float _masterVolume = 1f;

        internal Vector2 ListenerPosition => PlayerManager.Instance.GetPlayerPosition();
        private const int POOL_SIZE = 32;

        // System for initial calcualtions
        private AudioSystem _audioSystem;

        internal void Initialize(AudioSystem audioSystem)
        {
            _audioSystem = audioSystem;

            // Audio playback objects
            for (int i = 0; i < POOL_SIZE; i++)
            {
                _emitterPool.Enqueue(new AudioEmitter());
            }
        }

        internal void PlayMusic(string songId, float volume = 1f, bool loop = true)
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

        internal void PlaySound(SoundEffectSource source, string soundId)
        {
            if (source == null || string.IsNullOrEmpty(soundId)) return;

            // Get emitter from pool (audio playback object)
            if (!_emitterPool.TryDequeue(out var emitter))
            {
                //Debug.LogWarning("Audio emitter pool exhausted");
                return;
            }

            // Get audio
            var sound = AssetManager.Instance.GetSoundEffect(soundId);

            if (sound == null)
            {
                _emitterPool.Enqueue(emitter);
                //Debug.LogWarning($"Audio clip not found: {source.SoundId}");
                return;
            }

            // Setup emitter parameters
            var instanceId = IDManager.GetId();
            source.ActiveInstances.Add(instanceId);

            // Track active emitter
            _activeEmitters[instanceId] = emitter;

            emitter.Volume = MathHelper.Clamp(source.Volume * _masterVolume, 0f, 1f);
            emitter.Pitch = MathHelper.Clamp(source.Pitch, 0.1f, 3f);
            emitter.Pan = MathHelper.Clamp(source.Pan, -1f, 1f);
            emitter.Loop = source.Loop;

            if (source.Is3D)
            {
                _audioSystem.UpdateSpatialAudio(source);
            }

            // Start actual playback
            emitter.Play(sound);
        }

        internal void StopSound(AudioSource source)
        {
            // Stop all instances for this source
            foreach (var instanceId in source.ActiveInstances)
            {
                ReleaseEmitter(instanceId);
            }
            source.ActiveInstances.Clear();
        }

        internal void UpdateSource(AudioSource source, float volume, float pan)
        {
            // Update all instances of this source
            foreach (var instanceId in source.ActiveInstances)
            {
                if (_activeEmitters.TryGetValue(instanceId, out var emitter))
                {
                    emitter.Volume = MathHelper.Clamp(volume * _masterVolume, 0f, 1f);
                    emitter.Pan = MathHelper.Clamp(pan, -1f, 1f);
                    emitter.UpdateParameters();
                }
            }
        }
        internal void UpdateInstances(AudioSource source)
        {
            //Console.WriteLine("Active instances: " + source.ActiveInstances.Count);
            //Console.WriteLine("Active emitters: " + _activeEmitters.Count);

            foreach (var instanceId in source.ActiveInstances.ToList())
            {
                // Check if emmiter is still active 
                // Remove + cleanup if not active
                if (ReleaseEmitter(instanceId, forceStop: false))
                {
                    // Emitter released, remove active instance
                    source.ActiveInstances.Remove(instanceId);
                }
            }
        }
        private bool ReleaseEmitter(int instanceId, bool forceStop = true)
        {
            if (_activeEmitters.TryGetValue(instanceId, out var emitter))
            {
                // Check if emmiter is still active 
                if (forceStop || !emitter.IsPlaying)
                {
                    emitter.Stop();
                    _emitterPool.Enqueue(emitter);
                    _activeEmitters.Remove(instanceId);
                    IDManager.ReleaseId(instanceId);
                    return true;
                }
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
                ReleaseEmitter(kvp.Key, true);
            }

            _activeEmitters.Clear();
            _emitterPool.Clear();
        }
    }
}
