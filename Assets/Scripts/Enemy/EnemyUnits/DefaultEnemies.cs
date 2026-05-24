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
        private const float BoxRotationAngle = 0f;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private int _attackDamage = 1;

        [Header("Attack Box Settings")]
        [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

        [Header("Hurt Settings")]
        [SerializeField] private float _hurtInvulnerabilityDuration = 0.5f;

        private Animator _animator;
        private PatrolAI _patrolAI;

        private bool _isHurt;
        private bool _isAttacking;
        private float _nextAttackTime;
        private bool _isPlayerInRange;
        private LayerMask _playerLayerMask;

        private enum AnimationState
        {
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Hurt = 3,
            Death = 4
        }

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            _patrolAI = GetComponent<PatrolAI>();
            _playerLayerMask = LayerMask.GetMask(PlayerLayerName);
        }

        private void OnEnable()
        {
            if (_patrolAI != null)
            {
                _patrolAI.OnMoveDirectionChanged += HandleMovementDirectionChanged;
                _patrolAI.OnInAttackRange += HandleAttackRangeChanged;
            }
        }

        private void OnDisable()
        {
            if (_patrolAI != null)
            {
                _patrolAI.OnMoveDirectionChanged -= HandleMovementDirectionChanged;
                _patrolAI.OnInAttackRange -= HandleAttackRangeChanged;
            }
        }

        private void Update()
        {
            if (IsDead || _isHurt)
            {
                return;
            }

            CheckAttackCondition();
        }

        private void HandleMovementDirectionChanged(Vector2 direction)
        {
            if (IsDead || _isHurt || _isAttacking)
            {
                return;
            }

            if (direction.magnitude > MovementThreshold)
            {
                SetAnimationState(AnimationState.Walk);
                FlipSprite(direction.x);
            }
            else
            {
                SetAnimationState(AnimationState.Idle);
            }
        }

        private void FlipSprite(float directionX)
        {
            if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }

        private void HandleAttackRangeChanged(bool inRange)
        {
            _isPlayerInRange = inRange;
        }

        private void CheckAttackCondition()
        {
            if (_isPlayerInRange && !_isAttacking && Time.time >= _nextAttackTime)
            {
                StartAttack();
            }
        }

        private void StartAttack()
        {
            _isAttacking = true;

            SetAnimationState(AnimationState.Attack);
        }

        public override void TakeDamage(int amount)
        {
            if (IsDead || _isHurt)
            {
                return;
            }

            base.TakeDamage(amount);

            if (!IsDead)
            {
                TriggerHurtState();
            }
        }

        private void TriggerHurtState()
        {
            _isHurt = true;
            _isAttacking = false;

            SetAnimationState(AnimationState.Hurt);

            Invoke(nameof(EndHurtState), _hurtInvulnerabilityDuration);
        }

        private void EndHurtState()
        {
            if (IsDead)
            {
                return;
            }

            _isHurt = false;

            if (_isPlayerInRange)
            {
                SetAnimationState(AnimationState.Idle);
            }
            else if (_patrolAI != null && _patrolAI.CurrentMoveDirection.magnitude > MovementThreshold)
            {
                SetAnimationState(AnimationState.Walk);
            }
            else
            {
                SetAnimationState(AnimationState.Idle);
            }
        }

        public override void Die()
        {
            CancelInvoke(nameof(EndHurtState));

            _isHurt = false;
            _isAttacking = false;

            if (_patrolAI != null)
            {
                _patrolAI.enabled = false;
            }

            SetAnimationState(AnimationState.Death);

            base.Die();

            Destroy(gameObject, DestroyDelay);
        }

        private void SetAnimationState(AnimationState state)
        {
            if (_animator != null)
            {
                _animator.SetInteger("state", (int)state);
            }
        }

        public void AnimEvent_DealDamage()
        {
            Vector2 boxCenter = (Vector2)transform.position + new Vector2(_attackOffset.x * Mathf.Sign(transform.localScale.x), _attackOffset.y);
            Collider2D hit = Physics2D.OverlapBox(boxCenter, _attackSize, BoxRotationAngle, _playerLayerMask);

            if (hit != null)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(_attackDamage);
            }
        }

        public void AnimEvent_EndAttack()
        {
            _isAttacking = false;
            _nextAttackTime = Time.time + _attackCooldown;

            SetAnimationState(_isPlayerInRange ? AnimationState.Idle : AnimationState.Walk);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 boxCenter = (Vector2)transform.position + new Vector2(_attackOffset.x * Mathf.Sign(transform.localScale.x), _attackOffset.y);
            Gizmos.DrawWireCube(boxCenter, _attackSize);
        }
    }
}
