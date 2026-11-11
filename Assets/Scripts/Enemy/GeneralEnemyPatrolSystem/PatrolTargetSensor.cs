using Foxberry;
using Player;
using UnityEngine;

namespace GeneralEnemyPatrolSystem
{
    internal sealed class PatrolTargetSensor
    {
        private const float BoxRotationAngle = 0f;
        private readonly Transform _origin;
        private HealthManager _playerHealthManager;

        public PatrolTargetSensor(Transform origin)
        {
            _origin = origin;
        }

        public Transform Target { get; private set; }
        public bool IsDead { get; private set; }
        public void ResetDeath() => IsDead = false;

        public void SearchForPlayer(float radius, LayerMask players)
        {
            Collider2D hit = Physics2D.OverlapCircle(_origin.position, radius, players);

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

            Target = playerTransform;
            _playerHealthManager = Target.GetComponent<HealthManager>();

            if (_playerHealthManager == null)
            {
                _playerHealthManager = Target.GetComponentInParent<HealthManager>();
            }

            if (_playerHealthManager != null)
            {
                _playerHealthManager.OnDeath += OnPlayerDeath;
            }
        }

        public void ClearPlayerReference()
        {
            UnsubscribeFromPlayerDeath();

            Target = null;
            _playerHealthManager = null;
        }

        private void UnsubscribeFromPlayerDeath()
        {
            if (_playerHealthManager != null)
            {
                _playerHealthManager.OnDeath -= OnPlayerDeath;
            }
        }

        public bool IsPlayerInAttackBox(Vector2 offset, Vector2 size, LayerMask players, LayerMask doors)
        {
            Vector2 attackCenter = CalculateAttackCenter(_origin, offset);

            Collider2D playerInAttackRange =
                Physics2D.OverlapBox(attackCenter, size, BoxRotationAngle, players);

            if (playerInAttackRange == null)
            {
                return false;
            }

            Vector2 startPoint = _origin.position;
            Vector2 endPoint = playerInAttackRange.transform.position;

            RaycastHit2D hit = Physics2D.Linecast(startPoint, endPoint, doors);

            if (hit.collider != null)
            {
                IOpenable door = hit.collider.GetComponent<IOpenable>();

                if (door != null && door.IsClosed)
                {
                    return false;
                }
            }

            return true;
        }

        public static Vector2 CalculateAttackCenter(Transform origin, Vector2 offset)
        {
            float directionSign = Mathf.Sign(origin.localScale.x);

            if (Mathf.Approximately(directionSign, 0f))
            {
                directionSign = 1f;
            }

            return (Vector2)origin.position + new Vector2(directionSign * offset.x, offset.y);
        }

        public void CheckPlayerDeath()
        {
            if (_playerHealthManager != null && _playerHealthManager.CurrentHealth <= 0)
            {
                IsDead = true;
            }
        }

        private void OnPlayerDeath()
        {
            IsDead = true;

            ClearPlayerReference();
        }
    }
}
