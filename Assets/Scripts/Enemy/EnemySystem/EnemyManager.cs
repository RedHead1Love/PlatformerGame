using GeneralLogicEnemies;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly Dictionary<string, Entity> _enemies = new Dictionary<string, Entity>();
    private HashSet<string> _killedEnemies = new HashSet<string>();

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        InitializeSaveSystemConnection();
    }

    private void OnDestroy()
    {
        CleanupEventSubscriptions();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SubscribeToEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SubscribeToEvents()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.OnGameLoaded += OnGameLoaded;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void CleanupEventSubscriptions()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.OnGameLoaded -= OnGameLoaded;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeSaveSystemConnection()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            SyncWithSaveData();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncWithSaveData();
    }

    private void OnGameLoaded(GameSaveData saveData)
    {
        SyncWithSaveData();
    }

    public void RegisterEnemy(Entity enemy, string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId) || enemy == null)
        {
            return;
        }

        _enemies[enemyId] = enemy;

        if (_killedEnemies.Contains(enemyId))
        {
            enemy.gameObject.SetActive(false);
        }
    }

    public void RemoveEnemy(string enemyId)
    {
        if (_enemies.ContainsKey(enemyId))
        {
            _enemies.Remove(enemyId);
        }
    }

    public void MarkEnemyKilled(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId))
        {
            return;
        }

        _killedEnemies.Add(enemyId);

        ApplyKilledEnemiesStates();
    }

    public void SyncWithSaveData()
    {
        if (SaveSystem.Instance == null || !SaveSystem.Instance.HasSave())
        {
            return;
        }

        var saveData = SaveSystem.Instance.CurrentSave;

        if (saveData.KilledEnemies != null)
        {
            _killedEnemies = new HashSet<string>(saveData.KilledEnemies);

            ApplyKilledEnemiesStates();
        }
    }

    public void ResetAllEnemies()
    {
        _killedEnemies.Clear();
        _enemies.Clear();

        var allEnemies = FindObjectsByType<Entity>(FindObjectsSortMode.None);

        foreach (var enemy in allEnemies)
        {
            if (IsEnemyValid(enemy))
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }

    public bool IsEnemyAlive(string enemyId)
    {
        if (_killedEnemies.Contains(enemyId))
        {
            return false;
        }

        return _enemies.TryGetValue(enemyId, out Entity enemy) && IsEnemyValid(enemy) && enemy.gameObject.activeInHierarchy;
    }

    private void ApplyKilledEnemiesStates()
    {
        foreach (var enemyId in _killedEnemies)
        {
            if (_enemies.TryGetValue(enemyId, out Entity enemy) && IsEnemyValid(enemy))
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }

    private bool IsEnemyValid(Entity enemy)
    {
        return enemy != null;
    }
}
