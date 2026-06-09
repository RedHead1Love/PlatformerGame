using GeneralEnemyPatrolSystem;
using GeneralLogicEnemies;
using Shared.Damage;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class EnemyCombatController : Entity, IDamageable
{
    private const float StrongAttackProbability = 0.5f;
    private const float AttackCooldownDuration = 2f;
    private const int RegularAttackAnimationCount = 4;
    private const float DamageAnimationDelay = 0.3f;
    private const float AttackCheckRadius = 1.5f;
    private const float HitAnimationDuration = 0.5f;
    private const string AnimationStateParameterName = "state";
    private const string PlayerLayerName = "Player";
    private const float BoxRotationAngle = 0f;
    private const float MovementThreshold = 0.01f;
    private const float DirectionThreshold = 0f;
    private const float MaxDetectionRadiusMultiplier = 1.5f;

    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 2;
    [SerializeField] private int _currentHealth = 2;

    [Header("Attack Settings")]
    [SerializeField] private int _attackDamage = 1;
    [SerializeField] private int _strongAttackDamage = 2;
    [SerializeField] private float _attackCooldown = AttackCooldownDuration;

    [Header("Attack Box Settings")]
    [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
    [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

    [Header("Detection Settings")]
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private LayerMask _playerLayerMask;

    [Header("Hurt Settings")]
    [SerializeField] private float _hurtInvulnerabilityDuration = 0.1f;

    [Header("Component References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private PatrolAI _patrolAI;
    [SerializeField] private EnemyAudioController _audioController;

    private Transform _playerTransform;
    private HealthManager _playerHealthManager;
    private bool _isAttacking;
    private bool _isTakingDamage;
    private float _lastAttackTime;
    private Vector2 _lastPosition;
    private bool _isMoving;
    private Coroutine _attackCoroutine;
    private Vector2 _facingDirection = Vector2.right;
    private bool _isHurt;
    private bool _isPlayerInRange;
    private EnemyAnimationState _currentAnimationState = EnemyAnimationState.Idle;

    public new int MaxLives => _maxHealth;
    public new bool IsAlive => _currentHealth > 0 && IsDead == false;

    protected override void Awake()
    {
        base.Awake();

        InitializeHealth();
        InitializeComponents();

        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (IsDead || _isHurt || _isAttacking)
        {
            return;
        }

        UpdateMovementState();
        UpdatePlayerDetection();
        UpdateCombatState();
        UpdateAnimationState();
        UpdateFacingDirection();
    }

    public override void TakeDamage(int amount)
    {
        if (IsDead || _isTakingDamage || amount <= 0)
        {
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        lives = _currentHealth;

        if (_currentHealth <= 0)
        {
            Die();

            return;
        }

        StartCoroutine(TakeDamageRoutine());
    }

    public override void Die()
    {
        if (IsDead)
        {
            return;
        }

        _audioController?.PlayDeathSound();

        base.Die();
    }

    private void InitializeHealth()
    {
        _currentHealth = _maxHealth;
        lives = _currentHealth;
    }

    private void InitializeComponents()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        if (_patrolAI == null)
        {
            _patrolAI = GetComponent<PatrolAI>();
        }

        if (_audioController == null)
        {
            _audioController = GetComponent<EnemyAudioController>();
        }
    }

    private void UpdateMovementState()
    {
        if (_rigidbody != null)
        {
            _isMoving = Mathf.Abs(_rigidbody.velocity.x) > MovementThreshold;

            return;
        }

        Vector2 currentPosition = transform.position;

        _isMoving = Vector2.Distance(currentPosition, _lastPosition) > MovementThreshold;
        _lastPosition = currentPosition;
    }

    private void UpdatePlayerDetection()
    {
        if (_playerTransform != null && _playerHealthManager != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            _isPlayerInRange = distanceToPlayer <= _detectionRadius * MaxDetectionRadiusMultiplier &&
                               _playerHealthManager.CurrentHealth > 0;

            return;
        }

        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            _detectionRadius,
            _playerLayerMask);

        if (playerCollider == null)
        {
            _isPlayerInRange = false;

            return;
        }

        _playerTransform = playerCollider.transform;
        _playerHealthManager = playerCollider.GetComponent<HealthManager>();

        if (_playerHealthManager == null)
        {
            _playerHealthManager = playerCollider.GetComponentInParent<HealthManager>();
        }

        _isPlayerInRange = true;
    }

    private void UpdateCombatState()
    {
        if (_isPlayerInRange == false || _playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer > AttackCheckRadius || Time.time < _lastAttackTime + _attackCooldown)
        {
            return;
        }

        StartAttack();
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;

        bool useStrongAttack = Random.value <= StrongAttackProbability;
        int animationIndex = useStrongAttack
            ? RegularAttackAnimationCount
            : Random.Range(0, RegularAttackAnimationCount);

        SetAnimationState((EnemyAnimationState)animationIndex);

        _attackCoroutine = StartCoroutine(AttackRoutine(useStrongAttack));
    }

    private IEnumerator AttackRoutine(bool useStrongAttack)
    {
        yield return new WaitForSeconds(DamageAnimationDelay);

        int damage = useStrongAttack ? _strongAttackDamage : _attackDamage;
        bool hitConnected = ApplyAttackDamage(damage);

        PlayAttackSound(hitConnected, useStrongAttack);

        yield return new WaitForSeconds(HitAnimationDuration);

        _isAttacking = false;
    }

    private bool ApplyAttackDamage(int damage)
    {
        Vector2 attackCenter = CalculateAttackCenter();

        Collider2D playerCollider = Physics2D.OverlapBox(
            attackCenter,
            _attackSize,
            BoxRotationAngle,
            LayerMask.GetMask(PlayerLayerName));

        if (playerCollider == null)
        {
            return false;
        }

        IDamageable damageable = playerCollider.GetComponent<IDamageable>();

        if (damageable == null)
        {
            damageable = playerCollider.GetComponentInParent<IDamageable>();
        }

        if (damageable == null)
        {
            return false;
        }

        damageable.TakeDamage(damage);

        return true;
    }

    private Vector2 CalculateAttackCenter()
    {
        return (Vector2)transform.position + new Vector2(
            _attackOffset.x * _facingDirection.x,
            _attackOffset.y);
    }

    private IEnumerator TakeDamageRoutine()
    {
        _isTakingDamage = true;
        _isHurt = true;

        _audioController?.PlayHurtSound();
        SetAnimationState(EnemyAnimationState.Hurt);

        if (_patrolAI != null)
        {
            _patrolAI.enabled = false;
        }

        yield return new WaitForSeconds(_hurtInvulnerabilityDuration);

        if (_patrolAI != null && IsDead == false)
        {
            _patrolAI.enabled = true;
        }

        _isHurt = false;
        _isTakingDamage = false;
    }

    private void UpdateAnimationState()
    {
        if (_animator == null || _isAttacking || _isHurt)
        {
            return;
        }

        EnemyAnimationState targetState = _isMoving
            ? EnemyAnimationState.Walk
            : EnemyAnimationState.Idle;

        SetAnimationState(targetState);
    }

    private void UpdateFacingDirection()
    {
        if (_playerTransform != null && _isPlayerInRange)
        {
            float directionToPlayer = Mathf.Sign(_playerTransform.position.x - transform.position.x);

            if (Mathf.Abs(directionToPlayer) > DirectionThreshold)
            {
                _facingDirection = new Vector2(directionToPlayer, 0f);
                transform.localScale = new Vector3(directionToPlayer, 1f, 1f);
            }

            return;
        }

        if (_rigidbody == null || Mathf.Abs(_rigidbody.velocity.x) <= DirectionThreshold)
        {
            return;
        }

        float movementDirection = Mathf.Sign(_rigidbody.velocity.x);

        _facingDirection = new Vector2(movementDirection, 0f);
        transform.localScale = new Vector3(movementDirection, 1f, 1f);
    }

    private void SetAnimationState(EnemyAnimationState state)
    {
        if (_currentAnimationState == state)
        {
            return;
        }

        _currentAnimationState = state;

        if (_animator != null)
        {
            _animator.SetInteger(AnimationStateParameterName, (int)state);
        }
    }

    private void PlayAttackSound(bool hitConnected, bool strongAttack)
    {
        if (_audioController == null)
        {
            return;
        }

        if (strongAttack)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(CalculateAttackCenter(), _attackSize);
    }
}

public enum EnemyAnimationState
{
    Idle = 0,
    Attack1 = 1,
    Attack2 = 2,
    Attack3 = 3,
    Attack4 = 4,
    StrongAttack = 5,
    Hurt = 6,
    Walk = 7,
    Death = 8
}