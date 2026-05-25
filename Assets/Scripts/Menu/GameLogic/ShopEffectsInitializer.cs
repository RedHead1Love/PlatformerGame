using GameLogic;
using Player;
using Player.Abilities;
using ShopLogic;
using UnityEngine;

public sealed class ShopEffectsInitializer : MonoBehaviour
{
    private const string CommandUnlockMap = "1";
    private const string CommandUnlockDash = "2";
    private const string CommandUnlockAnatomy = "3";
    private const string CommandUnlockArmor = "4";
    private const string CommandUnlockSwampDamageBonus = "5";
    private const string CommandUnlockSkeletonDamageBonus = "8";
    private const string CommandUnlockDemonDamageBonus = "9";
    private const string CommandUnlockSpiderDamageBonus = "10";
    private const string CommandUnlockZombieDamageBonus = "11";
    private const string CommandUnlockPassiveHealthRegeneration = "12";
    private const string CommandUnlockRobocopRegeneration = "13";
    private const string CommandUnlockVampireAbility = "14";
    private const string CommandUnlockOnePunchManAbility = "15";
    private const string CommandUnlockBossDamageBonus = "16";

    [SerializeField] private ShopManager _shopManager;

    private void Start()
    {
        InitializeShopEffects();
    }

    public void InitializeShopEffects()
    {
        if (_shopManager == null)
        {
            _shopManager = FindFirstObjectByType<ShopManager>();

            if (_shopManager == null)
            {
                return;
            }
        }

        var hero = FindFirstObjectByType<Hero>();

        if (hero == null || hero.AbilityManager == null)
        {
            return;
        }

        var abilityManager = hero.AbilityManager;

        ApplyPurchasedEffects(abilityManager);
    }

    private void ApplyPurchasedEffects(AbilityManager abilityManager)
    {
        string purchasedKeysString = PlayerPrefs.GetString("ShopItemKeys", "");

        if (string.IsNullOrEmpty(purchasedKeysString))
        {
            return;
        }

        string[] purchasedKeys = purchasedKeysString.Split(',');

        foreach (var key in purchasedKeys)
        {
            if (!string.IsNullOrEmpty(key) && ShopSaveManager.Instance.IsItemPurchased(key))
            {
                ApplyEffect(key, abilityManager);
            }
        }
    }

    private void ApplyEffect(string itemId, AbilityManager abilityManager)
    {
        switch (itemId)
        {
            case CommandUnlockMap:
                if (!abilityManager.HasMap) abilityManager.UnlockMap();
                break;

            case CommandUnlockDash:
                if (!abilityManager.HasDash) abilityManager.UnlockDash();
                break;

            case CommandUnlockAnatomy:
                if (!abilityManager.HasAnatomy) abilityManager.UnlockAnatomy();
                break;

            case CommandUnlockArmor:
                if (!abilityManager.HasArmor) abilityManager.UnlockArmor();
                break;

            case CommandUnlockSwampDamageBonus:
                if (!abilityManager.HasSwampDamageBonus) abilityManager.UnlockSwampDamageBonus();
                break;

            case CommandUnlockSkeletonDamageBonus:
                if (!abilityManager.HasSkeletonDamageBonus) abilityManager.UnlockSkeletonDamageBonus();
                break;

            case CommandUnlockDemonDamageBonus:
                if (!abilityManager.HasDemonDamageBonus) abilityManager.UnlockDemonDamageBonus();
                break;

            case CommandUnlockSpiderDamageBonus:
                if (!abilityManager.HasSpiderDamageBonus) abilityManager.UnlockSpiderDamageBonus();
                break;

            case CommandUnlockZombieDamageBonus:
                if (!abilityManager.HasZombieDamageBonus) abilityManager.UnlockZombieDamageBonus();
                break;

            case CommandUnlockPassiveHealthRegeneration:
                if (!abilityManager.HasPassiveHealthRegeneration) abilityManager.UnlockPassiveHealthRegeneration();
                break;

            case CommandUnlockRobocopRegeneration:
                if (!abilityManager.HasRobocopRegeneration) abilityManager.UnlockRobocopRegeneration();
                break;

            case CommandUnlockVampireAbility:
                if (!abilityManager.HasVampireAbility) abilityManager.UnlockVampireAbility();
                break;

            case CommandUnlockOnePunchManAbility:
                if (!abilityManager.HasOnePunchManAbility) abilityManager.UnlockOnePunchManAbility();
                break;

            case CommandUnlockBossDamageBonus:
                if (!abilityManager.HasBossDamageBonus) abilityManager.UnlockBossDamageBonus();
                break;
        }
    }
}
