using System.Collections;
using System.Collections.Generic;
using Foxberry;
using Player;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public sealed class AudioController : MonoBehaviour
    {
        private const float MinimumVolume = 0f;
        private const float MaximumVolume = 1f;

        [SerializeField]
        private AudioMixerGroup _musicMixerGroup;
        [SerializeField]
        private AudioMixerGroup _sfxMixerGroup;
        [SerializeField]
        private SoundConfiguration _soundConfiguration;
        [SerializeField]
        private AudioClip _healSound;
        [SerializeField]
        private AudioClip _takeDamageSound;
        [SerializeField]
        private AudioClip _deathSound;
        [SerializeField]
        private AudioClip _enemyDetectedSound;
        [SerializeField]
        private AudioClip _victorySound;
        [SerializeField]
        private AudioClip _buttonClickSound;
        [SerializeField]
        private List<AudioClip> _backgroundMusicTracks = new List<AudioClip>();
        [SerializeField]
        private AudioClip _lightningTrapActivationSound;
        [SerializeField]
        private AudioClip _lightningTrapDamageSound;
        [SerializeField]
        private AudioClip _lightningTrapDeactivationSound;
        [SerializeField]
        private AudioClip _footstepSound;
        [SerializeField]
        private float _footstepVolumeMultiplier = 0.7f;
        [SerializeField]
        private bool _loopFootstepSound = true;
        [Range(0f, 1f)]
        [SerializeField]
        private float _musicVolume = 0.5f;
        [Range(0f, 1f)]
        [SerializeField]
        private float _soundEffectsVolume = 1f;
        [SerializeField]
        private AudioClip _defaultDoorOpenSound;
        [SerializeField]
        private AudioClip _defaultDoorCloseSound;
        [SerializeField]
        private AudioClip _armorDamageSound;
        [SerializeField]
        private AudioClip _armorBreakSound;
        [SerializeField]
        private float _armorSoundVolumeMultiplier = 1f;
        private IAudioPlayer _soundEffectsPlayer;
        private IAudioPlayer _musicPlayer;
        private IMusicPlaylist _musicPlaylist;
        private MusicPlayback _musicManager;
        private AudioSource _footstepSource;
        private bool _isMoving;

        public float MusicVolume => _musicVolume;

        public float SoundEffectsVolume => _soundEffectsVolume;

        public bool IsPlayingFootsteps => _isMoving && _footstepSource != null && _footstepSource.isPlaying;

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp(volume, MinimumVolume, MaximumVolume);

            _musicPlayer.SetVolume(_musicVolume);
        }

        public void SetSoundEffectsVolume(float volume)
        {
            _soundEffectsVolume = Mathf.Clamp(volume, MinimumVolume, MaximumVolume);

            _soundEffectsPlayer.SetVolume(_soundEffectsVolume);

            UpdateFootstepVolume();
        }

        public void StartFootsteps()
        {
            if (_isMoving || _footstepSource == null || _footstepSource.clip == null)
                return;
            _isMoving = true;
            _footstepSource.Play();
        }

        public void StopFootsteps()
        {
            _isMoving = false;
            _footstepSource?.Stop();
        }

        public void PauseFootsteps()
        {
            _footstepSource?.Pause();
        }

        public void ResumeFootsteps()
        {
            if (_isMoving && _footstepSource != null && !_footstepSource.isPlaying)
                _footstepSource.Play();
        }

        public void SetFootstepSound(AudioClip newClip, bool playImmediately = false)
        {
            _footstepSound = newClip;
            if (_footstepSource == null)
                return;
            _footstepSource.Stop();
            _footstepSource.clip = newClip;
            if (playImmediately && _isMoving && newClip != null)
                _footstepSource.Play();
        }

        public void SetFootstepVolumeMultiplier(float multiplier)
        {
            _footstepVolumeMultiplier = Mathf.Clamp01(multiplier);

            UpdateFootstepVolume();
        }

        public void PlayAttackSound(HeroAnimationState state, bool hitConnected)
        {
            if (_soundConfiguration == null)
            {
                return;
            }

            switch (state)
            {
            case HeroAnimationState.Attack:
                PlaySoundWithDelay(
                    hitConnected ? _soundConfiguration.Attack1HitSound
                                                : _soundConfiguration.Attack1MissSound,
                    _soundConfiguration.Attack1SoundDelay);
                break;
            case HeroAnimationState.Attack2:
                PlaySoundWithDelay(
                    hitConnected ? _soundConfiguration.Attack2HitSound
                                                : _soundConfiguration.Attack2MissSound,
                    _soundConfiguration.Attack2SoundDelay);
                break;
            case HeroAnimationState.Attack3:
                PlaySoundWithDelay(
                    hitConnected ? _soundConfiguration.Attack3HitSound
                                                : _soundConfiguration.Attack3MissSound,
                    _soundConfiguration.Attack3SoundDelay);
                break;
            case HeroAnimationState.AirAttack:
                PlaySoundWithDelay(
                    hitConnected ? _soundConfiguration.AirAttackHitSound
                                                : _soundConfiguration.AirAttackMissSound,
                    _soundConfiguration.AirAttackSoundDelay);
                break;
            }
        }

        public void PlayAttack1HitSound() => PlayAttackSound(HeroAnimationState.Attack, true);

        public void PlayAttack2HitSound() => PlayAttackSound(HeroAnimationState.Attack2, true);

        public void PlayAttack3HitSound() => PlayAttackSound(HeroAnimationState.Attack3, true);

        public void PlayAirAttackHitSound() => PlayAttackSound(HeroAnimationState.AirAttack, true);

        public void PlayAttack1MissSound() => PlayAttackSound(HeroAnimationState.Attack, false);

        public void PlayAttack2MissSound() => PlayAttackSound(HeroAnimationState.Attack2, false);

        public void PlayAttack3MissSound() => PlayAttackSound(HeroAnimationState.Attack3, false);

        public void PlayAirAttackMissSound() => PlayAttackSound(HeroAnimationState.AirAttack, false);

        public void PlayTakeDamageSound()
        {
            PlaySound(_takeDamageSound);
        }

        public void PlayDeathSound()
        {
            PlaySound(_deathSound);
        }

        public void PlayHealSound()
        {
            PlaySound(_healSound);
        }

        public void PlayVictorySound()
        {
            PlaySound(_victorySound);
        }

        public void PlayButtonClickSound()
        {
            PlaySound(_buttonClickSound);
        }

        public void PlayEnemyDetectedSound()
        {
            PlaySound(_enemyDetectedSound);
        }

        public void PlayBossDoorOpenSound()
        {
            PlaySound(_soundConfiguration?.BossDoorOpenSound);
        }

        public void PlayLightningTrapActivationSound()
        {
            PlaySound(_lightningTrapActivationSound);
        }

        public void PlayLightningTrapDamageSound()
        {
            PlaySound(_lightningTrapDamageSound);
        }

        public void PlayLightningTrapDeactivationSound()
        {
            PlaySound(_lightningTrapDeactivationSound);
        }

        public void PlayDefaultDoorOpenSound()
        {
            PlaySound(_defaultDoorOpenSound);
        }

        public void PlayDefaultDoorCloseSound()
        {
            PlaySound(_defaultDoorCloseSound);
        }

        public void PlayArmorDamageSound()
        {
            PlayOneShotWithVolume(_armorDamageSound, _armorSoundVolumeMultiplier);
        }

        public void PlayArmorBreakSound()
        {
            PlayOneShotWithVolume(_armorBreakSound, _armorSoundVolumeMultiplier);
        }

        public AudioClip GetLightningTrapActivationSound()
        {
            return _lightningTrapActivationSound;
        }

        public void PlayOneShot(AudioClip clip)
        {
            PlayOneShotWithVolume(clip);
        }

        public void PlayOneShotWithVolume(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null)
            {
                return;
            }

            float volume = Mathf.Clamp(_soundEffectsVolume * volumeMultiplier, MinimumVolume, MaximumVolume);

            _soundEffectsPlayer.PlayOneShot(clip, volume);
        }

        public void PlayBossMusic(AudioClip bossMusic)
        {
            _musicManager?.PlayBossMusic(bossMusic);
        }

        public void StopBossMusic()
        {
            _musicManager?.StopBossMusic();
        }

        public void PauseBackgroundMusic()
        {
            _musicManager?.PauseBackgroundMusic();
        }

        public void ResumeBackgroundMusic()
        {
            _musicManager?.ResumeBackgroundMusic();
        }

        public void SkipCurrentTrack()
        {
            _musicManager?.PlayNextTrack();
        }

        private void Awake()
        {
            InitializeAudioSources();
            InitializeMusicSystem();
            InitializeFootstepSource();

            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.8f));
            SetSoundEffectsVolume(PlayerPrefs.GetFloat("SFXVolume", 0.8f));

            StartBackgroundMusic();
        }

        private void Update()
        {
            _musicManager?.Update();

            if (MiniMapController.IsCutscenePlaying)
            {
                AudioListener.volume = 0f;
                AudioListener.pause = false;
                return;
            }

            if (PauseMenuController.IsGamePaused)
            {
                AudioListener.volume = 0f;
                AudioListener.pause = true;
            }
            else
            {
                AudioListener.volume = 1f;
                AudioListener.pause = false;
            }
        }

        private void InitializeAudioSources()
        {
            AudioSource soundEffectsAudioSource = GetComponent<AudioSource>();

            if (soundEffectsAudioSource == null)
            {
                soundEffectsAudioSource = gameObject.AddComponent<AudioSource>();
            }

            soundEffectsAudioSource.playOnAwake = false;

            if (_sfxMixerGroup != null)
            {
                soundEffectsAudioSource.outputAudioMixerGroup = _sfxMixerGroup;
            }

            _soundEffectsPlayer = new AudioPlayer(soundEffectsAudioSource);

            AudioSource musicSource = gameObject.AddComponent<AudioSource>();

            musicSource.loop = false;
            musicSource.playOnAwake = false;

            if (_musicMixerGroup != null)
            {
                musicSource.outputAudioMixerGroup = _musicMixerGroup;
            }

            _musicPlayer = new AudioPlayer(musicSource);
        }

        private void InitializeMusicSystem()
        {
            _musicPlaylist = new MusicPlaylist();
            _musicPlaylist.Initialize(_backgroundMusicTracks);

            _musicManager = new MusicPlayback(_musicPlayer, _musicPlaylist);

            SetMusicVolume(_musicVolume);
        }

        private void InitializeFootstepSource()
        {
            _footstepSource = gameObject.AddComponent<AudioSource>();
            _footstepSource.playOnAwake = false;
            _footstepSource.loop = _loopFootstepSound;
            _footstepSource.clip = _footstepSound;

            UpdateFootstepVolume();
        }

        private void UpdateFootstepVolume()
        {
            if (_footstepSource == null)
            {
                return;
            }

            _footstepSource.volume = Mathf.Clamp01(_soundEffectsVolume * _footstepVolumeMultiplier);
        }

        private void PlaySound(AudioClip clip)
        {
            PlayOneShotWithVolume(clip);
        }

        private void PlaySoundWithDelay(AudioClip clip, float delay)
        {
            if (clip == null)
            {
                return;
            }

            if (delay <= Mathf.Epsilon)
            {
                PlaySound(clip);

                return;
            }

            StartCoroutine(PlayDelayedSoundCoroutine(clip, delay));
        }

        private IEnumerator PlayDelayedSoundCoroutine(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);

            PlaySound(clip);
        }

        private void StartBackgroundMusic()
        {
            _musicManager?.StartBackgroundMusic();
        }
    }
}
