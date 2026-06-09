using UnityEngine;

namespace GeneralEnemyPatrolSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PatrolAI : MonoBehaviour
    {
        private const float ArrivalThreshold = 0.3f;
        private const float ChaseSpeedMultiplier = 1.5f;
        private const float BoxRotationAngle = 0f;
        private const int LayerNotFound = -1;

        [Header("Patrol Settings")]
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField] private float _patrolSpeed = 2f;

        [Header("Chase Settings")]
        [SerializeField] private float _visionRadius = 5f;
        [SerializeField] private float _loseRadius = 5f;
        [SerializeField] private LayerMask _playerLayerMask;

        [Header("Attack Box Settings")]
        [SerializeField] private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField] private Vector2 _attackSize = new Vector2(2.5f, 1.6f);

        [Header("Door Interaction")]
        [SerializeField] private float _doorCheckDistance = 1.2f;
        [SerializeField] private LayerMask _doorLayerMask;

        private Rigidbody2D _rigidbody;
        private Transform _playerTransform;
        private HealthManager _playerHealthManager;

        private Vector3 _nextPatrolPoint;
        private bool _isPlayerDead = false;

        public event System.Action<Vector2> OnMoveDirectionChanged;
        public event System.Action<bool> OnInAttackRange;

        public Vector2 CurrentMoveDirection { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _nextPatrolPoint = _pointA.position;
        }

        private void Update()
        {
            CheckPlayerDeath();

            if (_isPlayerDead)
            {
                Patrol();
                return;
            }

            if (_playerTransform == null)
            {
                SearchForPlayer();
                Patrol();
            }
            else
            {
                ChaseOrAttackPlayer();
            }
        }

        private void Patrol()
        {
            OnInAttackRange?.Invoke(false);

            if (Vector2.Distance(transform.position, _nextPatrolPoint) < ArrivalThreshold)
            {
                _nextPatrolPoint = _nextPatrolPoint == _pointA.position ? _pointB.position : _pointA.position;
            }

            MoveTowards(_nextPatrolPoint, _patrolSpeed);
        }

        private void ChaseOrAttackPlayer()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer > _loseRadius)
            {
                _playerTransform = null;
                return;
            }

            if (IsPlayerInAttackBox())
            {
                StopMovement();
                OnInAttackRange?.Invoke(true);
            }
            else
            {
                OnInAttackRange?.Invoke(false);

                if (!IsFacingDoor())
                {
                    MoveTowards(_playerTransform.position, _patrolSpeed * ChaseSpeedMultiplier);
                }
                else
                {
                    StopMovement();
                }
            }
        }

        private void MoveTowards(Vector3 targetPosition, float speed)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            Vector2 velocity = new Vector2(direction.x * speed, _rigidbody.velocity.y);

            _rigidbody.velocity = velocity;
            CurrentMoveDirection = velocity;

            OnMoveDirectionChanged?.Invoke(velocity);
        }

        private bool IsPlayerInAttackBox()
        {
            Vector2 boxCenter = (Vector2)transform.position + new Vector2(_attackOffset.x * Mathf.Sign(transform.localScale.x), _attackOffset.y);
            Collider2D hit = Physics2D.OverlapBox(boxCenter, _attackSize, BoxRotationAngle, _playerLayerMask);

            return hit != null;
        }

        private bool IsFacingDoor()
        {
            Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _doorCheckDistance, _doorLayerMask);

            return hit.collider != null;
        }

        private void StopMovement()
        {
            _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
            CurrentMoveDirection = Vector2.zero;

            OnMoveDirectionChanged?.Invoke(Vector2.zero);
        }

        private void SearchForPlayer()
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, _visionRadius, _playerLayerMask);

            if (hit != null)
            {
                _playerTransform = hit.transform;
                _playerHealthManager = _playerTransform.GetComponent<HealthManager>();

                if (_playerHealthManager != null)
                {
                    _playerHealthManager.OnDeath += HandlePlayerDeath;
                }
            }
            else
            {
                _playerTransform = null;
                _playerHealthManager = null;
            }
        }

        private void CheckPlayerDeath()
        {
            if (_playerHealthManager != null && _playerHealthManager.CurrentHealth <= 0)
            {
                _isPlayerDead = true;
            }
        }

        private void HandlePlayerDeath()
        {
            _isPlayerDead = true;
            _playerTransform = null;

            if (_playerHealthManager != null)
            {
                _playerHealthManager.OnDeath -= HandlePlayerDeath;
                _playerHealthManager = null;
            }
        }

        public void ResetPlayerState()
        {
            _isPlayerDead = false;
        }

        private void OnDestroy()
        {
            if (_playerHealthManager != null)
            {
                _playerHealthManager.OnDeath -= HandlePlayerDeath;
            }
        }
    }
}
