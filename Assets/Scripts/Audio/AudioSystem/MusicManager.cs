using UnityEngine;

namespace AudioSystem
{
    public sealed class MusicManager
    {
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
            if (!_isBossFightActive && _isBackgroundMusicPlaying && !_audioPlayer.IsPlaying)
            {
                PlayNextTrack();
            }
        }

        public void PlayNextTrack()
        {
            if (!_playlist.HasTracks)
            {
                return;
            }

            AudioClip nextTrack = _playlist.GetNextTrack();

            if (nextTrack != null)
            {
                _audioPlayer.Play(nextTrack);
            }
        }

        public void PlayBossMusic(AudioClip bossMusic)
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

        public void StopBossMusic()
        {
            _isBossFightActive = false;
            _audioPlayer.Stop();

            _isBackgroundMusicPlaying = true;
            PlayNextTrack();
        }

        public void PauseBackgroundMusic()
        {
            if (!_isBossFightActive)
            {
                _audioPlayer.Stop();
                _isBackgroundMusicPlaying = false;
            }
        }

        public void ResumeBackgroundMusic()
        {
            if (!_isBossFightActive)
            {
                _isBackgroundMusicPlaying = true;
                PlayNextTrack();
            }
        }

        public void SetVolume(float volume)
        {
            _audioPlayer.SetVolume(volume);
        }

        public void StartBackgroundMusic()
        {
            if (_playlist.HasTracks && !_isBossFightActive)
            {
                _isBackgroundMusicPlaying = true;
                PlayNextTrack();
            }
        }
    }
}