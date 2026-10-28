using UnityEngine;

namespace Player.Abilities
{
    public sealed class AbilityManager
    {
        private readonly AbilityDamageBonus _damageBonus;

        private readonly AbilityEffects _effects;

        public bool HasDash { get; private set; }
        public bool HasDoubleJump { get; private set; }
        public bool HasMap { get; private set; }
        public bool HasAnatomy { get; private set; }
        public bool HasArmor { get; private set; }
        public bool HasSwampDamageBonus { get; private set; }
        public bool HasSkeletonDamageBonus { get; private set; }
        public bool HasDemonDamageBonus { get; private set; }
        public bool HasSpiderDamageBonus { get; private set; }
        public bool HasZombieDamageBonus { get; private set; }
        public bool HasBossDamageBonus { get; private set; }
        public bool IsLastChanceActive { get; private set; }
        public bool HasPassiveHealthRegeneration { get; private set; }
        public bool HasRobocopRegeneration { get; private set; }

        public bool HasVampireAbility { get; private set; }
        public bool HasOnePunchManAbility { get; private set; }

        public AbilityManager(Hero hero)
        {
            _damageBonus = new AbilityDamageBonus(this);
            _effects = new AbilityEffects(hero);

            LoadAbilities();
        }

        public void UnlockDash()
        {
            HasDash = true;

            SaveAbilities();
        }

        public void UnlockAnatomy()
        {
            HasAnatomy = true;
            SaveAbilities();
            _effects.NotifyHealthPickups();  
        }

        public void UnlockMap()
        {
            HasMap = true;

            SaveAbilities();
        }

        public void UnlockArmor()
        {
            HasArmor = true;

            SaveAbilities();

            _effects.UnlockArmor();
        }

        public void UnlockSwampDamageBonus()
        {
            HasSwampDamageBonus = true;

            SaveAbilities();
        }

        public void UnlockSkeletonDamageBonus()
        {
            HasSkeletonDamageBonus = true;

            SaveAbilities();
        }

        public void UnlockDemonDamageBonus()
        {
            HasDemonDamageBonus = true;

            SaveAbilities();
        }

        public void UnlockSpiderDamageBonus()
        {
            HasSpiderDamageBonus = true;

            SaveAbilities();
        }

        public void UnlockZombieDamageBonus()
        {
            HasZombieDamageBonus = true;

            SaveAbilities();
        }

        public void UnlockBossDamageBonus()
        {
            HasBossDamageBonus = true;

            SaveAbilities();
        }

        public void PurchaseLastChance()
        {
            IsLastChanceActive = true;

            SaveAbilities();
        }

        public void UseLastChance()
        {
            IsLastChanceActive = false;

            SaveAbilities();
        }

        public void UnlockPassiveHealthRegeneration()
        {
            HasPassiveHealthRegeneration = true;

            SaveAbilities();

            _effects.EnableHealthRegeneration();
        }

        public void UnlockRobocopRegeneration()
        {
            HasRobocopRegeneration = true;

            SaveAbilities();

            _effects.EnableArmorRegeneration();
        }

        public void UnlockVampireAbility()
        {
            if (HasVampireAbility)
            {
                return;
            }

            HasVampireAbility = true;

            SaveAbilities();

            _effects.EnableVampire();
        }

        public void UnlockOnePunchManAbility()
        {
            if (HasOnePunchManAbility)
            {
                return;
            }

            HasOnePunchManAbility = true;

            SaveAbilities();

            _effects.EnableOnePunchMan();
        }

        public float GetDamageMultiplierForEnemy(GameObject enemy) => _damageBonus.GetDamageMultiplierForEnemy(enemy);

        public bool HasBonusForEnemy(GameObject enemy) => _damageBonus.HasBonusForEnemy(enemy);

        public bool NeedArmorPlates()
        {
            return HasArmor && _effects.NeedArmorPlates();
        }

        private void LoadAbilities()
        {
            if (SaveSystem.Instance != null &&
                SaveSystem.Instance.HasSave() &&
                SaveSystem.Instance.CurrentSave?.AbilityData != null)
            {
                LoadFromSaveData(SaveSystem.Instance.CurrentSave.AbilityData);

                return;
            }

            LoadFromSaveData(AbilityPreferences.Load());
        }

        private void LoadFromSaveData(AbilitySaveData saveData)
        {
            HasDash = saveData.HasDash;
            HasDoubleJump = saveData.HasDoubleJump;
            HasMap = saveData.HasMap;
            HasAnatomy = saveData.HasAnatomy;
            HasArmor = saveData.HasArmor;
            HasSwampDamageBonus = saveData.HasSwampDamageBonus;
            HasSkeletonDamageBonus = saveData.HasSkeletonDamageBonus;
            HasDemonDamageBonus = saveData.HasDemonDamageBonus;
            HasSpiderDamageBonus = saveData.HasSpiderDamageBonus;
            HasZombieDamageBonus = saveData.HasZombieDamageBonus;
            HasBossDamageBonus = saveData.HasBossDamageBonus;
            IsLastChanceActive = saveData.IsLastChanceActive;
            HasPassiveHealthRegeneration = saveData.HasPassiveHealthRegeneration;
            HasRobocopRegeneration = saveData.HasRobocopRegeneration;
            HasVampireAbility = saveData.HasVampireAbility;
            HasOnePunchManAbility = saveData.HasOnePunchManAbility;

            _effects.ApplyLoadedEffects(this);
        }

        private void SaveAbilities()
        {
            AbilityPreferences.Save(this);
            SaveToSaveSystem();
        }

        private void SaveToSaveSystem()
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.UpdateAbilityData(this);
            }
        }

    }
}
