namespace Player.Abilities
{
    public sealed class AbilityManager
    {
        private readonly Hero _hero;
        private AbilitySaveData _saveData;

        public bool IsLastChanceActive { get; private set; }

        public bool HasMap => _saveData.HasMap;
        public bool HasDash => _saveData.HasDash;
        public bool HasAnatomy => _saveData.HasAnatomy;
        public bool HasArmor => _saveData.HasArmor;
        public bool HasSwampDamageBonus => _saveData.HasSwampDamageBonus;
        public bool HasSkeletonDamageBonus => _saveData.HasSkeletonDamageBonus;
        public bool HasDemonDamageBonus => _saveData.HasDemonDamageBonus;
        public bool HasSpiderDamageBonus => _saveData.HasSpiderDamageBonus;
        public bool HasZombieDamageBonus => _saveData.HasZombieDamageBonus;
        public bool HasPassiveHealthRegeneration => _saveData.HasPassiveHealthRegeneration;
        public bool HasRobocopRegeneration => _saveData.HasRobocopRegeneration;
        public bool HasVampireAbility => _saveData.HasVampireAbility;
        public bool HasOnePunchManAbility => _saveData.HasOnePunchManAbility;
        public bool HasBossDamageBonus => _saveData.HasBossDamageBonus;

        public AbilityManager(Hero hero)
        {
            _hero = hero;
            _saveData = new AbilitySaveData();
            IsLastChanceActive = true;
        }

        public void LoadFromSave(AbilitySaveData data)
        {
            if (data != null)
            {
                _saveData = data;
            }
        }

        public AbilitySaveData GetSaveData()
        {
            return _saveData;
        }

        public void UseLastChance()
        {
            IsLastChanceActive = false;
        }

        public void UnlockMap() { _saveData.HasMap = true; SyncSave(); }
        public void UnlockDash() { _saveData.HasDash = true; SyncSave(); }
        public void UnlockAnatomy() { _saveData.HasAnatomy = true; SyncSave(); }
        public void UnlockArmor() { _saveData.HasArmor = true; SyncSave(); }
        public void UnlockSwampDamageBonus() { _saveData.HasSwampDamageBonus = true; SyncSave(); }
        public void UnlockSkeletonDamageBonus() { _saveData.HasSkeletonDamageBonus = true; SyncSave(); }
        public void UnlockDemonDamageBonus() { _saveData.HasDemonDamageBonus = true; SyncSave(); }
        public void UnlockSpiderDamageBonus() { _saveData.HasSpiderDamageBonus = true; SyncSave(); }
        public void UnlockZombieDamageBonus() { _saveData.HasZombieDamageBonus = true; SyncSave(); }
        public void UnlockPassiveHealthRegeneration() { _saveData.HasPassiveHealthRegeneration = true; SyncSave(); }
        public void UnlockRobocopRegeneration() { _saveData.HasRobocopRegeneration = true; SyncSave(); }
        public void UnlockVampireAbility() { _saveData.HasVampireAbility = true; SyncSave(); }
        public void UnlockOnePunchManAbility() { _saveData.HasOnePunchManAbility = true; SyncSave(); }
        public void UnlockBossDamageBonus() { _saveData.HasBossDamageBonus = true; SyncSave(); }

        private void SyncSave()
        {
            SaveSystem.Instance?.UpdateAbilityData(this);
        }
    }
}
