using Shared.Damage;
using System.Collections;
using UnityEngine;

namespace Traps
{
    [RequireComponent(typeof(Animator), typeof(Collider2D))]
    public sealed class LightningTrap : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        private const float DefaultStrikeDelay = 0.5f;
        private const float DefaultCooldown = 2f;
        private const int DefaultDamage = 2;
        private const string TriggerAnimationParam = "Strike";

        [Header("Trap Settings")]
        [SerializeField] private int _damage = DefaultDamage;
        [SerializeField] private float _strikeDelay = DefaultStrikeDelay;
        [SerializeField] private float _cooldown = DefaultCooldown;

        [Header("Effects")]
        [SerializeField] private LightningTrapSoundController _soundController;

        private Animator _animator;
        private bool _isActive;
        private bool _isCooldown;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            if (_soundController == null)
            {
                _soundController = GetComponent<LightningTrapSoundController>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isCooldown && !_isActive && other.CompareTag(PlayerTag))
            {
                StartCoroutine(ActivateTrapCoroutine(other.gameObject));
            }
        }

        private IEnumerator ActivateTrapCoroutine(GameObject target)
        {
            _isActive = true;
            _isCooldown = true;

            _animator?.SetTrigger(TriggerAnimationParam);
            _soundController?.PlayLightningStrikeSound();

            yield return new WaitForSeconds(_strikeDelay);

            DealDamage(target);

            _isActive = false;

            yield return new WaitForSeconds(_cooldown);

            _isCooldown = false;
        }

        private void DealDamage(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            IDamageable damageable = target.GetComponent<IDamageable>() ?? target.GetComponentInParent<IDamageable>();
            
            damageable?.TakeDamage(_damage);
        }
    }
}
