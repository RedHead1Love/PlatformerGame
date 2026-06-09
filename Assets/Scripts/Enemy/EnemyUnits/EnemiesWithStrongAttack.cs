using GeneralEnemyPatrolSystem;
using GeneralLogicEnemies;
using Shared.Damage;
using UnityEngine;

namespace EnemyLogicWithEnhancedStrike
{
    [RequireComponent(typeof(Animator))]
    public sealed class EnemiesWithStrongAttack : Entity, IDamageable
    {
        private const string PlayerLayerName = "Player";
        private const string DefaultLayer = "Enemy";
        private const string NoCollisionLayer = "IgnoreRaycast";

        private const float DestroyDelay = 0.55f;
        private const float MovementThreshold = 0.01f;
        private const int MaxRandomValue = 100;
        private const float BoxRotationAngle = 0f;
        private const int HealthThreshold = 0;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private int _defaultAttackDamage = 1;

        [Header("Special Attack Settings")]
        [SerializeField] private float _specialAttackCooldown = 5f;
        [SerializeField] private int _specialAttackDamage = 4;
        [SerializeField] private Vector2 _specialAttackOffset = new Vector2(1.8f, 0.8f);
        [SerializeField] private Vector2 _specialAttackSize = new Vector2(3.5f, 2.2f);

        [Header("Attack Box Settings")]
        [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

        [Header("Attack Timing")]
        [SerializeField] private float _attackDuration = 2.1f;
        [SerializeField] private float _attackPause = 0.4f;

        [Header("Special Attack Timing")]
        [SerializeField] private float _specialAttackDuration = 2.8f;
        [SerializeField] private float _specialAttackPause = 0.5f;

        [Header("Hurt Settings")]
        [SerializeField] private float _hurtInvulnerabilityDuration = 0.2f;

        [Header("Attack Weights")]
        [SerializeField] private int _normalAttackWeight = 70;
        [SerializeField] private int _specialAttackWeight = 30;

        private Animator _animator;
        private PatrolAI _patrolAI;
        private EnemyAudioController _audioController;
        private Collider2D _enemyCollider;

        private bool _isPlayerInRange;
        private bool _isAttackInProgress;
        private bool _isHurt;
        private bool _isDead;

        private float _nextAttackTime;
        private float _nextSpecialAttackTime;
        private float _hurtTimer;

        public float AttackCooldown => _attackCooldown;
        public float SpecialAttackCooldown => _specialAttackCooldown;
        public int SpecialDamage => _specialAttackDamage;
        public float AttackDuration => _attackDuration;
        public float AttackPause => _attackPause;

        protected override void Awake()
        {
            base.Awake();

            InitializeComponents();
            SubscribeToPatrolEvents();
        }

        private void Update()
        {
            if (_isHurt)
            {
                UpdateHurtState();

                return;
            }

            if (_isDead || _isAttackInProgress)
            {
                return;
            }

            if (_isPlayerInRange && Time.time >= _nextAttackTime)
            {
                TryStartAttack();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromPatrolEvents();
        }

        public void EnableCollider()
        {
            if (_enemyCollider != null)
            {
                gameObject.layer = LayerMask.NameToLayer(DefaultLayer);
            }
        }

        public void DisableCollider()
        {
            if (_enemyCollider != null)
            {
                gameObject.layer = LayerMask.NameToLayer(NoCollisionLayer);
            }
        }

        public void OnAttack()
        {
            PerformAttack(_attackOffset, _attackSize, _defaultAttackDamage, isSpecialAttack: false);
        }

        public void OnSpecialAttack()
        {
            PerformAttack(_specialAttackOffset, _specialAttackSize, _specialAttackDamage, isSpecialAttack: true);
        }

        public void OnAttackAnimationEnd()
        {
            _isAttackInProgress = false;

            if (_isHurt || _isDead)
            {
                return;
            }

            UpdateAnimation(_patrolAI != null ? _patrolAI.CurrentDirection : Vector2.zero);
        }

        public override void TakeDamage(int amount)
        {
            if (_isHurt || _isDead || amount <= 0)
            {
                return;
            }

            base.TakeDamage(amount);

            if (lives <= HealthThreshold)
            {
                Die();

                return;
            }

            StartHurtState();
        }

        public override void Die()
        {
            if (_isDead)
            {
                return;
            }

            _isDead = true;
            _isHurt = false;
            _isAttackInProgress = false;

            _audioController?.PlayDeathSound();

            if (_patrolAI != null)
            {
                _patrolAI.enabled = false;
            }

            SetAnimationState(AnimationState.Death);
            PlayAnimation("Death");

            base.Die();

            Destroy(gameObject, DestroyDelay);
        }

        private void InitializeComponents()
        {
            _animator = GetComponent<Animator>();
            _patrolAI = GetComponent<PatrolAI>();
            _audioController = GetComponent<EnemyAudioController>();
            _enemyCollider = GetComponent<Collider2D>();
        }

        private void SubscribeToPatrolEvents()
        {
            if (_patrolAI == null)
            {
                return;
            }

            _patrolAI.OnMoveDirectionChanged += HandleMovementDirectionChanged;
            _patrolAI.OnInAttackRange += HandleAttackRangeChanged;
        }

        private void UnsubscribeFromPatrolEvents()
        {
            if (_patrolAI == null)
            {
                return;
            }

            _patrolAI.OnMoveDirectionChanged -= HandleMovementDirectionChanged;
            _patrolAI.OnInAttackRange -= HandleAttackRangeChanged;
        }

        private void UpdateHurtState()
        {
            _hurtTimer -= Time.deltaTime;

            if (_hurtTimer <= 0f)
            {
                EndHurtState();
            }
        }

        private void TryStartAttack()
        {
            if (_isAttackInProgress || _isHurt || _isDead)
            {
                return;
            }

            bool canUseSpecialAttack = Time.time >= _nextSpecialAttackTime;
            bool shouldUseSpecialAttack = canUseSpecialAttack &&
                                          Random.Range(0, MaxRandomValue) < _specialAttackWeight;

            if (shouldUseSpecialAttack)
            {
                StartSpecialAttack();
            }
            else
            {
                StartNormalAttack();
            }
        }

        private void StartNormalAttack()
        {
            _isAttackInProgress = true;
            _nextAttackTime = Time.time + _attackCooldown + _attackPause;

            PlayAnimation("StrongAttack");
            SetAnimationState(AnimationState.Attack);
        }

        private void StartSpecialAttack()
        {
            _isAttackInProgress = true;
            _nextSpecialAttackTime = Time.time + _specialAttackCooldown + _specialAttackPause;
            _nextAttackTime = Time.time + _attackCooldown;

            PlayAnimation("StrongAttack");
            SetAnimationState(AnimationState.StrongAttack);
        }

        private void PerformAttack(Vector2 offset, Vector2 size, int damage, bool isSpecialAttack)
        {
            Vector2 attackCenter = CalculateAttackCenter(offset);
            bool hitConnected = CheckForHit(attackCenter, size, damage);

            PlayAttackSound(hitConnected, isSpecialAttack);
        }

        private Vector2 CalculateAttackCenter(Vector2 offset)
        {
            float directionSign = Mathf.Sign(transform.localScale.x);

            if (Mathf.Approximately(directionSign, 0f))
            {
                directionSign = 1f;
            }

            return (Vector2)transform.position + new Vector2(directionSign * offset.x, offset.y);
        }

        private bool CheckForHit(Vector2 center, Vector2 size, int damage)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                center,
                size,
                BoxRotationAngle,
                LayerMask.GetMask(PlayerLayerName));

            bool hitConnected = false;

            foreach (Collider2D hit in hits)
            {
                if (hit == null)
                {
                    continue;
                }

                IDamageable damageable = hit.GetComponent<IDamageable>();

                if (damageable == null)
                {
                    damageable = hit.GetComponentInParent<IDamageable>();
                }

                if (damageable == null)
                {
                    continue;
                }

                damageable.TakeDamage(damage);
                hitConnected = true;
            }

            return hitConnected;
        }

