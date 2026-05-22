using UnityEngine;

namespace Player.Abilities
{
    public sealed class PassiveArmorRegeneration : MonoBehaviour, IPassiveArmorRegeneration
    {
        private const float RegenerationInterval = 10f;
        private const int ArmorRegenAmount = 1;
        private const float ResetTimerValue = 0f;

        private Hero _hero;
        private ArmorManager _armorManager;
        private float _timer;

        private void Start()
        {
            _hero = GetComponent<Hero>() ?? FindFirstObjectByType<Hero>();
            _armorManager = GetComponent<ArmorManager>();
        }

        private void Update()
        {
            if (!CanRegenerateArmor())
            {
                _timer = ResetTimerValue;
                return;
            }

            ProcessRegeneration();
        }

        private bool CanRegenerateArmor()
        {
            return _hero != null
                   && _armorManager != null
                   && _hero.AbilityManager != null
                   && _hero.AbilityManager.HasRobocopRegeneration
                   && _armorManager.HasArmor
                   && _hero.IsAlive()
                   && _armorManager.CurrentArmor < _armorManager.MaxArmor;
        }

        private void ProcessRegeneration()
        {
            _timer += Time.deltaTime;

            if (_timer >= RegenerationInterval)
            {
                _armorManager.AddArmor(ArmorRegenAmount);
                _timer = ResetTimerValue;
            }
        }
    }
}
