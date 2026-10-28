using UnityEngine;

namespace Player.Abilities
{
    public sealed class AbilityDamageBonus
    {
        private const float BonusMultiplier = 2f;
        private readonly AbilityManager _abilities;

        public AbilityDamageBonus(AbilityManager abilities)
        {
            _abilities = abilities;
        }

        public float GetDamageMultiplierForEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                return 1f;
            }

            EnemyTypeComponent enemyTypeComponent = GetEnemyTypeComponent(enemy);

            if (enemyTypeComponent == null)
            {
                return 1f;
            }

            return enemyTypeComponent.EnemyType switch
            {
                EnemyType.Swamp => _abilities.HasSwampDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Skeleton => _abilities.HasSkeletonDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Demon => _abilities.HasDemonDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Spider => _abilities.HasSpiderDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Zombie => _abilities.HasZombieDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Boss => _abilities.HasBossDamageBonus ? BonusMultiplier : 1f,
                _ => 1f
            };
        }

        public bool HasBonusForEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                return false;
            }

            EnemyTypeComponent enemyTypeComponent = GetEnemyTypeComponent(enemy);

            if (enemyTypeComponent == null)
            {
                return false;
            }

            return enemyTypeComponent.EnemyType switch
            {
                EnemyType.Swamp => _abilities.HasSwampDamageBonus,
                EnemyType.Skeleton => _abilities.HasSkeletonDamageBonus,
                EnemyType.Demon => _abilities.HasDemonDamageBonus,
                EnemyType.Spider => _abilities.HasSpiderDamageBonus,
                EnemyType.Zombie => _abilities.HasZombieDamageBonus,
                EnemyType.Boss => _abilities.HasBossDamageBonus,
                _ => false
            };
        }

        private EnemyTypeComponent GetEnemyTypeComponent(GameObject enemy)
        {
            EnemyTypeComponent component = enemy.GetComponent<EnemyTypeComponent>();

            if (component == null)
            {
                component = enemy.GetComponentInParent<EnemyTypeComponent>();
            }

            return component;
        }
    }
}
