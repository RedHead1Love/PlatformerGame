using UnityEngine;

namespace Player.Abilities
{
    public sealed class AbilityManager
    {
        private const float BonusMultiplier = 2f;

        private const string DashKey = "Ability_Dash";
        private const string DoubleJumpKey = "Ability_DoubleJump";
        private const string MapKey = "Ability_Map";
        private const string AnatomyKey = "Ability_Anatomy";
        private const string ArmorKey = "Ability_Armor";
        private const string SwampDamageKey = "Ability_SwampDamage";
        private const string SkeletonDamageKey = "Ability_SkeletonDamage";
        private const string DemonDamageKey = "Ability_DemonDamage";
        private const string SpiderDamageKey = "Ability_SpiderDamage";
        private const string ZombieDamageKey = "Ability_ZombieDamage";
        private const string BossDamageKey = "Ability_BossDamage";
        private const string PassiveHealthRegenerationKey = "Ability_PassiveHealthRegen";
        private const string RobocopRegenerationKey = "Ability_RobocopRegen";
        private const string LastChanceKey = "LastChance_Active";
        private const string VampireKey = "Ability_Vampire";
        private const string OnePunchManKey = "Ability_OnePunchMan";

        private const int ActiveValue = 1;
        private const int InactiveValue = 0;

        private readonly Hero _hero;
        private readonly ArmorManager _armorManager;

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

        public bool HasVampireAbility;
        public bool HasOnePunchManAbility;

        public AbilityManager(Hero hero)
        {
            _hero = hero;
            _armorManager = FindArmorManager();

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

            _armorManager?.UnlockArmorAbility();
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

            PassiveHealthRegeneration regeneration = _hero?.GetComponent<PassiveHealthRegeneration>();
            regeneration?.EnableRegeneration();
        }

        public void UnlockRobocopRegeneration()
        {
            HasRobocopRegeneration = true;
            SaveAbilities();

            PassiveArmorRegeneration regeneration = _hero?.GetComponent<PassiveArmorRegeneration>();
            regeneration?.Activate();
        }

        public void UnlockVampireAbility()
        {
            HasVampireAbility = true;
            SaveAbilities();

            VampireHealthSystem vampireSystem = GetOrAddComponent<VampireHealthSystem>();
            vampireSystem?.Activate();
        }

        public void UnlockOnePunchManAbility()
        {
            HasOnePunchManAbility = true;
            SaveAbilities();

            OnePunchManSystem onePunchManSystem = GetOrAddComponent<OnePunchManSystem>();
            onePunchManSystem?.Activate();
        }

        public float GetDamageMultiplierForEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                return 1f;
            }

            EnemyTypeComponent enemyTypeComponent = GetEnemyTypeComponent(enemy);

            if (enemyTypeComponent == null)
            {
                return 1f;
            }

