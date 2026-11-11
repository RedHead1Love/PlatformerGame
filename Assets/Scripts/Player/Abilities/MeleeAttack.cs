using Foxberry;
using GeneralLogicEnemies;
using UnityEngine;

namespace Player.Abilities
{
    public sealed class MeleeAttack
    {
        private const float AngleMultiplier = 0.5f;
        private const int InstakillDamage = 9999;

        private readonly Transform _attackOrigin;
        private readonly LayerMask _enemyLayerMask;
        private readonly float _attackRange;
        private readonly float _attackAngleHalf;
        private readonly Hero _hero;

        private OnePunchManSystem _onePunchSystem;
        private DamageBonusService _damageBonusService;
        private bool _onePunchSystemChecked;

        public MeleeAttack(
            Transform attackOrigin,
            float attackRange,
            float attackAngle,
            LayerMask enemyLayerMask,
            Hero hero)
        {
            _attackOrigin = attackOrigin;
            _attackRange = attackRange;
            _enemyLayerMask = enemyLayerMask;
            _attackAngleHalf = attackAngle * AngleMultiplier;
            _hero = hero;
        }

        public bool Perform(int baseDamage, Vector2 attackDirection)
        {
            if (_attackOrigin == null)
            {
                return false;
            }

            InitializeDamageBonusService();

            Collider2D[] hitColliders =
                Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRange, _enemyLayerMask);

            bool hitConnected = false;
            OnePunchManSystem onePunchSystem = GetOnePunchSystem();

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider == null)
                {
                    continue;
                }

                Vector2 closestPointOnEnemy = hitCollider.ClosestPoint(_attackOrigin.position);

                if (IsWithinAttackAngle(closestPointOnEnemy, attackDirection) == false)
                {
                    continue;
                }

                Entity enemyEntity = hitCollider.GetComponent<Entity>();

                if (enemyEntity == null)
                {
                    enemyEntity = hitCollider.GetComponentInParent<Entity>();
                }

                if (enemyEntity == null)
                {
                    continue;
                }

                ApplyDamageToEnemy(enemyEntity, hitCollider.gameObject, baseDamage, onePunchSystem);

                hitConnected = true;
            }

            return hitConnected;
        }

        public void DrawGizmos()
        {
            if (_attackOrigin == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackOrigin.position, _attackRange);
        }

        private void ApplyDamageToEnemy(
            Entity enemyEntity,
            GameObject enemyObject,
            int baseDamage,
            OnePunchManSystem onePunchSystem)
        {
            bool wasInstakill =
                onePunchSystem != null && onePunchSystem.IsActive && onePunchSystem.CheckForInstakill(enemyEntity);

            if (wasInstakill)
            {
                enemyEntity.TakeDamage(InstakillDamage);

                return;
            }

            int finalDamage = _damageBonusService?.CalculateDamageWithBonuses(baseDamage, enemyObject) ?? baseDamage;

            enemyEntity.TakeDamage(finalDamage);
        }

        private void InitializeDamageBonusService()
        {
            if (_damageBonusService != null || _hero == null || _hero.AbilityManager == null)
            {
                return;
            }

            _damageBonusService = new DamageBonusService(_hero.AbilityManager);
        }

        private OnePunchManSystem GetOnePunchSystem()
        {
            if (_onePunchSystemChecked)
            {
                return _onePunchSystem;
            }

            if (_hero != null)
            {
                _onePunchSystem = _hero.GetComponent<OnePunchManSystem>();
            }

            _onePunchSystemChecked = true;

            return _onePunchSystem;
        }

        private bool IsWithinAttackAngle(Vector3 enemyPosition, Vector2 attackDirection)
        {
            Vector2 directionToEnemy = (enemyPosition - _attackOrigin.position).normalized;
            float angle = Vector2.Angle(attackDirection, directionToEnemy);

            return angle <= _attackAngleHalf;
        }
    }
}
