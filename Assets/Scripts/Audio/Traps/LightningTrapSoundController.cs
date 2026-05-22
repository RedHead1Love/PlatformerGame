using AudioSystem;
using UnityEngine;

namespace Traps
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class LightningTrapSoundController : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        private const float DefaultVolume = 1f;

        [Header("Sound Configuration")]
        [SerializeField] private AudioClip _lightningStrikeSound;

        [Header("Audio Player Integration")]
        [SerializeField] private AudioController _audioController;

        [Header("Distance Settings")]
        [SerializeField] private bool _enableDistanceCheck = true;
        [SerializeField] private float _maxSoundDistance = 20f;

        private IAudioPlayer _audioPlayer;
        private IDistanceChecker _distanceChecker;
        private Transform _playerTransform;

        private void Awake()
        {
            InitializeAudioPlayer();
            InitializePlayerReference();
            InitializeDistanceChecker();

            if (_audioController == null)
            {
                _audioController = FindFirstObjectByType<AudioController>();
            }
        }

        private void InitializeAudioPlayer()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;

            _audioPlayer = new AudioPlayer(audioSource);
        }

        private void InitializePlayerReference()
        {
            GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);

            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        private void InitializeDistanceChecker()
        {
            _distanceChecker = new DistanceChecker();
        }

        public void PlayLightningStrikeSound()
        {
            if (_lightningStrikeSound == null || !ShouldPlaySound())
            {
                return;
            }

            float volume = GetSoundVolume();
            _audioPlayer.PlayOneShot(_lightningStrikeSound, volume);
        }

        private bool ShouldPlaySound()
        {
            if (!_enableDistanceCheck || _playerTransform == null)
            {
                return true;
            }

            return _distanceChecker.IsWithinDistance(transform.position, _playerTransform.position, _maxSoundDistance);
        }

        private float GetSoundVolume()
        {
            return _audioController != null ? _audioController.SoundEffectsVolume : DefaultVolume;
        }

        public void StopSound()
        {
            _audioPlayer.Stop();
        }

        public void SetLightningSound(AudioClip clip)
        {
            _lightningStrikeSound = clip;
        }

        public void SetMaxDistance(float distance)
        {
            _maxSoundDistance = distance;
        }

        public void SetAudioController(AudioController controller)
        {
            _audioController = controller;
        }
    }
}
