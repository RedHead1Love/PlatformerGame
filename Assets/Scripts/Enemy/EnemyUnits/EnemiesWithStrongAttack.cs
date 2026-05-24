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
        private const string NoCollisionLayer = "IgnoreRaycast";
        private const float DestroyDelay = 0.55f;
        private const int MaxRandomValue = 100;
        private const float BoxRotationAngle = 0f;
        private const float StrongAttackProbability = 30f;
        private const float DefaultSoundVolume = 1f;
        private const float MovementThreshold = 0.01f;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private int _defaultAttackDamage = 1;
        [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

        [Header("Special Attack Settings")]
        [SerializeField] private float _specialAttackCooldown = 5f;
        [SerializeField] private int _specialAttackDamage = 4;
        [SerializeField] private Vector2 _specialAttackOffset = new Vector2(1.8f, 0.8f);
        [SerializeField] private Vector2 _specialAttackSize = new Vector2(3.5f, 2.2f);

        [Header("Audio")]
        [SerializeField] private AudioClip _regularAttackSound;
        [SerializeField] private AudioClip _strongAttackSound;
        [SerializeField] private AudioClip _deathSound;

        private Animator _animator;
        private PatrolAI _patrolAI;
        private AudioController _audioController;
        private LayerMask _playerLayerMask;

        private float _attackTimer;
        private float _specialAttackTimer;
        private bool _isAttacking;
        private bool _inAttackRange;

        private enum AnimationState
        {
            Idle = 0,
            Walk = 1
        }

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            _patrolAI = GetComponent<PatrolAI>();
            _audioController = FindFirstObjectByType<AudioController>();

            _playerLayerMask = LayerMask.GetMask(PlayerLayerName);
        }

        private void OnEnable()
        {
            if (_patrolAI != null)
            {
                _patrolAI.OnInAttackRange += HandleAttackRange;
                _patrolAI.OnMoveDirectionChanged += HandleMovementDirectionChanged;
            }
        }

        private void OnDisable()
        {
            if (_patrolAI != null)
            {
                _patrolAI.OnInAttackRange -= HandleAttackRange;
                _patrolAI.OnMoveDirectionChanged -= HandleMovementDirectionChanged;
            }
        }

        private void Update()
        {
            if (IsDead)
            {
                return;
            }

            UpdateTimers();
            CheckAndPerformAttack();
        }

        private void UpdateTimers()
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= Time.deltaTime;
            }

            if (_specialAttackTimer > 0f)
            {
                _specialAttackTimer -= Time.deltaTime;
            }
        }

        private void HandleMovementDirectionChanged(Vector2 direction)
        {
            if (IsDead || _isAttacking)
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

        private void HandleAttackRange(bool inRange)
        {
            _inAttackRange = inRange;
        }

        private void CheckAndPerformAttack()
        {
            if (_inAttackRange && !_isAttacking && _attackTimer <= 0f)
            {
                PerformAttack();
            }
        }

        private void PerformAttack()
        {
            _isAttacking = true;
            _attackTimer = _attackCooldown;

            StopMovementAnimation();

            bool canPerformSpecial = _specialAttackTimer <= 0f;
            bool triggersSpecial = Random.Range(0, MaxRandomValue) < StrongAttackProbability;

            if (canPerformSpecial && triggersSpecial)
            {
                _specialAttackTimer = _specialAttackCooldown;
                _animator.SetTrigger("StrongAttack");
            }
            else
            {
                _animator.SetTrigger("Attack");
            }
        }

        private void StopMovementAnimation()
        {
            SetAnimationState(AnimationState.Idle);
        }

        private void SetAnimationState(AnimationState state)
        {
            if (_animator != null)
            {
                _animator.SetInteger("state", (int)state);
            }
        }

        public override void Die()
        {
            base.Die();

            if (_patrolAI != null)
            {
                _patrolAI.enabled = false;
            }

            if (_audioController != null && _deathSound != null)
            {
                _audioController.PlayOneShotWithVolume(_deathSound, DefaultSoundVolume);
            }

            gameObject.layer = LayerMask.NameToLayer(NoCollisionLayer);

            Destroy(gameObject, DestroyDelay);
        }


        public void AnimEvent_DealDamage()
        {
            ProcessDamage(_attackOffset, _attackSize, _defaultAttackDamage, _regularAttackSound);
        }

        public void AnimEvent_DealStrongDamage()
        {
            ProcessDamage(_specialAttackOffset, _specialAttackSize, _specialAttackDamage, _strongAttackSound);
        }

        public void AnimEvent_EndAttack()
        {
            _isAttacking = false;

            if (_patrolAI != null && _patrolAI.CurrentMoveDirection.magnitude > MovementThreshold)
            {
                SetAnimationState(AnimationState.Walk);
            }
            else
            {
                SetAnimationState(AnimationState.Idle);
            }
        }

        private void ProcessDamage(Vector2 offset, Vector2 size, int damage, AudioClip attackSound)
        {
            if (_audioController != null && attackSound != null)
            {
                _audioController.PlayOneShotWithVolume(attackSound, DefaultSoundVolume);
            }

            Vector2 boxCenter = (Vector2)transform.position + new Vector2(offset.x * Mathf.Sign(transform.localScale.x), offset.y);
            Collider2D hit = Physics2D.OverlapBox(boxCenter, size, BoxRotationAngle, _playerLayerMask);

            if (hit != null)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 boxCenter = (Vector2)transform.position + new Vector2(_attackOffset.x * Mathf.Sign(transform.localScale.x), _attackOffset.y);
            Gizmos.DrawWireCube(boxCenter, _attackSize);

            Gizmos.color = Color.magenta;
            Vector2 specialBoxCenter = (Vector2)transform.position + new Vector2(_specialAttackOffset.x * Mathf.Sign(transform.localScale.x), _specialAttackOffset.y);
            Gizmos.DrawWireCube(specialBoxCenter, _specialAttackSize);
        }
    }
}
