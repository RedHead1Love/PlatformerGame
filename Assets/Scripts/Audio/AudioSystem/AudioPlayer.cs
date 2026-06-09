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

        public bool IsPlaying => _audioSource != null && _audioSource.isPlaying;

        public void Play(AudioClip audioClip)
        {
            if (_audioSource == null || audioClip == null)
            {
                return;
            }

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            if (_audioSource == null || audioClip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(audioClip);
        }

        public void PlayOneShot(AudioClip audioClip, float volume)
        {
            if (_audioSource == null || audioClip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(audioClip, Mathf.Clamp01(volume));
        }

        public void Stop()
        {
            if (_audioSource == null)
            {
                return;
            }

            _audioSource.Stop();
        }

        public void SetVolume(float volume)
        {
            if (_audioSource == null)
            {
                return;
            }

            _audioSource.volume = Mathf.Clamp01(volume);
        }

        public float GetVolume()
        {
            if (_audioSource == null)
            {
                return 0f;
            }

            return _audioSource.volume;
        }
    }
}