            return enemyTypeComponent.EnemyType switch
            {
                EnemyType.Swamp => HasSwampDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Skeleton => HasSkeletonDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Demon => HasDemonDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Spider => HasSpiderDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Zombie => HasZombieDamageBonus ? BonusMultiplier : 1f,
                EnemyType.Boss => HasBossDamageBonus ? BonusMultiplier : 1f,
                _ => 1f
            };
        }

        public bool HasBonusForEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                return false;
            }

            EnemyTypeComponent enemyTypeComponent = GetEnemyTypeComponent(enemy);

            if (enemyTypeComponent == null)
            {
                return false;
            }

            return enemyTypeComponent.EnemyType switch
            {
                EnemyType.Swamp => HasSwampDamageBonus,
                EnemyType.Skeleton => HasSkeletonDamageBonus,
                EnemyType.Demon => HasDemonDamageBonus,
                EnemyType.Spider => HasSpiderDamageBonus,
                EnemyType.Zombie => HasZombieDamageBonus,
                EnemyType.Boss => HasBossDamageBonus,
                _ => false
            };
        }

        public bool NeedArmorPlates()
        {
            return HasArmor &&
                   _armorManager != null &&
                   _armorManager.NeedArmorPlates();
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

        private EnemyTypeComponent GetEnemyTypeComponent(GameObject enemy)
        {
            EnemyTypeComponent component = enemy.GetComponent<EnemyTypeComponent>();

            if (component == null)
            {
                component = enemy.GetComponentInParent<EnemyTypeComponent>();
            }

            return component;
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

        private void LoadAbilities()
        {
            if (SaveSystem.Instance != null &&
                SaveSystem.Instance.HasSave() &&
                SaveSystem.Instance.CurrentSave?.abilityData != null)
            {
                LoadFromSaveData(SaveSystem.Instance.CurrentSave.abilityData);

                return;
            }

            LoadFromPlayerPrefs();
            ApplyLoadedEffects();
        }

        private void LoadFromSaveData(AbilitySaveData saveData)
        {
            HasDash = saveData.hasDash;
            HasDoubleJump = saveData.hasDoubleJump;
            HasMap = saveData.hasMap;
            HasAnatomy = saveData.hasAnatomy;
            HasArmor = saveData.hasArmor;
            HasSwampDamageBonus = saveData.hasSwampDamageBonus;
            HasSkeletonDamageBonus = saveData.hasSkeletonDamageBonus;
            HasDemonDamageBonus = saveData.hasDemonDamageBonus;
            HasSpiderDamageBonus = saveData.hasSpiderDamageBonus;
            HasZombieDamageBonus = saveData.hasZombieDamageBonus;
            HasBossDamageBonus = saveData.hasBossDamageBonus;
            IsLastChanceActive = saveData.isLastChanceActive;
            HasPassiveHealthRegeneration = saveData.hasPassiveHealthRegeneration;
            HasRobocopRegeneration = saveData.hasRobocopRegeneration;
            HasVampireAbility = saveData.hasVampireAbility;
            HasOnePunchManAbility = saveData.hasOnePunchManAbility;

            ApplyLoadedEffects();
        }

        private void LoadFromPlayerPrefs()
        {
            HasDash = ReadBool(DashKey);
            HasDoubleJump = ReadBool(DoubleJumpKey);
            HasMap = ReadBool(MapKey);
            HasAnatomy = ReadBool(AnatomyKey);
            HasArmor = ReadBool(ArmorKey);
            HasSwampDamageBonus = ReadBool(SwampDamageKey);
            HasSkeletonDamageBonus = ReadBool(SkeletonDamageKey);
            HasDemonDamageBonus = ReadBool(DemonDamageKey);
            HasSpiderDamageBonus = ReadBool(SpiderDamageKey);
            HasZombieDamageBonus = ReadBool(ZombieDamageKey);
            HasBossDamageBonus = ReadBool(BossDamageKey);
            HasPassiveHealthRegeneration = ReadBool(PassiveHealthRegenerationKey);
            HasRobocopRegeneration = ReadBool(RobocopRegenerationKey);
            IsLastChanceActive = ReadBool(LastChanceKey);
            HasVampireAbility = ReadBool(VampireKey);
            HasOnePunchManAbility = ReadBool(OnePunchManKey);
        }

        private void ApplyLoadedEffects()
        {
            if (_hero == null)
            {
                return;
            }

            if (HasArmor)
            {
                _armorManager?.UnlockArmorAbility();
            }

            if (HasPassiveHealthRegeneration)
            {
                _hero.GetComponent<PassiveHealthRegeneration>()?.EnableRegeneration();
            }

            if (HasRobocopRegeneration)
            {
                _hero.GetComponent<PassiveArmorRegeneration>()?.Activate();
            }

            if (HasVampireAbility)
            {
                GetOrAddComponent<VampireHealthSystem>()?.Activate();
            }

            if (HasOnePunchManAbility)
            {
                GetOrAddComponent<OnePunchManSystem>()?.Activate();
            }
        }

        private void SaveAbilities()
        {
            SaveToPlayerPrefs();
            SaveToSaveSystem();
        }

        private void SaveToPlayerPrefs()
        {
            WriteBool(DashKey, HasDash);
            WriteBool(DoubleJumpKey, HasDoubleJump);
            WriteBool(MapKey, HasMap);
            WriteBool(AnatomyKey, HasAnatomy);
            WriteBool(ArmorKey, HasArmor);
            WriteBool(SwampDamageKey, HasSwampDamageBonus);
            WriteBool(SkeletonDamageKey, HasSkeletonDamageBonus);
            WriteBool(DemonDamageKey, HasDemonDamageBonus);
            WriteBool(SpiderDamageKey, HasSpiderDamageBonus);
            WriteBool(ZombieDamageKey, HasZombieDamageBonus);
            WriteBool(BossDamageKey, HasBossDamageBonus);
            WriteBool(PassiveHealthRegenerationKey, HasPassiveHealthRegeneration);
            WriteBool(RobocopRegenerationKey, HasRobocopRegeneration);
            WriteBool(LastChanceKey, IsLastChanceActive);
            WriteBool(VampireKey, HasVampireAbility);
            WriteBool(OnePunchManKey, HasOnePunchManAbility);

            PlayerPrefs.Save();
        }

        private void SaveToSaveSystem()
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.UpdateAbilityData(this);
            }
        }

        private bool ReadBool(string key)
        {
            return PlayerPrefs.GetInt(key, InactiveValue) == ActiveValue;
        }

        private void WriteBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? ActiveValue : InactiveValue);
        }
    }
}