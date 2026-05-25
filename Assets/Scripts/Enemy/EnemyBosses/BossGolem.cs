using GeneralLogicEnemies;
using Shared.Damage;
using UnityEngine;

namespace MiddleBossLogic
{
    [RequireComponent(typeof(Animator))]
    public sealed class BossGolem : Entity, IDamageable
    {
        private const float DestroyDelay = 1f;
        private const float DefaultSoundEffectVolume = 1f;

        [SerializeField] private AudioClip _attackSound;
        [SerializeField] private AudioClip _attackMissSound;
        [SerializeField] private AudioClip _takeDamageSound;
        [SerializeField] private AudioClip _deathSound;
        [SerializeField] private float _soundEffectVolume = DefaultSoundEffectVolume;

        private AudioController _audioController;
        private bool _isActivated;

        protected override void Awake()
        {
            base.Awake();
            _audioController = FindFirstObjectByType<AudioController>();
        }

        private void OnDestroy()
        {
            if (_isActivated) _audioController?.StopBossMusic();
        }

        private void PlayAttack() => _audioController?.PlayOneShotWithVolume(_attackSound, _soundEffectVolume);
    }
}
