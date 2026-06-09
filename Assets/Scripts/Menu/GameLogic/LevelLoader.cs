using DoorControl;
using Player.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (currentSceneName != saveData.sceneName)
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

        if (saveData.playerHealth > 0)
        {
            player.SetHealth(saveData.playerHealth);
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

        if (saveData.abilityData != null && saveData.abilityData.hasArmor)
        {
            armorManager.UnlockArmorAbility();
        }

        armorManager.LoadArmorFromSave(saveData.playerArmor);
    }

    private void ApplyAbilityData(Hero player, GameSaveData saveData)
    {
        if (player == null || player.AbilityManager == null || saveData?.abilityData == null)
        {
            return;
        }

        AbilityManager abilityManager = player.AbilityManager;
        AbilitySaveData abilityData = saveData.abilityData;

        if (abilityData.hasDash)
        {
            abilityManager.UnlockDash();
        }

        if (abilityData.hasAnatomy)
        {
            abilityManager.UnlockAnatomy();
        }

        if (abilityData.hasMap)
        {
            abilityManager.UnlockMap();
        }

        if (abilityData.hasArmor)
        {
            abilityManager.UnlockArmor();
        }

        if (abilityData.hasSwampDamageBonus)
        {
            abilityManager.UnlockSwampDamageBonus();
        }

        if (abilityData.hasSkeletonDamageBonus)
        {
            abilityManager.UnlockSkeletonDamageBonus();
        }

        if (abilityData.hasDemonDamageBonus)
        {
            abilityManager.UnlockDemonDamageBonus();
        }

        if (abilityData.hasSpiderDamageBonus)
        {
            abilityManager.UnlockSpiderDamageBonus();
        }

        if (abilityData.hasZombieDamageBonus)
        {
            abilityManager.UnlockZombieDamageBonus();
        }

        if (abilityData.hasBossDamageBonus)
        {
            abilityManager.UnlockBossDamageBonus();
        }

        if (abilityData.hasPassiveHealthRegeneration)
        {
            abilityManager.UnlockPassiveHealthRegeneration();
        }

        if (abilityData.hasRobocopRegeneration)
        {
            abilityManager.UnlockRobocopRegeneration();
        }

        if (abilityData.hasVampireAbility)
        {
            abilityManager.UnlockVampireAbility();
        }

        if (abilityData.hasOnePunchManAbility)
        {
            abilityManager.UnlockOnePunchManAbility();
        }

        if (abilityData.isLastChanceActive)
        {
            abilityManager.PurchaseLastChance();
        }
    }

    private void RestoreShopPurchases(GameSaveData saveData)
    {
        if (saveData?.purchasedItemIds == null)
        {
            return;
        }

        ShopManager shopManager = FindFirstObjectByType<ShopManager>();

        if (shopManager != null)
        {
            MarkPurchasedItemsAsSold(shopManager.ShopItems, saveData.purchasedItemIds);
        }

        Hero hero = FindFirstObjectByType<Hero>();

        if (hero == null || hero.AbilityManager == null)
        {
            return;
        }

        foreach (string itemId in saveData.purchasedItemIds)
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

        if (keyCollection != null && saveData.collectedKeys != null)
        {
            keyCollection.LoadCollectedKeys(saveData.collectedKeys);
        }

        EnemyManager.Instance?.SyncWithSaveData();
    }

    private void ApplyCoinData(GameSaveData saveData)
    {
        if (saveData == null || saveData.coins.isInitialized == false)
        {
            return;
        }

        PersistentWallet.Instance?.LoadCoinsFromSave(saveData.coins);

        if (GameLogic.WalletManager.Instance != null)
        {
            GameLogic.WalletManager.Instance.LoadFromSaveData(saveData.coins);
        }
    }

    private Vector3 CalculateSpawnPosition(GameSaveData saveData)
    {
        if (saveData == null)
        {
            return Vector3.zero;
        }

        if (string.IsNullOrEmpty(saveData.checkpointId) == false)
        {
            Vector3 checkpointPosition = FindCheckpointPosition(saveData.checkpointId);

            if (checkpointPosition != Vector3.zero)
            {
                return checkpointPosition;
            }
        }

        return saveData.playerPosition;
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