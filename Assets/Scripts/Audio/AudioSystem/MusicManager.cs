using UnityEngine;

namespace AudioSystem
{
    public sealed class MusicManager
    {
        private const float MinimumTrackCount = 0;

        private readonly IAudioPlayer _audioPlayer;
        private readonly IMusicPlaylist _playlist;

        private bool _isBossFightActive;
        private bool _isBackgroundMusicPlaying;

        public MusicManager(IAudioPlayer audioPlayer, IMusicPlaylist playlist)
        {
            _audioPlayer = audioPlayer;
            _playlist = playlist;
        }

        public bool IsBossFightActive => _isBossFightActive;
        public bool IsBackgroundMusicPlaying => _isBackgroundMusicPlaying;

        public void Update()
        {
            if (_isBossFightActive == false && _isBackgroundMusicPlaying && _audioPlayer.IsPlaying == false)
            {
                PlayNextTrack();
            }
        }

        public void PlayNextTrack()
        {
            if (_playlist.HasTracks == false)
            {
                return;
            }

            AudioClip nextTrack = _playlist.GetNextTrack();

            if (nextTrack != null)
            {
                _audioPlayer.Play(nextTrack);
            }
        }

        public void PlayBoss(AudioClip bossMusic)
        {
            if (bossMusic == null)
            {
                return;
            }

            _isBossFightActive = true;
            _isBackgroundMusicPlaying = false;

            _audioPlayer.Stop();
            _audioPlayer.Play(bossMusic);
        }

        public void StopBoss()
        {
            _isBossFightActive = false;

            _audioPlayer.Stop();

            _isBackgroundMusicPlaying = true;

            PlayNextTrack();
        }

        public void Pause()
        {
            if (_isBossFightActive == false)
            {
                _audioPlayer.Stop();

                _isBackgroundMusicPlaying = false;
            }
        }

        public void Resume()
        {
            if (_isBossFightActive == false)
            {
                _isBackgroundMusicPlaying = true;

                PlayNextTrack();
            }
        }

        public void SetVolume(float volume)
        {
            _audioPlayer.SetVolume(volume);
        }

        public void StartBackground()
        {
            if (_playlist.HasTracks && _isBossFightActive == false)
            {
                _isBackgroundMusicPlaying = true;

                PlayNextTrack();
            }
        }
    }
}
