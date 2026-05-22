using GeneralLogicEnemies;
using UnityEngine;

namespace Player.Abilities
{
    public sealed class VampireHealthSystem : MonoBehaviour, IVampireHealthSystem
    {
        private const float VampireHealChance = 0.2f;
        private const int HealAmount = 1;

        private Hero _hero;

        private void Start()
        {
            _hero = GetComponent<Hero>();

            if (_hero == null)
            {
                _hero = FindFirstObjectByType<Hero>();
            }
        }

        public void OnEnemyKilled(Entity enemy)
        {
            if (_hero == null || _hero.AbilityManager == null || !_hero.AbilityManager.HasVampireAbility)
            {
                return;
            }

            if (Random.value <= VampireHealChance && _hero.NeedsHealing())
            {
                HealthManager healthManager = _hero.GetComponent<HealthManager>();

                healthManager?.Heal(HealAmount);
            }
        }
    }
}
