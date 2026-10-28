using DoorControl;
using Player.Abilities;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public sealed class LevelLoader : MonoBehaviour
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

    private const float LoadDelay = 0.1f;
    private const float RestoreShopDelay = 0.1f;
    private const float WorldDataApplyDelay = 0.2f;

    private void Start()
    {
        StartCoroutine(LoadSavedGameData());
    }

    private IEnumerator LoadSavedGameData()
    {
        yield return new WaitForSeconds(LoadDelay);

        if (CanLoadGame() == false)
        {
            yield break;
        }

        GameSaveData saveData = SaveSystem.Instance.CurrentSave;
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName != saveData.SceneName)
        {
            yield break;
        }

        yield return StartCoroutine(ApplySavedGameData(saveData));
    }

    private bool CanLoadGame()
    {
        return SaveSystem.Instance != null &&
               SaveSystem.Instance.HasSave() &&
               SaveSystem.Instance.CurrentSave != null;
    }

    private IEnumerator ApplySavedGameData(GameSaveData saveData)
    {
        Hero player = FindFirstObjectByType<Hero>();

        if (player == null)
        {
            yield break;
        }

        Vector3 spawnPosition = CalculateSpawnPosition(saveData);

        ApplyPlayerData(player, spawnPosition, saveData);
        ApplyAbilityData(player, saveData);

        yield return new WaitForSeconds(RestoreShopDelay);

        RestoreShopPurchases(saveData);

        yield return new WaitForSeconds(WorldDataApplyDelay);

        ApplyGameWorldData(saveData);
        ApplyCoinData(saveData);

        SaveSystem.Instance?.UpdateAbilityData(player.AbilityManager);
    }

    private void ApplyPlayerData(Hero player, Vector3 spawnPosition, GameSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        if (spawnPosition != Vector3.zero)
        {
            player.transform.position = spawnPosition;
        }

        if (saveData.PlayerHealth > 0)
        {
            player.SetHealth(saveData.PlayerHealth);
        }

        ApplyArmorData(player, saveData);
    }

    private void ApplyArmorData(Hero player, GameSaveData saveData)
    {
        ArmorManager armorManager = player.GetComponent<ArmorManager>();

        if (armorManager == null)
        {
            return;
        }

        if (saveData.AbilityData != null && saveData.AbilityData.HasArmor)
        {
            armorManager.UnlockArmorAbility();
        }

        armorManager.LoadArmorFromSave(saveData.PlayerArmor);
    }

    private void ApplyAbilityData(Hero player, GameSaveData saveData)
    {
        if (player == null || player.AbilityManager == null || saveData?.AbilityData == null)
        {
            return;
        }

        AbilityManager abilityManager = player.AbilityManager;
        AbilitySaveData abilityData = saveData.AbilityData;

        if (abilityData.HasDash)
        {
            abilityManager.UnlockDash();
        }

        if (abilityData.HasAnatomy)
        {
            abilityManager.UnlockAnatomy();
        }

        if (abilityData.HasMap)
        {
            abilityManager.UnlockMap();
        }

        if (abilityData.HasArmor)
        {
            abilityManager.UnlockArmor();
        }

        if (abilityData.HasSwampDamageBonus)
        {
            abilityManager.UnlockSwampDamageBonus();
        }

        if (abilityData.HasSkeletonDamageBonus)
        {
            abilityManager.UnlockSkeletonDamageBonus();
        }

        if (abilityData.HasDemonDamageBonus)
        {
            abilityManager.UnlockDemonDamageBonus();
        }

        if (abilityData.HasSpiderDamageBonus)
        {
            abilityManager.UnlockSpiderDamageBonus();
        }

        if (abilityData.HasZombieDamageBonus)
        {
            abilityManager.UnlockZombieDamageBonus();
        }

        if (abilityData.HasBossDamageBonus)
        {
            abilityManager.UnlockBossDamageBonus();
        }

        if (abilityData.HasPassiveHealthRegeneration)
        {
            abilityManager.UnlockPassiveHealthRegeneration();
        }

        if (abilityData.HasRobocopRegeneration)
        {
            abilityManager.UnlockRobocopRegeneration();
        }

        if (abilityData.HasVampireAbility)
        {
            abilityManager.UnlockVampireAbility();
        }

        if (abilityData.HasOnePunchManAbility)
        {
            abilityManager.UnlockOnePunchManAbility();
        }

        if (abilityData.IsLastChanceActive)
        {
            abilityManager.PurchaseLastChance();
        }
    }

    private void RestoreShopPurchases(GameSaveData saveData)
    {
        if (saveData?.PurchasedItemIds == null)
        {
            return;
        }

        ShopManager shopManager = FindFirstObjectByType<ShopManager>();

        if (shopManager != null)
        {
            MarkPurchasedItemsAsSold(shopManager.ShopItems, saveData.PurchasedItemIds);
        }

        Hero hero = FindFirstObjectByType<Hero>();

        if (hero == null || hero.AbilityManager == null)
        {
            return;
        }

        foreach (string itemId in saveData.PurchasedItemIds)
        {
            ApplyAbilityEffect(itemId, hero.AbilityManager, hero);
        }
    }

    private void MarkPurchasedItemsAsSold(List<ShopItemData> shopItems, List<string> purchasedItemIds)
    {
        if (shopItems == null)
        {
            return;
        }

        foreach (string itemId in purchasedItemIds)
        {
            ShopItemData item = shopItems.Find(shopItem => shopItem.ItemId == itemId);

            if (item != null && itemId != "6" && itemId != "7")
            {
                item.IsSold = true;
            }
        }
    }

    private void ApplyAbilityEffect(string itemId, AbilityManager abilityManager, Hero hero)
    {
        if (abilityManager == null)
        {
            return;
        }

        switch (itemId)
        {
            case CommandUnlockMap:
                abilityManager.UnlockMap();
                break;

            case CommandUnlockDash:
                abilityManager.UnlockDash();
                break;

            case CommandUnlockAnatomy:
                abilityManager.UnlockAnatomy();
                break;

            case CommandUnlockArmor:
                abilityManager.UnlockArmor();
                hero.GetComponent<ArmorManager>()?.UnlockArmorAbility();
                break;

            case CommandUnlockSwampDamageBonus:
                abilityManager.UnlockSwampDamageBonus();
                break;

            case CommandUnlockSkeletonDamageBonus:
                abilityManager.UnlockSkeletonDamageBonus();
                break;

            case CommandUnlockDemonDamageBonus:
                abilityManager.UnlockDemonDamageBonus();
                break;

            case CommandUnlockSpiderDamageBonus:
                abilityManager.UnlockSpiderDamageBonus();
                break;

            case CommandUnlockZombieDamageBonus:
                abilityManager.UnlockZombieDamageBonus();
                break;

            case CommandUnlockPassiveHealthRegeneration:
                abilityManager.UnlockPassiveHealthRegeneration();
                break;

            case CommandUnlockRobocopRegeneration:
                abilityManager.UnlockRobocopRegeneration();
                break;

            case CommandUnlockVampireAbility:
                abilityManager.UnlockVampireAbility();
                break;

            case CommandUnlockOnePunchManAbility:
                abilityManager.UnlockOnePunchManAbility();
                break;

            case CommandUnlockBossDamageBonus:
                abilityManager.UnlockBossDamageBonus();
                break;
        }
    }

    private void ApplyGameWorldData(GameSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        KeyCollection keyCollection = FindFirstObjectByType<KeyCollection>();

        if (keyCollection != null && saveData.CollectedKeys != null)
        {
            keyCollection.LoadCollectedKeys(saveData.CollectedKeys);
        }

        EnemyManager.Instance?.SyncWithSaveData();
    }

    private void ApplyCoinData(GameSaveData saveData)
    {
        if (saveData == null || saveData.Coins.IsInitialized == false)
        {
            return;
        }

        PersistentWallet.Instance?.LoadCoinsFromSave(saveData.Coins);

        if (GameLogic.WalletManager.Instance != null)
        {
            GameLogic.WalletManager.Instance.LoadFromSaveData(saveData.Coins);
        }
    }

    private Vector3 CalculateSpawnPosition(GameSaveData saveData)
    {
        if (saveData == null)
        {
            return Vector3.zero;
        }

        if (string.IsNullOrEmpty(saveData.CheckpointId) == false)
        {
            Vector3 checkpointPosition = FindCheckpointPosition(saveData.CheckpointId);

            if (checkpointPosition != Vector3.zero)
            {
                return checkpointPosition;
            }
        }

        return saveData.PlayerPosition;
    }

    private Vector3 FindCheckpointPosition(string checkpointId)
    {
        if (string.IsNullOrEmpty(checkpointId))
        {
            return Vector3.zero;
        }

        Checkpoint[] checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.GetCheckpointId() == checkpointId)
            {
                return checkpoint.GetSpawnPosition();
            }
        }

        return Vector3.zero;
    }
}
