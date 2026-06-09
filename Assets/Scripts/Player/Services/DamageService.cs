using Player.StateMachine;

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
            if (_hero == null || _healthManager == null || damageAmount <= 0)
            {
                return;
            }

            if (CanUseLastChance(damageAmount))
            {
                _hero.AbilityManager.UseLastChance();
                _hero.SetHealth(LastChanceHealth);

                return;
            }

            _healthManager.TakeDamage(damageAmount);

            if (_healthManager.CurrentHealth <= FatalHealthThreshold)
            {
                _hero.StateMachine?.Change<DieState>();

                return;
            }

            _hero.StateMachine?.Change<HurtState>();
        }

        private bool CanUseLastChance(int damageAmount)
        {
            return _hero.AbilityManager != null &&
                   _hero.AbilityManager.IsLastChanceActive &&
                   damageAmount >= _healthManager.CurrentHealth;
        }
    }
}