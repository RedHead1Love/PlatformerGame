using UnityEngine;

namespace AudioSystem
{
    internal sealed class BossAudio
    {
        private readonly AudioController _output;
        private readonly AudioClip _music;

        public BossAudio(AudioController output, AudioClip music)
        {
            _output = output;
            _music = music;
        }

        public void StartMusic()
        {
            if (_output != null && _music != null)
            {
                _output.PlayBossMusic(_music);
            }
        }

        public void StopMusic()
        {
            if (_output != null)
            {
                _output.StopBossMusic();
            }
        }

        public void Play(AudioClip clip, float volume)
        {
            if (_output != null && clip != null)
            {
                _output.PlayOneShotWithVolume(clip, volume);
            }
        }
    }
}
