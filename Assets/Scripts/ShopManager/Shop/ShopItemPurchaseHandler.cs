using GameLogic;
using Player.Abilities;
using UnityEngine;

public sealed class ShopItemPurchaseHandler
{
    private const float EffectVerticalOffset = 2f;
    private const float EffectDestroyDelay = 2f;
    private const int EffectFontSize = 20;

    private readonly Hero _hero;
    private readonly AbilityManager _abilityManager;

    private ArmorManager _armorManager;
    private ShopManager _shopManager;

    public ShopItemPurchaseHandler(Hero hero = null, ArmorManager armorManager = null, ShopManager shopManager = null)
    {
        _hero = hero;
        _abilityManager = _hero != null ? _hero.AbilityManager : null;
        _armorManager = armorManager;
        _shopManager = shopManager;

        if (_armorManager == null && _hero != null)
        {
            _armorManager = _hero.GetComponent<ArmorManager>();
        }
    }

    public bool TryPurchaseItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return false;
        }

        ShopItemData item = FindShopItem(itemId);

        if (item == null || item.CanBePurchased() == false)
        {
            return false;
        }

        if (ProcessPayment(item) == false)
        {
            return false;
        }

        ApplyItemEffect(itemId);
        SavePurchase(itemId);

        return true;
    }

    public string GetItemDescription(string itemId)
    {
        bool isEnglish = LocalizationManager.CurrentLanguage == LocalizationManager.Language.English;

        return itemId switch
        {
            ShopItemIds.UnlockMap => isEnglish ? "prison map" : "карта тюрьмы",
            ShopItemIds.UnlockDash => isEnglish ? "quick dash" : "быстрый подкат",
            ShopItemIds.UnlockAnatomy => isEnglish ? "allows picking up medkits" : "позволяет подбирать аптечки",
            ShopItemIds.UnlockArmor => isEnglish ? "grants armor" : "получение брони",
            ShopItemIds.UnlockSwampDamageBonus => isEnglish ? "+100% damage  swamp monsters" : "+100% урона по болотным",
            ShopItemIds.ActivateLastChance => isEnglish ? "survive a fatal blow" : "выживание при смертельном ударе",
            ShopItemIds.RestoreArmor => isEnglish ? "restores armor" : "восстановление брони",
            ShopItemIds.UnlockSkeletonDamageBonus => isEnglish ? "+100% damage to skeletons" : "+100% урона по скелетам",
            ShopItemIds.UnlockDemonDamageBonus => isEnglish ? "+100% damage to demons" : "+100% урона по демонам",
            ShopItemIds.UnlockSpiderDamageBonus => isEnglish ? "+100% damage to spiders" : "+100% урона по паукам",
            ShopItemIds.UnlockZombieDamageBonus => isEnglish ? "+100% damage to zombies" : "+100% урона по зомби",
            ShopItemIds.UnlockPassiveHealthRegeneration => isEnglish ? "passive health regeneration" : "пассивное восстановление здоровья",
            ShopItemIds.UnlockRobocopRegeneration => isEnglish ? "passive armor regeneration" : "пассивное восстановление брони",
            ShopItemIds.UnlockVampireAbility => isEnglish ? "restores health on kill" : "получение здоровья за убийство",
            ShopItemIds.UnlockOnePunchManAbility => isEnglish ? "chance of instant kill" : "шанс мгновенного убийства ",
            ShopItemIds.UnlockBossDamageBonus => isEnglish ? "+100% damage to bosses" : "+100% урона по боссам",
            _ => string.Empty
        };
    }

    public string GetItemName(string itemId)
    {
        bool isEnglish = LocalizationManager.CurrentLanguage == LocalizationManager.Language.English;

        return itemId switch
        {
            ShopItemIds.UnlockMap => isEnglish ? "Map" : "Карта",
            ShopItemIds.UnlockDash => isEnglish ? "Dash" : "Рывок",
            ShopItemIds.UnlockAnatomy => isEnglish ? "Anatomy" : "Aнатомия",
            ShopItemIds.UnlockArmor => isEnglish ? "Armor" : "Броня",
            ShopItemIds.UnlockSwampDamageBonus => isEnglish ? "Dirty" : "Грязный",
            ShopItemIds.ActivateLastChance => isEnglish ? "Last Dance" : "Ласт Дэнс",
            ShopItemIds.RestoreArmor => isEnglish ? "Plates" : "Пластины",
            ShopItemIds.UnlockSkeletonDamageBonus => isEnglish ? "Rotten" : "Гнилой",
            ShopItemIds.UnlockDemonDamageBonus => isEnglish ? "Nine Circles" : "Девять кругов",
            ShopItemIds.UnlockSpiderDamageBonus => isEnglish ? "Arachnohate" : "Aрахнофобия",
            ShopItemIds.UnlockZombieDamageBonus => isEnglish ? "Walking Dad" : "Xодячий дед",
            ShopItemIds.UnlockPassiveHealthRegeneration => isEnglish ? "Time Heals" : "Время лечит",
            ShopItemIds.UnlockRobocopRegeneration => isEnglish ? "Robocop" : "Робокоп",
            ShopItemIds.UnlockVampireAbility => isEnglish ? "Dracula" : "Дракула",
            ShopItemIds.UnlockOnePunchManAbility => isEnglish ? "One Punch " : "Ван Панч Мэн",
            ShopItemIds.UnlockBossDamageBonus => isEnglish ? "Godslayer" : "ГодСлэер",
            _ => string.Empty
        };
    }

    private bool ProcessPayment(ShopItemData item)
    {
        return WalletManager.Instance != null &&
               WalletManager.Instance.TrySpendCoins(item.CurrencyType, item.Price);
    }

    private ShopItemData FindShopItem(string itemId)
    {
        if (_shopManager == null)
        {
            _shopManager = Object.FindFirstObjectByType<ShopManager>();
        }

        if (_shopManager == null || _shopManager.ShopItems == null)
        {
            return null;
        }

        return _shopManager.ShopItems.Find(item => item.ItemId == itemId);
    }

    private void ApplyItemEffect(string itemId)
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

                RefreshAllPickups();

                if (_shopManager != null)
                {
                    _shopManager.Invoke(nameof(RefreshAllPickups), 0.2f);
                }

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

            case ShopItemIds.UnlockZombieDamageBonus:
                _abilityManager.UnlockZombieDamageBonus();

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

    private void RefreshAllPickups()
    {
        var pickups = Object.FindObjectsByType<HealthPickup>(FindObjectsSortMode.None);

        foreach (var pickup in pickups)
        {
            if (pickup != null)
            {
                pickup.RefreshPickupState();
            }
        }
    }

    private void SavePurchase(string itemId)
    {
        if (itemId == ShopItemIds.RestoreArmor)
        {
            int uses = PlayerPrefs.GetInt("ArmorPlates_Used", 0);

            PlayerPrefs.SetInt("ArmorPlates_Used", uses + 1);
            PlayerPrefs.Save();

            return;
        }

        if (itemId != ShopItemIds.ActivateLastChance)
        {
            SaveSystem.Instance?.MarkItemPurchased(itemId);
        }

        if (_abilityManager != null)
        {
            SaveSystem.Instance?.UpdateAbilityData(_abilityManager);
            ShopSaveManager.Instance?.OnItemPurchased(itemId, _abilityManager);
        }

        SaveGameAfterPurchase();
    }

    private void SaveGameAfterPurchase()
    {
        if (SaveSystem.Instance == null || _hero == null)
        {
            return;
        }

        SaveSystem.Instance.SaveGame(string.Empty, _hero.transform.position);
    }

    private void ShowArmorRestoredEffect()
    {
        if (_hero == null)
        {
            return;
        }

        GameObject effect = new GameObject("ArmorRestoredEffect");

        effect.transform.position = _hero.transform.position + Vector3.up * EffectVerticalOffset;

        TextMesh textMesh = effect.AddComponent<TextMesh>();

        bool isEnglish = LocalizationManager.CurrentLanguage == LocalizationManager.Language.English;
        textMesh.text = isEnglish ? "armor restored" : "броня восстановлена";

        textMesh.color = Color.cyan;
        textMesh.fontSize = EffectFontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontStyle = FontStyle.Bold;

        Object.Destroy(effect, EffectDestroyDelay);
    }
}