using Foxberry;
using UnityEngine;

namespace GeneralEnemyPatrolSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PatrolAI : MonoBehaviour
    {
        private const float ArrivalThreshold = 0.3f;
        private const float ChaseSpeedMultiplier = 1.5f;

        [SerializeField]
        private Transform _pointA;
        [SerializeField]
        private Transform _pointB;
        [SerializeField]
        private float _patrolSpeed = 2f;
        [SerializeField]
        private float _visionRadius = 5f;
        [SerializeField]
        private float _loseRadius = 5f;
        [SerializeField]
        private LayerMask _playerLayerMask;
        [SerializeField]
        private Vector2 _attackOffset = new Vector2(1.5f, 0.8f);
        [SerializeField]
        private Vector2 _attackSize = new Vector2(2.5f, 1.6f);
        [SerializeField]
        private float _doorCheckDistance = 1.2f;
        [SerializeField]
        private LayerMask _doorLayerMask;
        private PatrolTargetSensor _sensor;
        private Rigidbody2D _rigidbody;
        private Vector3 _nextPatrolPoint;

        public event System.Action<Vector2> MovementDirectionChanged;

        public event System.Action<bool> AttackRangeChanged;

        public Vector2 CurrentDirection { get; private set; }

        public bool InAttackRange { get; private set; }

        public void ResetPlayerState()
        {
            _sensor.ResetDeath();
        }

        private void Awake()
        {
            _sensor = new PatrolTargetSensor(transform);
            InitializeComponents();
            InitializePatrolPoint();
        }

        private void Update()
        {
            if (_sensor.IsDead)
            {
                HandlePlayerDeadState();
                NotifyStateChanges();

                return;
            }

            if (_sensor.Target == null)
            {
                _sensor.SearchForPlayer(_visionRadius, _playerLayerMask);
            }

            if (_sensor.Target != null)
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
            _sensor?.ClearPlayerReference();
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
            _sensor.ClearPlayerReference();

            InAttackRange = false;

            Patrol();
        }

        private void HandlePlayerChase()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _sensor.Target.position);

            InAttackRange = _sensor.IsPlayerInAttackBox(_attackOffset, _attackSize, _playerLayerMask, _doorLayerMask);

            if (distanceToPlayer > _loseRadius)
            {
                _sensor.ClearPlayerReference();

                InAttackRange = false;

                return;
            }

            if (InAttackRange)
            {
                StopMovement();
                _sensor.CheckPlayerDeath();

                return;
            }

            MoveToward(_sensor.Target.position, _patrolSpeed * ChaseSpeedMultiplier);
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

            float distanceX = Mathf.Abs(transform.position.x - _nextPatrolPoint.x);

            if (distanceX < ArrivalThreshold)
            {
                _nextPatrolPoint = _nextPatrolPoint == _pointA.position ? _pointB.position : _pointA.position;
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

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _doorCheckDistance, _doorLayerMask);

            IOpenable door = hit.collider != null ? hit.collider.GetComponent<IOpenable>() : null;

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

        private void NotifyStateChanges()
        {
            MovementDirectionChanged?.Invoke(CurrentDirection);
            AttackRangeChanged?.Invoke(InAttackRange);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _visionRadius);

            Gizmos.color = Color.red;

            Vector2 attackCenter = PatrolTargetSensor.CalculateAttackCenter(transform, _attackOffset);

            Gizmos.DrawWireCube(attackCenter, _attackSize);
        }
    }
}
