using DoorControl;
using GameLogic;
using Player.Abilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public sealed class SaveSystem : MonoBehaviour
{
    private const string SaveDirectory = "/Saves/";
    private const string SaveFileName = "game_save.json";

    public static SaveSystem Instance { get; private set; }

    public GameSaveData CurrentSave { get; private set; }

    public event Action<GameSaveData> OnGameLoaded;

    private string SaveDirectoryPath => Application.persistentDataPath + SaveDirectory;
    private string SaveFilePath => SaveDirectoryPath + SaveFileName;

    private void Awake()
    {
        InitializeSingleton();
    }

    public void SaveGame(string checkpointId, Vector3 playerPosition)
    {
        InitializeSaveDataIfNeeded();

        CurrentSave.saveId = Guid.NewGuid().ToString();
        CurrentSave.saveTime = DateTime.Now;
        CurrentSave.checkpointId = checkpointId;
        CurrentSave.playerPosition = playerPosition;
        CurrentSave.sceneName = SceneManager.GetActiveScene().name;

        UpdatePlayerData();
        UpdateKeyCollectionData();
        UpdateEnemyData();
        UpdateCoinData();

        SaveToFile();

        YG2.InterstitialAdvShow();
    }

    public void MarkEnemyKilled(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId))
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        if (CurrentSave.killedEnemies.Contains(enemyId))
        {
            return;
        }

        CurrentSave.killedEnemies.Add(enemyId);
    }

    public void MarkHealthPickupCollected(string pickupId)
    {
        if (string.IsNullOrEmpty(pickupId))
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        if (CurrentSave.collectedHealthPickups.Contains(pickupId))
        {
            return;
        }

        CurrentSave.collectedHealthPickups.Add(pickupId);
    }

    public void MarkItemPurchased(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        if (CurrentSave.purchasedItemIds.Contains(itemId))
        {
            return;
        }

        CurrentSave.purchasedItemIds.Add(itemId);

        SaveToFile();
    }

    public void UpdateAbilityData(AbilityManager abilityManager)
    {
        if (abilityManager == null)
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        CurrentSave.abilityData.hasDash = abilityManager.HasDash;
        CurrentSave.abilityData.hasDoubleJump = abilityManager.HasDoubleJump;
        CurrentSave.abilityData.hasMap = abilityManager.HasMap;
        CurrentSave.abilityData.hasAnatomy = abilityManager.HasAnatomy;
        CurrentSave.abilityData.hasArmor = abilityManager.HasArmor;
        CurrentSave.abilityData.hasSwampDamageBonus = abilityManager.HasSwampDamageBonus;
        CurrentSave.abilityData.hasSkeletonDamageBonus = abilityManager.HasSkeletonDamageBonus;
        CurrentSave.abilityData.hasDemonDamageBonus = abilityManager.HasDemonDamageBonus;
        CurrentSave.abilityData.hasSpiderDamageBonus = abilityManager.HasSpiderDamageBonus;
        CurrentSave.abilityData.hasZombieDamageBonus = abilityManager.HasZombieDamageBonus;
        CurrentSave.abilityData.hasBossDamageBonus = abilityManager.HasBossDamageBonus;
        CurrentSave.abilityData.isLastChanceActive = abilityManager.IsLastChanceActive;
        CurrentSave.abilityData.hasPassiveHealthRegeneration = abilityManager.HasPassiveHealthRegeneration;
        CurrentSave.abilityData.hasRobocopRegeneration = abilityManager.HasRobocopRegeneration;
        CurrentSave.abilityData.hasVampireAbility = abilityManager.HasVampireAbility;
        CurrentSave.abilityData.hasOnePunchManAbility = abilityManager.HasOnePunchManAbility;

        SaveToFile();
    }

    public void LoadGameData()
    {
        if (File.Exists(SaveFilePath))
        {
            string jsonData = File.ReadAllText(SaveFilePath);

            CurrentSave = JsonUtility.FromJson<GameSaveData>(jsonData);
            FixNullValues();
        }
        else
        {
            CurrentSave = null;
        }

        OnGameLoaded?.Invoke(CurrentSave);
    }

    public void DeleteSaveData()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }

        CurrentSave = null;
    }

    public bool HasSave()
    {
        return File.Exists(SaveFilePath);
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            LoadGameData();

            return;
        }

        Destroy(gameObject);
    }

    private void InitializeSaveDataIfNeeded()
    {
        if (CurrentSave == null)
        {
            CurrentSave = new GameSaveData();
        }

        CurrentSave.EnsureValidCollections();
    }

    private void UpdatePlayerData()
    {
        Hero player = FindFirstObjectByType<Hero>();

        if (player == null)
        {
            return;
        }

        CurrentSave.playerHealth = player.Lives;

        ArmorManager armorManager = player.GetComponent<ArmorManager>();
        CurrentSave.playerArmor = armorManager != null ? armorManager.CurrentArmor : 0;

        if (player.AbilityManager != null)
        {
            CurrentSave.abilityData.hasDash = player.AbilityManager.HasDash;
            CurrentSave.abilityData.hasDoubleJump = player.AbilityManager.HasDoubleJump;
            CurrentSave.abilityData.hasMap = player.AbilityManager.HasMap;
            CurrentSave.abilityData.hasAnatomy = player.AbilityManager.HasAnatomy;
            CurrentSave.abilityData.hasArmor = player.AbilityManager.HasArmor;
            CurrentSave.abilityData.hasSwampDamageBonus = player.AbilityManager.HasSwampDamageBonus;
            CurrentSave.abilityData.hasSkeletonDamageBonus = player.AbilityManager.HasSkeletonDamageBonus;
            CurrentSave.abilityData.hasDemonDamageBonus = player.AbilityManager.HasDemonDamageBonus;
            CurrentSave.abilityData.hasSpiderDamageBonus = player.AbilityManager.HasSpiderDamageBonus;
            CurrentSave.abilityData.hasZombieDamageBonus = player.AbilityManager.HasZombieDamageBonus;
            CurrentSave.abilityData.hasBossDamageBonus = player.AbilityManager.HasBossDamageBonus;
            CurrentSave.abilityData.isLastChanceActive = player.AbilityManager.IsLastChanceActive;
            CurrentSave.abilityData.hasPassiveHealthRegeneration = player.AbilityManager.HasPassiveHealthRegeneration;
            CurrentSave.abilityData.hasRobocopRegeneration = player.AbilityManager.HasRobocopRegeneration;
            CurrentSave.abilityData.hasVampireAbility = player.AbilityManager.HasVampireAbility;
            CurrentSave.abilityData.hasOnePunchManAbility = player.AbilityManager.HasOnePunchManAbility;
        }
    }

    private void UpdateKeyCollectionData()
    {
        KeyCollection keyCollection = FindFirstObjectByType<KeyCollection>();

        if (keyCollection != null)
        {
            CurrentSave.collectedKeys = keyCollection.GetCollectedKeys();
        }
    }

    private void UpdateEnemyData()
    {
        if (EnemyManager.Instance == null)
        {
            CurrentSave.killedEnemies = new List<string>();

            return;
        }

        CurrentSave.killedEnemies = new List<string>(EnemyManager.Instance.GetKilledEnemies());
    }

    private void UpdateCoinData()
    {
        if (PersistentWallet.Instance != null)
        {
            CurrentSave.coins = PersistentWallet.Instance.CurrentCoins;

            return;
        }

        if (WalletManager.Instance == null)
        {
            return;
        }

        CurrentSave.coins = new CoinData(
            WalletManager.Instance.GetCoins(WalletManager.CoinType.Bronze),
            WalletManager.Instance.GetCoins(WalletManager.CoinType.Silver),
            WalletManager.Instance.GetCoins(WalletManager.CoinType.Gold));
    }

    private void SaveToFile()
    {
        if (CurrentSave == null)
        {
            return;
        }

        CreateDirectoryIfNotExists(SaveDirectoryPath);

        string jsonData = JsonUtility.ToJson(CurrentSave, true);

        File.WriteAllText(SaveFilePath, jsonData);
    }

    private void CreateDirectoryIfNotExists(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            return;
        }

        Directory.CreateDirectory(directoryPath);
    }

    private void FixNullValues()
    {
        if (CurrentSave == null)
        {
            return;
        }

        CurrentSave.EnsureValidCollections();
    }
}