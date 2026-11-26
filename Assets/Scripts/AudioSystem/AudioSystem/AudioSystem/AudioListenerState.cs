using UnityEngine;

namespace AudioSystem
{
    internal static class AudioListenerState
    {
        public static void Apply(bool cutscenePlaying, bool paused)
        {
            AudioListener.volume = cutscenePlaying || paused
                ? 0f
                : 1f;
            AudioListener.pause = !cutscenePlaying && paused;
        }
    }
}
