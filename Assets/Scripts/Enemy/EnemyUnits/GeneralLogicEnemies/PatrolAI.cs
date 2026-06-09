using UnityEngine;

namespace GeneralEnemyPatrolSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PatrolAI : MonoBehaviour
    {
        private const float ArrivalThreshold = 0.3f;
        private const float ChaseSpeedMultiplier = 1.5f;
        private const float BoxRotationAngle = 0f;

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
        private bool _isPlayerDead;

        public System.Action<Vector2> OnMoveDirectionChanged;
        public System.Action<bool> OnInAttackRange;

        public Vector2 CurrentDirection { get; private set; }
        public bool InAttackRange { get; private set; }

        private void Awake()
        {
            InitializeComponents();
            InitializePatrolPoint();
        }

        private void Update()
        {
            if (_isPlayerDead)
            {
                HandlePlayerDeadState();
                NotifyStateChanges();

                return;
            }

            if (_playerTransform == null)
            {
                SearchForPlayer();
            }

            if (_playerTransform != null)
            {
                HandlePlayerChase();
            }
            else
            {
                HandlePatrolState();
            }

            NotifyStateChanges();
        }

        private void OnDestroy()
        {
            UnsubscribeFromPlayerDeath();
        }

        public void ResetPlayerState()
        {
            _isPlayerDead = false;
        }

        private void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void InitializePatrolPoint()
        {
            if (_pointB != null)
            {
                _nextPatrolPoint = _pointB.position;
                CurrentDirection = (_nextPatrolPoint - transform.position).normalized;

                return;
            }

            _nextPatrolPoint = transform.position;
            CurrentDirection = Vector2.zero;
        }

        private void HandlePlayerDeadState()
        {
            ClearPlayerReference();

            InAttackRange = false;

            Patrol();
        }

        private void HandlePlayerChase()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            InAttackRange = IsPlayerInAttackBox();

            if (distanceToPlayer > _loseRadius)
            {
                ClearPlayerReference();
                InAttackRange = false;

                return;
            }

            if (InAttackRange)
            {
                StopMovement();
                CheckPlayerDeath();

                return;
            }

            MoveToward(_playerTransform.position, _patrolSpeed * ChaseSpeedMultiplier);
        }

        private void HandlePatrolState()
        {
            InAttackRange = false;

            Patrol();
        }

        private void Patrol()
        {
            if (_pointA == null || _pointB == null)
            {
                StopMovement();

                return;
            }

            if (Vector2.Distance(transform.position, _nextPatrolPoint) < ArrivalThreshold)
            {
                _nextPatrolPoint = _nextPatrolPoint == _pointA.position
                    ? _pointB.position
                    : _pointA.position;
            }

            MoveToward(_nextPatrolPoint, _patrolSpeed);
        }

        private void MoveToward(Vector3 target, float speed)
        {
            if (_rigidbody == null)
            {
                return;
            }

            Vector2 direction = (target - transform.position).normalized;

            if (TryOpenDoorInDirection(direction))
            {
                StopMovement();

                return;
            }

            _rigidbody.velocity = new Vector2(direction.x * speed, _rigidbody.velocity.y);
            CurrentDirection = direction;
        }

        private bool TryOpenDoorInDirection(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return false;
            }

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                direction,
                _doorCheckDistance,
                _doorLayerMask);

            IOpenable door = hit.collider != null
                ? hit.collider.GetComponent<IOpenable>()
                : null;

            if (door == null || door.IsClosed == false)
            {
                return false;
            }

            door.Open();

            return true;
        }

        private void StopMovement()
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector2.zero;
            }

            CurrentDirection = Vector2.zero;
        }

        private void SearchForPlayer()
        {
            Collider2D hit = Physics2D.OverlapCircle(
                transform.position,
                _visionRadius,
                _playerLayerMask);

            if (hit == null)
            {
                ClearPlayerReference();

                return;
            }

            SetPlayerReference(hit.transform);
        }

        private void SetPlayerReference(Transform playerTransform)
        {
            UnsubscribeFromPlayerDeath();

            _playerTransform = playerTransform;
            _playerHealthManager = _playerTransform.GetComponent<HealthManager>();

            if (_playerHealthManager == null)
            {
                _playerHealthManager = _playerTransform.GetComponentInParent<HealthManager>();
            }

            if (_playerHealthManager != null)
            {
                _playerHealthManager.OnDeath += HandlePlayerDeath;
            }
        }

        private void ClearPlayerReference()
        {
            UnsubscribeFromPlayerDeath();

            _playerTransform = null;
            _playerHealthManager = null;
        }

        private void UnsubscribeFromPlayerDeath()
        {
            if (_playerHealthManager != null)
            {
                _playerHealthManager.OnDeath -= HandlePlayerDeath;
            }
        }

        private bool IsPlayerInAttackBox()
        {
            Vector2 attackCenter = CalculateAttackCenter(_attackOffset);

            Collider2D playerInAttackRange = Physics2D.OverlapBox(
                attackCenter,
                _attackSize,
                BoxRotationAngle,
                _playerLayerMask);

            return playerInAttackRange != null;
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

            ClearPlayerReference();
        }

        private void NotifyStateChanges()
        {
            OnMoveDirectionChanged?.Invoke(CurrentDirection);
            OnInAttackRange?.Invoke(InAttackRange);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _visionRadius);

            Gizmos.color = Color.red;

            Vector2 attackCenter = CalculateAttackCenter(_attackOffset);

            Gizmos.DrawWireCube(attackCenter, _attackSize);
        }
    }
}