using GeneralLogicEnemies;
using Shared.Damage;
using UnityEngine;

namespace HardBossLogic
{
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public sealed class BossStoneGolem : Entity, IDamageable
    {
        private const float DestroyDelay = 3f;
        private const float MoveSoundVolumeMultiplier = 0.4f;
        private const float AttackMissSoundVolumeMultiplier = 0.7f;
        private const float DefaultAttackCooldown = 2f;
        private const float MeleeAttackRange = 2f;
        private const float LaserAttackRange = 8f;
        private const float AggroRange = 12f;
        private const float AttackBoxRadius = 1.5f;
        private const int MeleeDamage = 2;
        private const float LaserDuration = 0.5f;

        [Header("Combat Settings")]
        [SerializeField] private LayerMask _playerLayerMask;
        [SerializeField] private Transform _laserFirePoint;
        [SerializeField] private float _moveSpeed = 2.5f;
        [SerializeField] private GameObject _laserPrefab;

        [Header("Audio References")]
        [SerializeField] private AudioClip _footstepSound;
        [SerializeField] private AudioClip _meleeAttackSound;
        [SerializeField] private AudioClip _laserFireSound;
        [SerializeField] private AudioClip _deathSound;
        [SerializeField] private AudioClip _bossMusic;

        private AudioController _audioController;
        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private Transform _playerTransform;

        private bool _isAggro;
        private bool _isAttacking;
        private float _soundEffectVolume = 1f;
        private float _attackCooldownTimer;

        protected override void Awake()
        {
            base.Awake();

            _audioController = FindFirstObjectByType<AudioController>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            FindPlayer();
        }

        private void Update()
        {
            if (IsDead || _playerTransform == null)
            {
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            CheckAggroState(distanceToPlayer);

            if (!_isAggro || _isAttacking)
            {
                return;
            }

            HandleCombatLogic(distanceToPlayer);
        }

        private void FixedUpdate()
        {
            if (IsDead || !_isAggro || _isAttacking || _playerTransform == null)
            {
                StopMovement();
                return;
            }

            MoveTowardsPlayer();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        private void CheckAggroState(float distanceToPlayer)
        {
            if (!_isAggro && distanceToPlayer <= AggroRange)
            {
                _isAggro = true;
                _audioController?.PlayBossMusic(_bossMusic); 
            }
        }

        private void HandleCombatLogic(float distanceToPlayer)
        {
            _attackCooldownTimer -= Time.deltaTime;

            if (_attackCooldownTimer <= 0f)
            {
                if (distanceToPlayer <= MeleeAttackRange)
                {
                    TriggerMeleeAttack();
                }
                else if (distanceToPlayer <= LaserAttackRange)
                {
                    TriggerLaserAttack();
                }
            }
        }

        private void MoveTowardsPlayer()
        {
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            _rigidbody.velocity = new Vector2(direction.x * _moveSpeed, _rigidbody.velocity.y);

            FlipTowardsPlayer(direction.x);

            if (_animator != null)
            {
                _animator.SetBool("IsMoving", true);
            }
        }

        private void StopMovement()
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
            }

            if (_animator != null)
            {
                _animator.SetBool("IsMoving", false);
            }
        }

        private void FlipTowardsPlayer(float directionX)
        {
            if ((directionX > 0 && transform.localScale.x < 0) || (directionX < 0 && transform.localScale.x > 0))
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }

        private void TriggerMeleeAttack()
        {
            _isAttacking = true;
            _attackCooldownTimer = DefaultAttackCooldown;
            _animator?.SetTrigger("MeleeAttack");
            StopMovement();
        }

        private void TriggerLaserAttack()
        {
            _isAttacking = true;
            _attackCooldownTimer = DefaultAttackCooldown;
            _animator?.SetTrigger("LaserAttack");
            StopMovement();
        }

        public override void Die()
        {
            base.Die();

            StopMovement();

            _audioController?.PlayOneShotWithVolume(_deathSound, _soundEffectVolume);

            if (_isAggro)
            {
                _audioController?.StopBossMusic();
            }

            Destroy(gameObject, DestroyDelay);
        }

        private void OnDestroy()
        {
            if (_isAggro)
            {
                _audioController?.StopBossMusic();
            }
        }

        public void AnimEvent_PlayFootstep()
        {
            _audioController?.PlayOneShotWithVolume(_footstepSound, _soundEffectVolume * MoveSoundVolumeMultiplier);
        }

        public void AnimEvent_DealMeleeDamage()
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, AttackBoxRadius, _playerLayerMask);

            if (hit != null)
            {
                _audioController?.PlayOneShotWithVolume(_meleeAttackSound, _soundEffectVolume);

                IDamageable damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(MeleeDamage);
            }
            else
            {
                _audioController?.PlayOneShotWithVolume(_meleeAttackSound, _soundEffectVolume * AttackMissSoundVolumeMultiplier);
            }
        }

        public void AnimEvent_FireLaser()
        {
            _audioController?.PlayOneShotWithVolume(_laserFireSound, _soundEffectVolume);

            if (_laserPrefab != null && _laserFirePoint != null)
            {
                GameObject laser = Instantiate(_laserPrefab, _laserFirePoint.position, _laserFirePoint.rotation);

                laser.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1f, 1f);

                Destroy(laser, LaserDuration);
            }
        }

        public void AnimEvent_EndAttack()
        {
            _isAttacking = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AggroRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, MeleeAttackRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, LaserAttackRange);
        }
    }
}