        private void PlayAttackSound(bool hitConnected, bool isSpecialAttack)
        {
            if (_audioController == null)
            {
                return;
            }

            if (isSpecialAttack)
            {
                if (hitConnected)
                {
                    _audioController.PlaySpecialAttackHitSound();
                }
                else
                {
                    _audioController.PlaySpecialAttackMissSound();
                }

                return;
            }

            if (hitConnected)
            {
                _audioController.PlayAttackHitSound();
            }
            else
            {
                _audioController.PlayAttackMissSound();
            }
        }

        private void StartHurtState()
        {
            _isHurt = true;
            _hurtTimer = _hurtInvulnerabilityDuration;
            _isAttackInProgress = false;

            _audioController?.PlayHurtSound();

            if (_patrolAI != null)
            {
                _patrolAI.enabled = false;
            }

            PlayAnimation("Hurt");
            SetAnimationState(AnimationState.Hurt);
        }

        private void EndHurtState()
        {
            _isHurt = false;

            if (_patrolAI != null && _isDead == false)
            {
                _patrolAI.enabled = true;
            }

            if (_isDead)
            {
                return;
            }

            PlayAnimation("Idle");
            UpdateAnimation(_patrolAI != null ? _patrolAI.CurrentDirection : Vector2.zero);
        }

        private void HandleAttackRangeChanged(bool isInRange)
        {
            _isPlayerInRange = isInRange;

            if (_isHurt || _isDead)
            {
                return;
            }

            if (isInRange && _isAttackInProgress == false && Time.time >= _nextAttackTime)
            {
                TryStartAttack();
            }
        }

        private void HandleMovementDirectionChanged(Vector2 direction)
        {
            if (_isHurt || _isDead || _isAttackInProgress)
            {
                return;
            }

            UpdateAnimation(direction);
        }

        private void UpdateAnimation(Vector2 direction)
        {
            if (_isHurt || _isDead || _isAttackInProgress)
            {
                return;
            }

            AnimationState animationState = direction.magnitude > MovementThreshold
                ? AnimationState.Walk
                : AnimationState.Idle;

            SetAnimationState(animationState);
            UpdateFacingDirection(direction);
        }

        private void UpdateFacingDirection(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) <= 0f)
            {
                return;
            }

            float scaleSign = Mathf.Sign(direction.x);

            transform.localScale = new Vector3(
                scaleSign * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }

        private void PlayAnimation(string animationName)
        {
            if (_animator == null)
            {
                return;
            }

            _animator.Play(animationName, 0, 0f);
        }

        private void SetAnimationState(AnimationState state)
        {
            if (_animator != null)
            {
                _animator.SetInteger("state", (int)state);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(CalculateAttackCenter(_attackOffset), _attackSize);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(CalculateAttackCenter(_specialAttackOffset), _specialAttackSize);
        }

        private enum AnimationState
        {
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Hurt = 3,
            Death = 4,
            StrongAttack = 5
        }
    }
}