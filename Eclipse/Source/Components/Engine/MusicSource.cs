using Eclipse.Engine.Managers;
using Eclipse.Engine.Core;

namespace Eclipse.Components.Engine
{
    internal class MusicSource : Component
    {
        internal override void OnReset()
        {
            Stop();
        }
        internal void Play(string soundId, float volume = 1.0f, bool loop = true)
        {
            Stop();
            AudioManager.Instance.PlayMusic(soundId, volume, loop);
        }

        internal void Stop()
        {
            AudioManager.Instance.StopMusic();
        }
    }
}

