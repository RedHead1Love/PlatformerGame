using GeneralLogicEnemies;
using UnityEngine;

namespace Player.Abilities
{
    public sealed class MeleeAttack
    {
        private const float AngleMultiplier = 0.5f;

        private readonly Transform _attackOrigin;
        private readonly LayerMask _enemyLayerMask;
        private readonly float _attackRange;
        private readonly float _attackAngleHalf;
        private readonly Hero _hero;

        private OnePunchManSystem _onePunchSystem;
        private bool _onePunchSystemChecked;
        private DamageBonusService _damageBonusService;
        private bool _serviceInitialized;

        public MeleeAttack(Transform attackOrigin, float attackRange, float attackAngle, LayerMask enemyLayerMask, Hero hero)
        {
            _attackOrigin = attackOrigin;
            _attackRange = attackRange;
            _enemyLayerMask = enemyLayerMask;
            _attackAngleHalf = attackAngle * AngleMultiplier;
            _hero = hero;
        }

        private void InitializeDamageBonusService()
        {
            if (_serviceInitialized)
            {
                return;
            }

            if (_hero == null || _hero.AbilityManager == null)
            {
                return;
            }

            _damageBonusService = new DamageBonusService(_hero.AbilityManager);
            _serviceInitialized = true;
        }

        public bool Perform(int baseDamage, Vector2 attackDirection)
        {
            InitializeDamageBonusService();

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRange, _enemyLayerMask);
            bool hasHitConnected = hitEnemies.Length > 0;

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                if (IsWithinAttackAngle(enemyCollider.transform.position, attackDirection))
                {
                    ProcessEnemyHit(enemyCollider.gameObject, baseDamage);
                }
            }

            return hasHitConnected;
        }

        private void ProcessEnemyHit(GameObject enemyObject, int baseDamage)
        {
            Entity enemyEntity = enemyObject.GetComponent<Entity>() ?? enemyObject.GetComponentInParent<Entity>();

            if (enemyEntity == null || !enemyEntity.IsAlive)
            {
                return;
            }

            if (CheckOnePunchKill(enemyEntity))
            {
                return;
            }

            int finalDamage = CalculateFinalDamage(baseDamage, enemyObject);
            enemyEntity.TakeDamage(finalDamage);
        }

        private bool CheckOnePunchKill(Entity enemyEntity)
        {
            if (!_onePunchSystemChecked)
            {
                _onePunchSystem = _hero?.GetComponent<OnePunchManSystem>();
                _onePunchSystemChecked = true;
            }

            return _onePunchSystem != null && _onePunchSystem.CheckForInstakill(enemyEntity);
        }

        private int CalculateFinalDamage(int baseDamage, GameObject enemyObject)
        {
            if (_damageBonusService != null)
            {
                int finalDamage = _damageBonusService.CalculateDamageWithBonuses(baseDamage, enemyObject);

                if (finalDamage > baseDamage)
                {
                    CreateBonusEffect(enemyObject, (float)finalDamage / baseDamage);
                }

                return finalDamage;
            }

            return baseDamage;
        }

        private void CreateBonusEffect(GameObject target, float multiplier)
        {
            float verticalOffset = 1.5f;
            string effectName = "DirectBonusEffect";
            float destroyDelay = 1f;
            int fontSize = 24;

            GameObject textObj = new GameObject(effectName);
            textObj.transform.position = target.transform.position + Vector3.up * verticalOffset;

            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = $"x{multiplier}!";
            textMesh.color = Color.magenta;
            textMesh.fontSize = fontSize;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.fontStyle = FontStyle.Bold;

            Object.Destroy(textObj, destroyDelay);
        }

        private bool IsWithinAttackAngle(Vector3 enemyPosition, Vector2 attackDirection)
        {
            Vector2 directionToEnemy = (enemyPosition - _attackOrigin.position).normalized;
            float angle = Vector2.Angle(attackDirection, directionToEnemy);

            return angle <= _attackAngleHalf;
        }

        public void DrawGizmos()
        {
            if (_attackOrigin != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_attackOrigin.position, _attackRange);
            }
        }
    }
}
