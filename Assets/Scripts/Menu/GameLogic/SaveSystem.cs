using DoorControl;
using GameLogic;
using Player.Abilities;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
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

        CurrentSave.SaveId = Guid.NewGuid().ToString();
        CurrentSave.SaveTime = DateTime.Now;
        CurrentSave.CheckpointId = checkpointId;
        CurrentSave.PlayerPosition = playerPosition;
        CurrentSave.SceneName = SceneManager.GetActiveScene().name;

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

        if (CurrentSave.KilledEnemies.Contains(enemyId))
        {
            return;
        }

        CurrentSave.KilledEnemies.Add(enemyId);
    }

    public void MarkHealthPickupCollected(string pickupId)
    {
        if (string.IsNullOrEmpty(pickupId))
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        if (CurrentSave.CollectedHealthPickups.Contains(pickupId))
        {
            return;
        }

        CurrentSave.CollectedHealthPickups.Add(pickupId);
    }

    public void MarkItemPurchased(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        if (CurrentSave.PurchasedItemIds.Contains(itemId))
        {
            return;
        }

        CurrentSave.PurchasedItemIds.Add(itemId);

        SaveToFile();
    }

    public void UpdateAbilityData(AbilityManager abilityManager)
    {
        if (abilityManager == null)
        {
            return;
        }

        InitializeSaveDataIfNeeded();

        CurrentSave.AbilityData.HasDash = abilityManager.HasDash;
        CurrentSave.AbilityData.HasDoubleJump = abilityManager.HasDoubleJump;
        CurrentSave.AbilityData.HasMap = abilityManager.HasMap;
        CurrentSave.AbilityData.HasAnatomy = abilityManager.HasAnatomy;
        CurrentSave.AbilityData.HasArmor = abilityManager.HasArmor;
        CurrentSave.AbilityData.HasSwampDamageBonus = abilityManager.HasSwampDamageBonus;
        CurrentSave.AbilityData.HasSkeletonDamageBonus = abilityManager.HasSkeletonDamageBonus;
        CurrentSave.AbilityData.HasDemonDamageBonus = abilityManager.HasDemonDamageBonus;
        CurrentSave.AbilityData.HasSpiderDamageBonus = abilityManager.HasSpiderDamageBonus;
        CurrentSave.AbilityData.HasZombieDamageBonus = abilityManager.HasZombieDamageBonus;
        CurrentSave.AbilityData.HasBossDamageBonus = abilityManager.HasBossDamageBonus;
        CurrentSave.AbilityData.IsLastChanceActive = abilityManager.IsLastChanceActive;
        CurrentSave.AbilityData.HasPassiveHealthRegeneration = abilityManager.HasPassiveHealthRegeneration;
        CurrentSave.AbilityData.HasRobocopRegeneration = abilityManager.HasRobocopRegeneration;
        CurrentSave.AbilityData.HasVampireAbility = abilityManager.HasVampireAbility;
        CurrentSave.AbilityData.HasOnePunchManAbility = abilityManager.HasOnePunchManAbility;

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

        CurrentSave.PlayerHealth = player.Lives;

        ArmorManager armorManager = player.GetComponent<ArmorManager>();
        CurrentSave.PlayerArmor = armorManager != null ? armorManager.CurrentArmor : 0;

        if (player.AbilityManager != null)
        {
            CurrentSave.AbilityData.HasDash = player.AbilityManager.HasDash;
            CurrentSave.AbilityData.HasDoubleJump = player.AbilityManager.HasDoubleJump;
            CurrentSave.AbilityData.HasMap = player.AbilityManager.HasMap;
            CurrentSave.AbilityData.HasAnatomy = player.AbilityManager.HasAnatomy;
            CurrentSave.AbilityData.HasArmor = player.AbilityManager.HasArmor;
            CurrentSave.AbilityData.HasSwampDamageBonus = player.AbilityManager.HasSwampDamageBonus;
            CurrentSave.AbilityData.HasSkeletonDamageBonus = player.AbilityManager.HasSkeletonDamageBonus;
            CurrentSave.AbilityData.HasDemonDamageBonus = player.AbilityManager.HasDemonDamageBonus;
            CurrentSave.AbilityData.HasSpiderDamageBonus = player.AbilityManager.HasSpiderDamageBonus;
            CurrentSave.AbilityData.HasZombieDamageBonus = player.AbilityManager.HasZombieDamageBonus;
            CurrentSave.AbilityData.HasBossDamageBonus = player.AbilityManager.HasBossDamageBonus;
            CurrentSave.AbilityData.IsLastChanceActive = player.AbilityManager.IsLastChanceActive;
            CurrentSave.AbilityData.HasPassiveHealthRegeneration = player.AbilityManager.HasPassiveHealthRegeneration;
            CurrentSave.AbilityData.HasRobocopRegeneration = player.AbilityManager.HasRobocopRegeneration;
            CurrentSave.AbilityData.HasVampireAbility = player.AbilityManager.HasVampireAbility;
            CurrentSave.AbilityData.HasOnePunchManAbility = player.AbilityManager.HasOnePunchManAbility;
        }
    }

    private void UpdateKeyCollectionData()
    {
        KeyCollection keyCollection = FindFirstObjectByType<KeyCollection>();

        if (keyCollection != null)
        {
            CurrentSave.CollectedKeys = keyCollection.GetCollectedKeys();
        }
    }

    private void UpdateEnemyData()
    {
        if (EnemyManager.Instance == null)
        {
            CurrentSave.KilledEnemies = new List<string>();

            return;
        }

        CurrentSave.KilledEnemies = new List<string>(EnemyManager.Instance.GetKilledEnemies());
    }

    private void UpdateCoinData()
    {
        if (PersistentWallet.Instance != null)
        {
            CurrentSave.Coins = PersistentWallet.Instance.CurrentCoins;

            return;
        }

        if (WalletManager.Instance == null)
        {
            return;
        }

        CurrentSave.Coins = new CoinData(
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
