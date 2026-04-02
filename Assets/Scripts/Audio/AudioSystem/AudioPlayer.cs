using UnityEngine;

namespace AudioSystem
{
    public sealed class AudioPlayer : IAudioPlayer
    {
        private readonly AudioSource _audioSource;

        public AudioPlayer(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        public bool IsPlaying => _audioSource.isPlaying;

        public void Play(AudioClip audioClip)
        {
            if (audioClip == null)
            {
                return;
            }

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            if (audioClip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(audioClip);
        }

        public void PlayOneShot(AudioClip audioClip, float volume)
        {
            if (audioClip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(audioClip, volume);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void SetVolume(float volume)
        {
            _audioSource.volume = Mathf.Clamp01(volume);
        }

        public float GetVolume()
        {
            return _audioSource.volume;
        }
    }
}