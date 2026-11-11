using Player.Abilities;
using UnityEngine;

namespace GameLogic
{
    [System.Serializable]
    public sealed class AbilitySaveData
    {
        [SerializeField]
        private bool HasDash;
        [SerializeField]
        private bool HasDoubleJump;
        [SerializeField]
        private bool HasMap;
        [SerializeField]
        private bool HasAnatomy;
        [SerializeField]
        private bool HasArmor;
        [SerializeField]
        private bool HasSwampDamageBonus;
        [SerializeField]
        private bool HasSkeletonDamageBonus;
        [SerializeField]
        private bool HasDemonDamageBonus;
        [SerializeField]
        private bool HasSpiderDamageBonus;
        [SerializeField]
        private bool HasZombieDamageBonus;
        [SerializeField]
        private bool HasBossDamageBonus;
        [SerializeField]
        private bool IsLastChanceActive;
        [SerializeField]
        private bool HasPassiveHealthRegeneration;
        [SerializeField]
        private bool HasRobocopRegeneration;
        [SerializeField]
        private bool HasVampireAbility;
        [SerializeField]
        private bool HasOnePunchManAbility;

        public bool Dash
        {
            get => HasDash;
            set => HasDash = value;
        }

        public bool DoubleJump
        {
            get => HasDoubleJump;
            set => HasDoubleJump = value;
        }

        public bool Map
        {
            get => HasMap;
            set => HasMap = value;
        }

        public bool Anatomy
        {
            get => HasAnatomy;
            set => HasAnatomy = value;
        }

        public bool Armor
        {
            get => HasArmor;
            set => HasArmor = value;
        }

        public bool SwampDamageBonus
        {
            get => HasSwampDamageBonus;
            set => HasSwampDamageBonus = value;
        }

        public bool SkeletonDamageBonus
        {
            get => HasSkeletonDamageBonus;
            set => HasSkeletonDamageBonus = value;
        }

        public bool DemonDamageBonus
        {
            get => HasDemonDamageBonus;
            set => HasDemonDamageBonus = value;
        }

        public bool SpiderDamageBonus
        {
            get => HasSpiderDamageBonus;
            set => HasSpiderDamageBonus = value;
        }

        public bool ZombieDamageBonus
        {
            get => HasZombieDamageBonus;
            set => HasZombieDamageBonus = value;
        }

        public bool BossDamageBonus
        {
            get => HasBossDamageBonus;
            set => HasBossDamageBonus = value;
        }

        public bool LastChanceActive
        {
            get => IsLastChanceActive;
            set => IsLastChanceActive = value;
        }

        public bool PassiveHealthRegeneration
        {
            get => HasPassiveHealthRegeneration;
            set => HasPassiveHealthRegeneration = value;
        }

        public bool RobocopRegeneration
        {
            get => HasRobocopRegeneration;
            set => HasRobocopRegeneration = value;
        }

        public bool VampireAbility
        {
            get => HasVampireAbility;
            set => HasVampireAbility = value;
        }

        public bool OnePunchManAbility
        {
            get => HasOnePunchManAbility;
            set => HasOnePunchManAbility = value;
        }

        public void Capture(AbilityManager abilities)
        {
            HasDash = abilities.HasDash;
            HasDoubleJump = abilities.HasDoubleJump;
            HasMap = abilities.HasMap;
            HasAnatomy = abilities.HasAnatomy;
            HasArmor = abilities.HasArmor;
            HasSwampDamageBonus = abilities.HasSwampDamageBonus;
            HasSkeletonDamageBonus = abilities.HasSkeletonDamageBonus;
            HasDemonDamageBonus = abilities.HasDemonDamageBonus;
            HasSpiderDamageBonus = abilities.HasSpiderDamageBonus;
            HasZombieDamageBonus = abilities.HasZombieDamageBonus;
            HasBossDamageBonus = abilities.HasBossDamageBonus;
            IsLastChanceActive = abilities.IsLastChanceActive;
            HasPassiveHealthRegeneration = abilities.HasPassiveHealthRegeneration;
            HasRobocopRegeneration = abilities.HasRobocopRegeneration;
            HasVampireAbility = abilities.HasVampireAbility;
            HasOnePunchManAbility = abilities.HasOnePunchManAbility;
        }
    }
}
