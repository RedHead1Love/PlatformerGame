using Player.StateMachine;
using UnityEngine;

namespace Player
{
    public sealed class DamageService
    {
        private const int LastChanceHealth = 1;
        private const int FatalHealthThreshold = 0;

        private readonly Hero _hero;
        private readonly HealthManager _healthManager;

        public DamageService(Hero hero, HealthManager healthManager)
        {
            _hero = hero;
            _healthManager = healthManager;
        }

        public void ApplyDamage(int damageAmount)
        {
            if (_healthManager == null)
            {
                return;
            }

            int currentHealth = _healthManager.CurrentHealth;

            if (_hero.AbilityManager != null && _hero.AbilityManager.IsLastChanceActive && damageAmount >= currentHealth)
            {
                _hero.AbilityManager.UseLastChance();
                _hero.SetHealth(LastChanceHealth);

                return;
            }

            _healthManager.TakeDamage(damageAmount);

            HandleDamageResponse(currentHealth, damageAmount);
        }

        private void HandleDamageResponse(int currentHealth, int damageAmount)
        {
            if (currentHealth - damageAmount <= FatalHealthThreshold)
            {
                _hero.StateMachine.Change<DieState>();
            }
            else
            {
                _hero.StateMachine.Change<HurtState>();
            }
        }
    }
}
