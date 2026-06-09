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
        return itemId switch
        {
            ShopItemIds.UnlockMap => "Карта - показывает всю карту игры",
            ShopItemIds.UnlockDash => "Рывок - быстрое перемещение",
            ShopItemIds.UnlockAnatomy => "Анатомия - позволяет подбирать аптечки",
            ShopItemIds.UnlockArmor => "Броня - открывает шкалу брони",
            ShopItemIds.UnlockSwampDamageBonus => "И на болоте хорошо - +100% урона по болотным врагам",
            ShopItemIds.ActivateLastChance => "Последний шанс - выживание при смертельном ударе",
            ShopItemIds.RestoreArmor => "Пластины брони - восстановление брони до максимума",
            ShopItemIds.UnlockSkeletonDamageBonus => "Несвежее мясо - +100% урона по скелетам",
            ShopItemIds.UnlockDemonDamageBonus => "Девять кругов - +100% урона по демонам",
            ShopItemIds.UnlockSpiderDamageBonus => "Арахнофобия - +100% урона по паукам",
            ShopItemIds.UnlockZombieDamageBonus => "По имени Шон - +100% урона по зомби",
            ShopItemIds.UnlockPassiveHealthRegeneration => "Спасение утопающего - пассивное восстановление здоровья",
            ShopItemIds.UnlockRobocopRegeneration => "Робокоп - пассивное восстановление брони",
            ShopItemIds.UnlockVampireAbility => "Дракула - получение здоровья за убийство врагов",
            ShopItemIds.UnlockOnePunchManAbility => "Ван Панч Мэн - шанс мгновенного убийства врага",
            ShopItemIds.UnlockBossDamageBonus => "ГодСлэер - +100% урона по боссам",
            _ => string.Empty
        };
    }

    public string GetItemName(string itemId)
    {
        return itemId switch
        {
            ShopItemIds.UnlockMap => "Карта",
            ShopItemIds.UnlockDash => "Рывок",
            ShopItemIds.UnlockAnatomy => "Анатомия",
            ShopItemIds.UnlockArmor => "Броня",
            ShopItemIds.UnlockSwampDamageBonus => "И на болоте хорошо",
            ShopItemIds.ActivateLastChance => "Последний шанс",
            ShopItemIds.RestoreArmor => "Пластины брони",
            ShopItemIds.UnlockSkeletonDamageBonus => "Несвежее мясо",
            ShopItemIds.UnlockDemonDamageBonus => "Девять кругов",
            ShopItemIds.UnlockSpiderDamageBonus => "Арахнофобия",
            ShopItemIds.UnlockZombieDamageBonus => "По имени Шон",
            ShopItemIds.UnlockPassiveHealthRegeneration => "Спасение утопающего",
            ShopItemIds.UnlockRobocopRegeneration => "Робокоп",
            ShopItemIds.UnlockVampireAbility => "Дракула",
            ShopItemIds.UnlockOnePunchManAbility => "Ван Панч Мэн",
            ShopItemIds.UnlockBossDamageBonus => "ГодСлэер",
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

        textMesh.text = "броня восстановлена";
        textMesh.color = Color.cyan;
        textMesh.fontSize = EffectFontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontStyle = FontStyle.Bold;

        Object.Destroy(effect, EffectDestroyDelay);
    }
}