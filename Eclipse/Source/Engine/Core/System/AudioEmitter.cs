using Microsoft.Xna.Framework.Audio;


namespace Eclipse.Engine.Core
{
    internal class AudioEmitter
    {
        internal SoundEffectInstance SoundInstance { get; private set; }
        internal float Volume { get; set; } = 1f;
        internal float Pitch { get; set; } = 1f;
        internal float Pan { get; set; }
        internal bool Loop { get; set; }

        internal bool IsPlaying => SoundInstance?.State == SoundState.Playing;

        internal void Play(SoundEffect sound)
        {
            Stop();  // Stop any existing playback
            SoundInstance = sound.CreateInstance();
            SoundInstance.Volume = Volume;
            SoundInstance.Pitch = Pitch;
            SoundInstance.Pan = Pan;
            SoundInstance.IsLooped = Loop;
            SoundInstance.Play();
        }

        internal void Stop()
        {
            if (SoundInstance != null)
            {
                SoundInstance.Stop();
                SoundInstance.Dispose();
                SoundInstance = null;
            }
        }

        internal void UpdateParameters()
        {
            if (SoundInstance != null)
            {
                SoundInstance.Volume = Volume;
                SoundInstance.Pitch = Pitch;
                SoundInstance.Pan = Pan;
            }
        }
    }
}
