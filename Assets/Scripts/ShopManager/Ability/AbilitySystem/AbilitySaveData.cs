using System;

namespace Player.Abilities
{
    [Serializable]
    public sealed class AbilitySaveData
    {
        public bool HasMap;
        public bool HasDash;
        public bool HasAnatomy;
        public bool HasArmor;
        public bool HasSwampDamageBonus;
        public bool HasSkeletonDamageBonus;
        public bool HasDemonDamageBonus;
        public bool HasSpiderDamageBonus;
        public bool HasZombieDamageBonus;
        public bool HasPassiveHealthRegeneration;
        public bool HasRobocopRegeneration;
        public bool HasVampireAbility;
        public bool HasOnePunchManAbility;
        public bool HasBossDamageBonus;

        public AbilitySaveData()
        {
            HasMap = false;
            HasDash = false;
            HasAnatomy = false;
            HasArmor = false;
            HasSwampDamageBonus = false;
            HasSkeletonDamageBonus = false;
            HasDemonDamageBonus = false;
            HasSpiderDamageBonus = false;
            HasZombieDamageBonus = false;
            HasPassiveHealthRegeneration = false;
            HasRobocopRegeneration = false;
            HasVampireAbility = false;
            HasOnePunchManAbility = false;
            HasBossDamageBonus = false;
        }
    }
}
