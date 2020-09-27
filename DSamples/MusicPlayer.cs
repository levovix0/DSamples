using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSamples
{
    static class MusicPlayer
    {
        public static float duration, t;

        static float GetT()
        {
            if (duration <= 0) return 0;
            return t / duration;
        }

        static void Play()
        {

        }

        static void Pause()
        {

        }

        static void Resume()
        {

        }

        static void Stop()
        {

        }
    }
}
