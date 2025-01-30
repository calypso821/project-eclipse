using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse.Engine.Managers;

namespace Eclipse.Components.Engine
{
    internal class MusicSource : AudioSource
    {
        internal override bool AllowOverlap { get; set; } = false;
        internal override bool Is3D { get; set; } = false;
        internal override bool Loop { get; set; } = true;


        internal void Play(string soundId)
        {
            Stop();
            AudioManager.Instance.PlayMusic(soundId);
        }

        internal override void Stop()
        {
            AudioManager.Instance.StopMusic();
        }
    }
}

