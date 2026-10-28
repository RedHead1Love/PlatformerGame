using UnityEngine;

namespace Player.Abilities
{
    public sealed class AbilityEffects
    {
        private readonly Hero _hero;
        private readonly ArmorManager _armorManager;

        public AbilityEffects(Hero hero)
        {
            _hero = hero;
            _armorManager = FindArmorManager();
        }

        public bool NeedArmorPlates() => _armorManager != null && _armorManager.NeedArmorPlates();

        public void UnlockArmor() => _armorManager?.UnlockArmorAbility();

        public void EnableHealthRegeneration() => _hero?.GetComponent<PassiveHealthRegeneration>()?.EnableRegeneration();

        public void EnableArmorRegeneration() => _hero?.GetComponent<PassiveArmorRegeneration>()?.Activate();

        public void EnableVampire() => GetOrAddComponent<VampireHealthSystem>()?.Activate();

        public void EnableOnePunchMan() => GetOrAddComponent<OnePunchManSystem>()?.Activate();

        public void NotifyHealthPickups()
        {
            var pickups = Object.FindObjectsByType<HealthPickup>(FindObjectsSortMode.None);

            foreach (var pickup in pickups)
            {
                pickup?.RefreshPickupState();
            }
        }

        private ArmorManager FindArmorManager()
        {
            ArmorManager armorManager = _hero != null ? _hero.GetComponent<ArmorManager>() : null;

            if (armorManager == null)
            {
                armorManager = Object.FindFirstObjectByType<ArmorManager>();
            }

            return armorManager;
        }

        private T GetOrAddComponent<T>() where T : Component
        {
            if (_hero == null)
            {
                return null;
            }

            T component = _hero.GetComponent<T>();

            if (component == null)
            {
                component = _hero.gameObject.AddComponent<T>();
            }

            return component;
        }

        public void ApplyLoadedEffects(AbilityManager abilities)
        {
            if (_hero == null)
            {
                return;
            }

            if (abilities.HasArmor)
            {
                _armorManager?.UnlockArmorAbility();
            }

            if (abilities.HasPassiveHealthRegeneration)
            {
                _hero.GetComponent<PassiveHealthRegeneration>()?.EnableRegeneration();
            }

            if (abilities.HasRobocopRegeneration)
            {
                _hero.GetComponent<PassiveArmorRegeneration>()?.Activate();
            }

            if (abilities.HasVampireAbility)
            {
                GetOrAddComponent<VampireHealthSystem>()?.Activate();
            }

            if (abilities.HasOnePunchManAbility)
            {
                GetOrAddComponent<OnePunchManSystem>()?.Activate();
            }
        }
    }
}
