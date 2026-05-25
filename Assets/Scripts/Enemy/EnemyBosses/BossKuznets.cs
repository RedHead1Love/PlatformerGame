using GeneralLogicEnemies;
using Shared.Damage;
using UnityEngine;

namespace HardBossLogic
{
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public sealed class BossKuznets : Entity, IDamageable
    {
        private const float DestroyDelay = 6.3f;

        [Header("Audio")]
        [SerializeField] private AudioController _audioController;
        [SerializeField] private float _soundEffectVolume = 1f;

        private bool _isAggro;

        private void OnDestroy()
        {
            if (_isAggro) _audioController?.StopBossMusic();
        }
    }
}
