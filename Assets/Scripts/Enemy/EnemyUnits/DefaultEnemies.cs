using GeneralEnemyPatrolSystem;
using GeneralLogicEnemies;
using Shared.Damage;
using UnityEngine;

namespace EnemyLogicDefault
{
    [RequireComponent(typeof(Animator))]
    public sealed class DefaultEnemies : Entity, IDamageable
    {
        private const string PlayerLayerName = "Player";
        private const float DestroyDelay = 1f;
        private const float MovementThreshold = 0.01f;
        private const float DirectionThreshold = 0f;
        private const float BoxRotationAngle = 0f;
        private const int HealthThreshold = 0;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private int _attackDamage = 1;

        [Header("Attack Box Settings")]
        [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

        [Header("Attack Timing")]
        [SerializeField] private float _attackDuration = 0.5f;
        [SerializeField] private float _attackPause = 0.4f;

        [Header("Hurt Settings")]
        [SerializeField] private float _hurtInvulnerabilityDuration = 0.1f;

        private Animator _animator;
        private PatrolAI _patrolAI;
        private EnemyAudioController _audioController;

        private bool _isHurt;
        private bool _isAttacking;
        private float _nextAttackTime;
        private bool _isPlayerInRange;
        private bool _wasPlayerInRangeBeforeHurt;

        public override void Die()
        {
            CancelInvoke(nameof(EndHurtState));
            CancelInvoke(nameof(OnAttackEnded));

            if (_patrolAI != null)
            {
                _patrolAI.enabled = false;
            }

            enabled = false;

            SetAnimationState(AnimationState.Death);
            PlayDeathSound();

            base.Die();

            Destroy(gameObject, DestroyDelay);
        }

        private void OnDestroy()
        {
            if (_patrolAI != null)
            {
                _patrolAI.OnMoveDirectionChanged -= HandleMovementDirectionChanged;
                _patrolAI.OnInAttackRange -= HandleAttackRangeChanged;
            }
        }

        private void SetAnimationState(AnimationState state)
        {
            if (_animator != null)
            {
                _animator.SetInteger("state", (int)state);
            }
        }

        private void PlayDeathSound()
        {
            _audioController?.PlayDeathSound();
        }

        private void EndHurtState() { }
        private void OnAttackEnded() { }
        private void HandleMovementDirectionChanged(Vector2 direction) { }
        private void HandleAttackRangeChanged(bool inRange) { }

        private enum AnimationState
        {
            Idle,
            Walk,
            Attack,
            Hurt,
            Death
        }
    }
}
