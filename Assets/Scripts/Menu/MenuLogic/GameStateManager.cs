using DoorControl;
using UnityEngine;

public sealed class GameStateManager : MonoBehaviour
{
    private const string ChestOpenedPrefix = "Chest_";
    private const string ChestOpenedSuffix = "_Opened";
    private const string KeySpawnedSuffix = "_KeySpawned";
    private const string KeyPrefix = "Key_";
    private const string GameSavedKey = "GameSaved";

    private const int TrueValue = 1;
    private const int FalseValue = 0;
    private const int MaxChestCount = 100;

    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    public static bool IsChestOpened(string chestId)
    {
        return PlayerPrefs.GetInt(GetChestOpenedKey(chestId), FalseValue) == TrueValue;
    }

    public static void SetChestOpened(string chestId, bool isOpened)
    {
        PlayerPrefs.SetInt(GetChestOpenedKey(chestId), isOpened ? TrueValue : FalseValue);
        PlayerPrefs.Save();
    }

    public static bool IsKeySpawned(string chestId)
    {
        return PlayerPrefs.GetInt(GetKeySpawnedKey(chestId), FalseValue) == TrueValue;
    }

    public static void SetKeySpawned(string chestId, bool isSpawned)
    {
        PlayerPrefs.SetInt(GetKeySpawnedKey(chestId), isSpawned ? TrueValue : FalseValue);
        PlayerPrefs.Save();
    }

    public static void ResetGameState()
    {
        ClearAllKeyData();
        ClearAllChestData();

        PlayerPrefs.DeleteKey(GameSavedKey);
        PlayerPrefs.Save();
    }

    public static void MarkGameSaved()
    {
        PlayerPrefs.SetInt(GameSavedKey, TrueValue);
        PlayerPrefs.Save();
    }

    public static bool HasKey(KeyColor color)
    {
        return PlayerPrefs.GetInt(GetKeyColorKey(color), FalseValue) == TrueValue;
    }

    public static void AddKey(KeyColor color)
    {
        PlayerPrefs.SetInt(GetKeyColorKey(color), TrueValue);
        PlayerPrefs.Save();
    }

    public static void RemoveKey(KeyColor color)
    {
        PlayerPrefs.DeleteKey(GetKeyColorKey(color));
        PlayerPrefs.Save();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            return;
        }

        Destroy(gameObject);
    }

    private static string GetChestOpenedKey(string chestId)
    {
        return $"{ChestOpenedPrefix}{chestId}{ChestOpenedSuffix}";
    }

    private static string GetKeySpawnedKey(string chestId)
    {
        return $"{ChestOpenedPrefix}{chestId}{KeySpawnedSuffix}";
    }

    private static string GetKeyColorKey(KeyColor color)
    {
        return $"{KeyPrefix}{color}";
    }

    private static void ClearAllKeyData()
    {
        foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
        {
            PlayerPrefs.DeleteKey(GetKeyColorKey(color));
        }
    }

    private static void ClearAllChestData()
    {
        for (int i = 0; i < MaxChestCount; i++)
        {
            PlayerPrefs.DeleteKey(GetChestOpenedKey(i.ToString()));
            PlayerPrefs.DeleteKey(GetKeySpawnedKey(i.ToString()));
        }

        PlayerPrefs.DeleteKey($"{ChestOpenedPrefix}{ChestOpenedSuffix}");
        PlayerPrefs.DeleteKey($"{ChestOpenedPrefix}{KeySpawnedSuffix}");
    }
}