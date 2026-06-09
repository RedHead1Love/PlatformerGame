using GeneralLogicEnemies;
using Player;
using Shared.Damage;
using UnityEngine;

namespace EasyBossLogic
{
    [RequireComponent(typeof(Animator))]
    public sealed class BossReaper : Entity, IDamageable
    {
        private const float DestroyDelay = 3f;
        private const float DefaultSoundEffectVolume = 1f;
        private const float AttackMissSoundVolumeMultiplier = 0.7f;

        [Header("Player Detection")]
        [SerializeField] private Transform _groundCheckPoint;
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private float _detectionRadius = 1f;

        [Header("Movement and Attack")]
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackCooldown = 2f;
        [SerializeField] private int _attackDamage = 1;

        [Header("Evasion")]
        [SerializeField] private float _dodgeChance = 0.25f;
        [SerializeField] private float _dodgeDistance = 2f;
        [SerializeField] private float _dodgeDuration = 0.4f;
        [SerializeField] private float _dodgeCooldown = 1f;

        [Header("Boss Music")]
        [SerializeField] private AudioClip _bossMusic;

        [Header("Boss Sound Effects")]
        [SerializeField] private AudioClip _attackSound;
        [SerializeField] private AudioClip _attackMissSound;
        [SerializeField] private AudioClip _takeDamageSound;
        [SerializeField] private AudioClip _deathSound;
        [SerializeField] private AudioClip _dodgeSound;
        [SerializeField] private float _soundEffectVolume = DefaultSoundEffectVolume;

        private AudioController _audioController;
        private bool _isActivated;

        protected override void Awake()
        {
            base.Awake();
            _audioController = FindFirstObjectByType<AudioController>();
        }

        public override void Die()
        {
            base.Die();
            Destroy(gameObject, DestroyDelay);
        }

        private void OnDestroy()
        {
            if (_isActivated)
            {
                _audioController?.StopBossMusic();
            }
        }

        private void PlayAttack() => _audioController?.PlayOneShotWithVolume(_attackSound, _soundEffectVolume);

        private void PlayAttackMiss() => _audioController?.PlayOneShotWithVolume(_attackMissSound, _soundEffectVolume * AttackMissSoundVolumeMultiplier);

        private void PlayTakeDamage() => _audioController?.PlayOneShotWithVolume(_takeDamageSound, _soundEffectVolume);

        private void PlayDeath() => _audioController?.PlayOneShotWithVolume(_deathSound, _soundEffectVolume);

        private void PlayDodge() => _audioController?.PlayOneShotWithVolume(_dodgeSound, _soundEffectVolume);
    }
}
