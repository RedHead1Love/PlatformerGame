using UnityEngine;

namespace Player.Abilities
{
    public static class AbilityPreferences
    {
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

        public static AbilitySaveData Load()
        {
            AbilitySaveData data = new AbilitySaveData();
            data.HasDash = ReadBool(DashKey);
            data.HasDoubleJump = ReadBool(DoubleJumpKey);
            data.HasMap = ReadBool(MapKey);
            data.HasAnatomy = ReadBool(AnatomyKey);
            data.HasArmor = ReadBool(ArmorKey);
            data.HasSwampDamageBonus = ReadBool(SwampDamageKey);
            data.HasSkeletonDamageBonus = ReadBool(SkeletonDamageKey);
            data.HasDemonDamageBonus = ReadBool(DemonDamageKey);
            data.HasSpiderDamageBonus = ReadBool(SpiderDamageKey);
            data.HasZombieDamageBonus = ReadBool(ZombieDamageKey);
            data.HasBossDamageBonus = ReadBool(BossDamageKey);
            data.HasPassiveHealthRegeneration = ReadBool(PassiveHealthRegenerationKey);
            data.HasRobocopRegeneration = ReadBool(RobocopRegenerationKey);
            data.IsLastChanceActive = ReadBool(LastChanceKey);
            data.HasVampireAbility = ReadBool(VampireKey);
            data.HasOnePunchManAbility = ReadBool(OnePunchManKey);
            return data;
        }

        public static void Save(AbilityManager abilities)
        {
            WriteBool(DashKey, abilities.HasDash);
            WriteBool(DoubleJumpKey, abilities.HasDoubleJump);
            WriteBool(MapKey, abilities.HasMap);
            WriteBool(AnatomyKey, abilities.HasAnatomy);
            WriteBool(ArmorKey, abilities.HasArmor);
            WriteBool(SwampDamageKey, abilities.HasSwampDamageBonus);
            WriteBool(SkeletonDamageKey, abilities.HasSkeletonDamageBonus);
            WriteBool(DemonDamageKey, abilities.HasDemonDamageBonus);
            WriteBool(SpiderDamageKey, abilities.HasSpiderDamageBonus);
            WriteBool(ZombieDamageKey, abilities.HasZombieDamageBonus);
            WriteBool(BossDamageKey, abilities.HasBossDamageBonus);
            WriteBool(PassiveHealthRegenerationKey, abilities.HasPassiveHealthRegeneration);
            WriteBool(RobocopRegenerationKey, abilities.HasRobocopRegeneration);
            WriteBool(LastChanceKey, abilities.IsLastChanceActive);
            WriteBool(VampireKey, abilities.HasVampireAbility);
            WriteBool(OnePunchManKey, abilities.HasOnePunchManAbility);

            PlayerPrefs.Save();
        }

        private static bool ReadBool(string key)
        {
            return PlayerPrefs.GetInt(key, InactiveValue) == ActiveValue;
        }

        private static void WriteBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? ActiveValue : InactiveValue);
        }
    }
}
