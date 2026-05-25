using UnityEngine;

namespace Player.Abilities
{
    public sealed class PassiveHealthRegeneration : MonoBehaviour
    {
        private const float RegenerationInterval = 5f;
        private const int HealthRegenAmount = 1;
        private const float ResetTimerValue = 0f;

        private Hero _hero;
        private float _timer;

        private void Start()
        {
            _hero = GetComponent<Hero>();

            if (_hero == null)
            {
                _hero = FindFirstObjectByType<Hero>();
            }
        }

        private void Update()
        {
            if (!CanRegenerate())
            {
                _timer = ResetTimerValue;
                return;
            }

            ProcessRegeneration();
        }

        private bool CanRegenerate()
        {
            return _hero != null
                   && _hero.AbilityManager != null
                   && _hero.AbilityManager.HasPassiveHealthRegeneration
                   && _hero.IsAlive()
                   && _hero.NeedsHealing();
        }

        private void ProcessRegeneration()
        {
            _timer += Time.deltaTime;

            if (_timer >= RegenerationInterval)
            {
                HealthManager healthManager = _hero.GetComponent<HealthManager>();

                if (healthManager != null)
                {
                    _hero.SetHealth(healthManager.CurrentHealth + HealthRegenAmount);
                }

                _timer = ResetTimerValue;
            }
        }
    }
}
