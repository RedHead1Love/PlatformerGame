using DoorControl;
using GameLogic;
using Player;
using Player.Abilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using YG;

public sealed class SaveSystem : MonoBehaviour
{
    private const string SaveDirectory = "/Saves/";
    private const string SaveFileName = "game_save.json";

    public static SaveSystem Instance { get; private set; }
    public GameSaveData CurrentSave { get; private set; }

    public event Action<GameSaveData> OnGameLoaded;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
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

    private void InitializeSaveDataIfNeeded()
    {
        if (CurrentSave == null)
        {
            CurrentSave = new GameSaveData();
        }
    }

    private void UpdatePlayerData()
    {
        Hero player = FindFirstObjectByType<Hero>();

        if (player != null)
        {
            HealthManager healthManager = player.GetComponent<HealthManager>();

            if (healthManager != null)
            {
                CurrentSave.PlayerHealth = healthManager.CurrentHealth;
            }

            ArmorManager armorManager = player.GetComponent<ArmorManager>();

            if (armorManager != null)
            {
                CurrentSave.PlayerArmor = armorManager.CurrentArmor;
            }
        }
    }

    private void UpdateKeyCollectionData()
    {
        CurrentSave.CollectedKeys.Clear();

        foreach (KeyColor color in Enum.GetValues(typeof(KeyColor)))
        {
            if (GameStateManager.HasKey(color))
            {
                CurrentSave.CollectedKeys.Add(color);
            }
        }
    }

    private void UpdateEnemyData()
    {
        // Логика добавления убитых врагов (синхронизация с EnemyManager)
    }

    private void UpdateCoinData()
    {
        if (PersistentWallet.Instance != null)
        {
            CurrentSave.Coins = PersistentWallet.Instance.CurrentCoins;
        }
    }

    private void SaveToFile()
    {
        string directoryPath = Application.persistentDataPath + SaveDirectory;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = directoryPath + SaveFileName;
        string json = JsonUtility.ToJson(CurrentSave, true);

        File.WriteAllText(filePath, json);
    }

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + SaveDirectory + SaveFileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            CurrentSave = JsonUtility.FromJson<GameSaveData>(json);
            FixNullValues();

            OnGameLoaded?.Invoke(CurrentSave);
        }
    }

    public void MarkEnemyKilled(string enemyId)
    {
        InitializeSaveDataIfNeeded();

        if (!CurrentSave.KilledEnemies.Contains(enemyId))
        {
            CurrentSave.KilledEnemies.Add(enemyId);
            SaveToFile();
        }
    }

    public void UpdateAbilityData(AbilityManager abilityManager)
    {
        InitializeSaveDataIfNeeded();

        // Логика синхронизации данных абилок

        SaveToFile();
    }

    private void FixNullValues()
    {
        if (CurrentSave == null)
        {
            return;
        }

        CurrentSave.CollectedKeys ??= new List<KeyColor>();
        CurrentSave.KilledEnemies ??= new List<string>();
        CurrentSave.CollectedHealthPickups ??= new List<string>();
        CurrentSave.PurchasedItemIds ??= new List<string>();
        CurrentSave.AbilityData ??= new AbilitySaveData();
    }

    public void DeleteSaveData()
    {
        string filePath = Application.persistentDataPath + SaveDirectory + SaveFileName;

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        CurrentSave = null;
    }

    public bool HasSave()
    {
        string filePath = Application.persistentDataPath + SaveDirectory + SaveFileName;

        return File.Exists(filePath);
    }
}
