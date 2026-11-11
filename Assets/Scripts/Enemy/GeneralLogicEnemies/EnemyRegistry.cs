using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeneralLogicEnemies
{
    public sealed class EnemyRegistry : MonoBehaviour
    {
        private const float SaveSyncDelay = 0.5f;
        private const int RandomIdMinValue = 1000;
        private const int RandomIdMaxValue = 9999;
        private const string InvalidIdentifier = "-1";

        private readonly Dictionary<string, Entity> _enemies = new Dictionary<string, Entity>();

        private HashSet<string> _killedEnemies = new HashSet<string>();

        public static event System.Action OnEnemyKilledGlobal;

        public static EnemyRegistry Instance { get; private set; }

        public void RegisterEnemy(Entity enemy, string enemyId)
        {
            if (enemy == null)
            {
                return;
            }

            string validId = CreateValidEnemyId(enemy, enemyId);

            _enemies[validId] = enemy;

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

            OnEnemyKilledGlobal?.Invoke();

            _save?.MarkEnemyKilled(enemyId);

            _enemies.Remove(enemyId);
        }

        public HashSet<string> GetKilledEnemies()
        {
            return new HashSet<string>(_killedEnemies);
        }

        public void SyncWithSaveData()
        {
            if (_save == null || _save.HasSave() == false)
            {
                return;
            }

            GameSaveData saveData = _save.CurrentSave;

            if (saveData?.DefeatedEnemies == null)
            {
                return;
            }

            _killedEnemies = new HashSet<string>(saveData.DefeatedEnemies);

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

            return _enemies.ContainsKey(enemyId) && IsEnemyValid(_enemies[enemyId]) &&
                   _enemies[enemyId].gameObject.activeInHierarchy;
        }

        public void ResetForSceneRestart()
        {
            _enemies.Clear();

            if (_save != null && _save.HasSave())
            {
                GameSaveData saveData = _save.CurrentSave;

                _killedEnemies = saveData?.DefeatedEnemies != null ? new HashSet<string>(saveData.DefeatedEnemies)
                                                                   : new HashSet<string>();
            }
            else
            {
                _killedEnemies.Clear();
            }
        }

        private IGameProgressSave _save;

        private void BindSave(IGameProgressSave save)
        {
            if (_save != null)
            {
                _save.OnGameLoaded -= OnGameLoaded;
            }

            _save = save;

            if (_save != null)
            {
                _save.OnGameLoaded += OnGameLoaded;
                OnGameLoaded(_save.CurrentSave);
            }
        }

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
            GameSaveConnections.Disconnect(BindSave);
            if (Instance == this)
            {
                Instance = null;
            }
            CleanupEventSubscriptions();
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                GameSaveConnections.Connect(BindSave);

                DontDestroyOnLoad(gameObject);
                SubscribeToEvents();

                return;
            }

            Destroy(gameObject);
        }

        private void SubscribeToEvents()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void CleanupEventSubscriptions()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void InitializeSaveSystemConnection()
        {
            if (_save != null && _save.HasSave())
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

            _killedEnemies = saveData.DefeatedEnemies != null ? new HashSet<string>(saveData.DefeatedEnemies)
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
            if (_save == null || _save.HasSave() == false)
            {
                return false;
            }

            string savedSceneName = _save.CurrentSave?.Scene;

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
}
