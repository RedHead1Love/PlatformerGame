using GeneralLogicEnemies;
using UnityEngine;

namespace Player.Abilities
{
    public sealed class OnePunchManSystem : MonoBehaviour, IOnePunchManSystem
    {
        private const float InstakillChance = 0.05f;

        private Hero _hero;

        private void Start()
        {
            _hero = GetComponent<Hero>();

            if (_hero == null)
            {
                _hero = FindFirstObjectByType<Hero>();
            }
        }

        public bool CheckForInstakill(Entity enemy)
        {
            if (_hero == null || _hero.AbilityManager == null || !_hero.AbilityManager.HasOnePunchManAbility)
            {
                return false;
            }

            IEnemyTypeComponent enemyTypeComponent = enemy.GetComponent<IEnemyTypeComponent>();

            if (enemyTypeComponent != null && enemyTypeComponent.EnemyType == EnemyType.Boss)
            {
                return false;
            }

            return Random.value <= InstakillChance;
        }
    }
}
