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

        public float AttackCooldown => _attackCooldown;
        public float AttackDuration => _attackDuration;
        public float AttackPause => _attackPause;
        public Vector2 AttackOffset => _attackOffset;
        public Vector2 AttackSize => _attackSize;
        public int AttackDamage => _attackDamage;

        protected override void Awake()
        {
            base.Awake();

            InitializeComponents();
            SubscribeToPatrolEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromPatrolEvents();
        }

        private void Update()
        {
            if (IsDead || _isHurt || _isAttacking)
            {
                return;
            }

            if (_isPlayerInRange && Time.time >= _nextAttackTime)
            {
                StartAttack();
            }
        }

        public override void TakeDamage(int amount)
        {
            if (_isHurt || IsDead)
            {
                return;
            }

            base.TakeDamage(amount);

            if (lives > HealthThreshold)
            {
                StartHurtState();
                PlayHurtSound();
            }
        }

        public override void Die()
        {
            if (IsDead)
            {
                return;
            }

            PlayDeathSound();

            base.Die();

            Destroy(gameObject, DestroyDelay);
        }

        private void InitializeComponents()
        {
            _animator = GetComponent<Animator>();
            _patrolAI = GetComponent<PatrolAI>();
            _audioController = GetComponent<EnemyAudioController>();
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

        public void OnAttack()
        {
            ApplyAttackDamage();
        }

        public void OnAttackAnimationEnd()
        {
            FinishAttack();
        }

        public void OnAttackEnd()
        {
            FinishAttack();
        }

        private void OnAttackEnded()
        {
            FinishAttack();
        }

        private void FinishAttack()
        {
            _isAttacking = false;

            SetAnimationState(_isPlayerInRange ? AnimationState.Idle : AnimationState.Walk);
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _nextAttackTime = Time.time + _attackDuration + _attackPause + _attackCooldown;

            SetAnimationState(AnimationState.Attack);
            Invoke(nameof(OnAttackEnded), _attackDuration);
        }

        private void ApplyAttackDamage()
        {
            FacePlayerIfPossible();

            Vector2 attackCenter = CalculateAttackCenter();

            Collider2D playerCollider = Physics2D.OverlapBox(
                attackCenter,
                _attackSize,
                BoxRotationAngle,
                LayerMask.GetMask(PlayerLayerName));

            if (playerCollider == null)
            {
                _audioController?.PlayAttackMissSound();

                return;
            }

            IDamageable damageable = playerCollider.GetComponent<IDamageable>();

            if (damageable == null)
            {
                damageable = playerCollider.GetComponentInParent<IDamageable>();
            }

            if (damageable == null)
            {
                _audioController?.PlayAttackMissSound();

                return;
            }

            damageable.TakeDamage(_attackDamage);

            _audioController?.PlayAttackHitSound();
        }

        private void FacePlayerIfPossible()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                return;
            }

            float directionToPlayer = player.transform.position.x - transform.position.x;

            if (Mathf.Abs(directionToPlayer) <= DirectionThreshold)
            {
                return;
            }

            UpdateFacingDirection(directionToPlayer);
        }

        private Vector2 CalculateAttackCenter()
        {
            float direction = transform.localScale.x >= DirectionThreshold ? 1f : -1f;

            Vector2 offset = new Vector2(_attackOffset.x * direction, _attackOffset.y);

            return (Vector2)transform.position + offset;
        }

        private void StartHurtState()
        {
            const string HurtAnimationName = "Hurt";
            const string StateParameterName = "state";
            const int FullBodyAnimationLayer = -1;
            const float ResetToAnimationStart = 0f;

            _wasPlayerInRangeBeforeHurt = _isPlayerInRange;
            _isHurt = true;

            if (_patrolAI != null)
            {
                _patrolAI.enabled = false;
            }

            if (_animator != null)
            {
                _animator.Play(HurtAnimationName, FullBodyAnimationLayer, ResetToAnimationStart);
                _animator.SetInteger(StateParameterName, (int)AnimationState.Hurt);
            }

            Invoke(nameof(EndHurtState), _hurtInvulnerabilityDuration);
        }

        private void EndHurtState()
        {
            _isHurt = false;
            _isPlayerInRange = _wasPlayerInRangeBeforeHurt;

            if (_patrolAI != null && IsDead == false)
            {
                _patrolAI.enabled = true;
            }

            SetAnimationState(_isPlayerInRange ? AnimationState.Idle : AnimationState.Walk);
        }

        private void HandleMovementDirectionChanged(Vector2 direction)
        {
            if (_isHurt || _isAttacking)
            {
                return;
            }

            if (Mathf.Abs(direction.x) <= DirectionThreshold)
            {
                return;
            }

            UpdateFacingDirection(direction.x);
            SetAnimationState(AnimationState.Walk);
        }

        private void UpdateFacingDirection(float directionX)
        {
            float directionSign = Mathf.Sign(directionX);

            transform.localScale = new Vector3(
                directionSign * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }

        private void HandleAttackRangeChanged(bool isInRange)
        {
            _isPlayerInRange = isInRange;

            if (_isHurt || _isAttacking)
            {
                return;
            }

            SetAnimationState(isInRange ? AnimationState.Idle : AnimationState.Walk);
        }

        private void SetAnimationState(AnimationState state)
        {
            if (_animator != null)
            {
                _animator.SetInteger("state", (int)state);
            }
        }

        private void PlayHurtSound()
        {
            _audioController?.PlayHurtSound();
        }

        private void PlayDeathSound()
        {
            _audioController?.PlayDeathSound();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(CalculateAttackCenter(), _attackSize);
        }
    }

    public enum AnimationState
    {
        Idle = 0,
        Walk = 1,
        Attack = 2,
        Hurt = 3,
        Death = 4
    }
}