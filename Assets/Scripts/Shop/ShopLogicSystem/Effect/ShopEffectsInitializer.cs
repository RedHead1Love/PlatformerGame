using Player;
using Player.Abilities;
using UnityEngine;

namespace Foxberry.Shop
{
    public sealed class ShopEffectsInitializer : MonoBehaviour
    {
        [SerializeField]
        private ShopManager _shopManager;

        public void InitializeShopEffects()
        {
            if (_shopManager == null)
            {
                _shopManager = FindObjectOfType<ShopManager>();

                if (_shopManager == null)
                {
                    return;
                }
            }

            Hero hero = Hero.Instance != null ? Hero.Instance : FindAnyObjectByType<Hero>();

            if (hero == null || hero.AbilityManager == null)
            {
                return;
            }

            var shopItems = _shopManager.ShopItems;

            if (shopItems == null)
            {
                return;
            }

            foreach (var item in shopItems)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.IsSold && item.ItemId != ShopItemIds.ActivateLastChance && item.ItemId != ShopItemIds.RestoreArmor)
                {
                    ApplyItemEffect(item.ItemId, hero.AbilityManager, hero);
                }
            }
        }

        private void Start()
        {
            InitializeShopEffects();
        }

        private void ApplyItemEffect(string itemId, AbilityManager abilityManager, Hero hero)
        {
            switch (itemId)
            {
            case ShopItemIds.UnlockMap:
                if (!abilityManager.HasMap)
                {
                    abilityManager.UnlockMap();
                }

                break;

            case ShopItemIds.UnlockDash:
                if (!abilityManager.HasDash)
                {
                    abilityManager.UnlockDash();
                }

                break;

            case ShopItemIds.UnlockAnatomy:
                if (!abilityManager.HasAnatomy)
                {
                    abilityManager.UnlockAnatomy();
                }

                break;

            case ShopItemIds.UnlockArmor:
                if (!abilityManager.HasArmor)
                {
                    abilityManager.UnlockArmor();

                    var armorManager = hero.GetComponent<ArmorManager>();

                    armorManager?.UnlockArmorAbility();
                }

                break;

            case ShopItemIds.UnlockSwampDamageBonus:
                if (!abilityManager.HasSwampDamageBonus)
                {
                    abilityManager.UnlockSwampDamageBonus();
                }

                break;

            case ShopItemIds.UnlockSkeletonDamageBonus:
                if (!abilityManager.HasSkeletonDamageBonus)
                {
                    abilityManager.UnlockSkeletonDamageBonus();
                }

                break;

            case ShopItemIds.UnlockDemonDamageBonus:
                if (!abilityManager.HasDemonDamageBonus)
                {
                    abilityManager.UnlockDemonDamageBonus();
                }

                break;

            case ShopItemIds.UnlockSpiderDamageBonus:
                if (!abilityManager.HasSpiderDamageBonus)
                {
                    abilityManager.UnlockSpiderDamageBonus();
                }

                break;

            case ShopItemIds.UnlockZombieDamageBonus:
                if (!abilityManager.HasZombieDamageBonus)
                {
                    abilityManager.UnlockZombieDamageBonus();
                }

                break;

            case ShopItemIds.UnlockPassiveHealthRegeneration:
                if (!abilityManager.HasPassiveHealthRegeneration)
                {
                    abilityManager.UnlockPassiveHealthRegeneration();
                }

                break;

            case ShopItemIds.UnlockRobocopRegeneration:
                if (!abilityManager.HasRobocopRegeneration)
                {
                    abilityManager.UnlockRobocopRegeneration();
                }

                break;

            case ShopItemIds.UnlockVampireAbility:
                if (!abilityManager.HasVampireAbility)
                {
                    abilityManager.UnlockVampireAbility();
                }

                break;

            case ShopItemIds.UnlockOnePunchManAbility:
                if (!abilityManager.HasOnePunchManAbility)
                {
                    abilityManager.UnlockOnePunchManAbility();
                }

                break;

            case ShopItemIds.UnlockBossDamageBonus:
                if (!abilityManager.HasBossDamageBonus)
                {
                    abilityManager.UnlockBossDamageBonus();
                }

                break;
            }
        }
    }
}
