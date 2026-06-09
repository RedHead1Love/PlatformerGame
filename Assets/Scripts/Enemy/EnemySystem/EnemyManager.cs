using GeneralLogicEnemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class EnemyManager : MonoBehaviour
{
    private const float SaveSyncDelay = 0.5f;
    private const int RandomIdMinValue = 1000;
    private const int RandomIdMaxValue = 9999;
    private const string InvalidIdentifier = "-1";

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

    public void RegisterEnemy(Entity enemy, string enemyId)
    {
        if (enemy == null)
        {
            return;
        }

        string validId = CreateValidEnemyId(enemy, enemyId);

        if (_enemies.ContainsKey(validId))
        {
            _enemies[validId] = enemy;
        }
        else
        {
            _enemies.Add(validId, enemy);
        }

        if (_killedEnemies.Contains(validId))
        {
            enemy.gameObject.SetActive(false);
        }
    }

    public void RemoveEnemy(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId))
        {
            return;
        }

        _enemies.Remove(enemyId);
    }

    public void MarkEnemyKilled(string enemyId)
    {
        if (IsInvalidEnemyId(enemyId))
        {
            return;
        }

        if (_killedEnemies.Add(enemyId) == false)
        {
            return;
        }

        SaveSystem.Instance?.MarkEnemyKilled(enemyId);

        _enemies.Remove(enemyId);
    }

    public HashSet<string> GetKilledEnemies()
    {
        return new HashSet<string>(_killedEnemies);
    }

    public void SyncWithSaveData()
    {
        if (SaveSystem.Instance == null || SaveSystem.Instance.HasSave() == false)
        {
            return;
        }

        GameSaveData saveData = SaveSystem.Instance.CurrentSave;

        if (saveData?.killedEnemies == null)
        {
            return;
        }

        _killedEnemies = new HashSet<string>(saveData.killedEnemies);

        ApplyKilledEnemiesStates();
    }

    public void ResetAllEnemies()
    {
        _killedEnemies.Clear();
        _enemies.Clear();

        Entity[] allEnemies = FindObjectsByType<Entity>(FindObjectsSortMode.None);

        foreach (Entity enemy in allEnemies)
        {
            if (IsEnemyValid(enemy))
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }

    public bool IsEnemyAlive(string enemyId)
    {
        if (IsInvalidEnemyId(enemyId) || _killedEnemies.Contains(enemyId))
        {
            return false;
        }

        return _enemies.ContainsKey(enemyId) &&
               IsEnemyValid(_enemies[enemyId]) &&
               _enemies[enemyId].gameObject.activeInHierarchy;
    }

    public void ResetForSceneRestart()
    {
        _enemies.Clear();

        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            GameSaveData saveData = SaveSystem.Instance.CurrentSave;

            _killedEnemies = saveData?.killedEnemies != null
                ? new HashSet<string>(saveData.killedEnemies)
                : new HashSet<string>();
        }
        else
        {
            _killedEnemies.Clear();
        }
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            SubscribeToEvents();

            return;
        }

        Destroy(gameObject);
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
            StartCoroutine(DelayedSaveSync());
        }
    }

    private IEnumerator DelayedSaveSync()
    {
        yield return new WaitForSeconds(SaveSyncDelay);

        SyncWithSaveData();
    }

    private void OnGameLoaded(GameSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        _killedEnemies = saveData.killedEnemies != null
            ? new HashSet<string>(saveData.killedEnemies)
            : new HashSet<string>();

        ApplyKilledEnemiesStates();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsRestartingSameScene(scene.name))
        {
            ResetForSceneRestart();
        }
    }

    private bool IsRestartingSameScene(string newSceneName)
    {
        if (SaveSystem.Instance == null || SaveSystem.Instance.HasSave() == false)
        {
            return false;
        }

        string savedSceneName = SaveSystem.Instance.CurrentSave.sceneName;

        return newSceneName == savedSceneName;
    }

    private void ApplyKilledEnemiesStates()
    {
        foreach (string enemyId in _killedEnemies)
        {
            if (_enemies.TryGetValue(enemyId, out Entity enemy) && IsEnemyValid(enemy))
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }

    private string CreateValidEnemyId(Entity enemy, string enemyId)
    {
        if (IsInvalidEnemyId(enemyId) == false)
        {
            return enemyId;
        }

        string generatedId = $"{enemy.gameObject.name}_{Random.Range(RandomIdMinValue, RandomIdMaxValue)}";

        while (_enemies.ContainsKey(generatedId))
        {
            generatedId = $"{enemy.gameObject.name}_{Random.Range(RandomIdMinValue, RandomIdMaxValue)}";
        }

        return generatedId;
    }

    private bool IsInvalidEnemyId(string enemyId)
    {
        return string.IsNullOrEmpty(enemyId) || enemyId.Contains(InvalidIdentifier);
    }

    private bool IsEnemyValid(Entity enemy)
    {
        return enemy != null && enemy.gameObject != null;
    }
}