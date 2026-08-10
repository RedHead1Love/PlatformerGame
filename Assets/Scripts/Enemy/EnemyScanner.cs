using UnityEngine;

namespace Shared.Sensors
{
    public sealed class EnemyScanner : MonoBehaviour
    {
        private const float DefaultSoundCooldown = 3f;
        private const float DefaultDetectionRange = 5f;

        [SerializeField] private float _detectionRange = DefaultDetectionRange;
        [SerializeField] private LayerMask _enemyLayerMask;

        private AudioController _audioController;
        private bool _hadEnemyNearby;
        private float _lastSoundTime;

        public bool HasEnemyNearby => Physics2D.OverlapCircle(
            transform.position,
            _detectionRange,
            _enemyLayerMask);

        private void Start()
        {
            InitializeAudioController();
        }

        private void Update()
        {
            UpdateEnemyDetection();
        }

        private void InitializeAudioController()
        {
            _audioController = GetComponent<AudioController>();

            if (_audioController == null)
            {
                _audioController = FindFirstObjectByType<AudioController>();
            }
        }

        private void UpdateEnemyDetection()
        {
            bool hasEnemyNearby = HasEnemyNearby;

            if (ShouldPlayDetectionSound(hasEnemyNearby))
            {
                PlayDetectionSound();
                _lastSoundTime = Time.time;
            }

            _hadEnemyNearby = hasEnemyNearby;
        }

        private bool ShouldPlayDetectionSound(bool hasEnemyNearby)
        {
            return hasEnemyNearby &&
                   _hadEnemyNearby == false &&
                   Time.time >= _lastSoundTime + DefaultSoundCooldown;
        }

        private void PlayDetectionSound()
        {
            _audioController?.PlayEnemyDetectedSound();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        }
    }
}