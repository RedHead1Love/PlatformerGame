using GeneralLogicEnemies;
using UnityEngine;

namespace Player.Abilities
{
    public sealed class DamageBonusService
    {
        private const int SwampBonus = 2;
        private const int SkeletonBonus = 2;
        private const int DemonBonus = 2;
        private const int SpiderBonus = 2;
        private const int ZombieBonus = 2;
        private const int BossBonus = 3;

        private readonly AbilityManager _abilityManager;

        public DamageBonusService(AbilityManager abilityManager)
        {
            _abilityManager = abilityManager;
        }

        public int CalculateDamageWithBonuses(int baseDamage, GameObject enemyObject)
        {
            if (_abilityManager == null)
            {
                return baseDamage;
            }

            IEnemyTypeComponent enemyTypeComponent = enemyObject.GetComponent<IEnemyTypeComponent>();

            if (enemyTypeComponent == null)
            {
                return baseDamage;
            }

            return CalculateFinalDamage(baseDamage, enemyTypeComponent.EnemyType);
        }

        private int CalculateFinalDamage(int baseDamage, EnemyType type)
        {
            int finalDamage = baseDamage;

            if (type == EnemyType.Swamp && _abilityManager.HasSwampDamageBonus) finalDamage += SwampBonus;
            else if (type == EnemyType.Skeleton && _abilityManager.HasSkeletonDamageBonus) finalDamage += SkeletonBonus;
            else if (type == EnemyType.Demon && _abilityManager.HasDemonDamageBonus) finalDamage += DemonBonus;
            else if (type == EnemyType.Spider && _abilityManager.HasSpiderDamageBonus) finalDamage += SpiderBonus;
            else if (type == EnemyType.Zombie && _abilityManager.HasZombieDamageBonus) finalDamage += ZombieBonus;
            else if (type == EnemyType.Boss && _abilityManager.HasBossDamageBonus) finalDamage += BossBonus;

            return finalDamage;
        }
    }
}
