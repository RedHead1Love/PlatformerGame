using GameLogic;
using Player;
using Player.Abilities;
using UnityEngine;

namespace Foxberry.Shop
{
    internal sealed class ShopItemEffects
    {
        private const float EffectVerticalOffset = 2f;
        private const float EffectDestroyDelay = 2f;
        private const int EffectFontSize = 20;

        private readonly Hero _hero;
        private readonly AbilityManager _abilityManager;
        private ArmorManager _armorManager;

        public ShopItemEffects(Hero hero, AbilityManager abilities, ArmorManager armor)
        {
            _hero = hero;
            _abilityManager = abilities;
            _armorManager = armor;
        }

        public void Apply(string itemId)
        {
            if (_abilityManager == null)
            {
                return;
            }

            switch (itemId)
            {
                case ShopItemIds.UnlockMap:
                    _abilityManager.UnlockMap();
                    break;

                case ShopItemIds.UnlockDash:
                    _abilityManager.UnlockDash();
                    break;

                case ShopItemIds.UnlockAnatomy:
                    _abilityManager.UnlockAnatomy();

                    break;

                case ShopItemIds.UnlockArmor:
                    _abilityManager.UnlockArmor();
                    _armorManager?.FillArmor();

                    break;

                case ShopItemIds.UnlockSwampDamageBonus:
                    _abilityManager.UnlockSwampDamageBonus();
                    break;

                case ShopItemIds.ActivateLastChance:
                    _abilityManager.PurchaseLastChance();
                    break;

                case ShopItemIds.RestoreArmor:
                    RestoreArmor();
                    break;

                case ShopItemIds.UnlockSkeletonDamageBonus:
                    _abilityManager.UnlockSkeletonDamageBonus();
                    break;

                case ShopItemIds.UnlockDemonDamageBonus:
                    _abilityManager.UnlockDemonDamageBonus();
                    break;

                case ShopItemIds.UnlockSpiderDamageBonus:
                    _abilityManager.UnlockSpiderDamageBonus();
                    break;

                case ShopItemIds.UnlockPassiveHealthRegeneration:
                    _abilityManager.UnlockPassiveHealthRegeneration();
                    break;

                case ShopItemIds.UnlockRobocopRegeneration:
                    _abilityManager.UnlockRobocopRegeneration();
                    break;

                case ShopItemIds.UnlockVampireAbility:
                    _abilityManager.UnlockVampireAbility();
                    break;

                case ShopItemIds.UnlockOnePunchManAbility:
                    _abilityManager.UnlockOnePunchManAbility();
                    break;

                case ShopItemIds.UnlockZombieDamageBonus:
                    _abilityManager.UnlockZombieDamageBonus();
                    break;

                case ShopItemIds.UnlockBossDamageBonus:
                    _abilityManager.UnlockBossDamageBonus();
                    break;
            }
        }

        private void RestoreArmor()
        {
            if (_armorManager == null && _hero != null)
            {
                _armorManager = _hero.GetComponent<ArmorManager>();
            }

            if (_armorManager == null || _armorManager.IsArmorUnlocked() == false)
            {
                return;
            }

            _armorManager.FillArmor();
            ShowArmorRestoredEffect();
        }

        private void ShowArmorRestoredEffect()
        {
            if (_hero == null)
            {
                return;
            }

            GameObject effect = new GameObject("ArmorRestoredEffect");
            effect.transform.position = _hero.transform.position + (Vector3.up * EffectVerticalOffset);

            TextMesh textMesh = effect.AddComponent<TextMesh>();

            textMesh.text = LocalizationManager.CurrentLanguage switch
            {
                GameLanguage.English => "armor restored",
                GameLanguage.Turkish => "zırh yenilendi",
                _ => "броня восстановлена",
            };

            textMesh.color = Color.cyan;
            textMesh.fontSize = EffectFontSize;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.fontStyle = FontStyle.Bold;

            Object.Destroy(effect, EffectDestroyDelay);
        }
    }
}
