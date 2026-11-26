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
        private const string InvalidIdentifier = "-1";
        private const string DefaultIdFormat = "{0}_{1:F3}_{2:F3}";
        private readonly Dictionary<string, Entity> _enemies = new Dictionary<string, Entity>();
        private HashSet<string> _killedEnemies = new HashSet<string>();
        private IGameProgressSave _save;

        public static event System.Action OnEnemyKilledGlobal;

        public static EnemyRegistry Instance { get; private set; }

        public string RegisterEnemy(Entity enemy, string enemyId)
        {
            if (enemy == null)
            {
                return null;
            }

            string validId = CreateValidEnemyId(enemy, enemyId);
            if (_enemies.TryGetValue(validId, out Entity registeredEnemy) && registeredEnemy != enemy)
            {
                Debug.LogError($"Duplicate enemy ID '{validId}' on '{enemy.name}'. Give each enemy a unique ID.", enemy);
                return null;
            }

            _enemies[validId] = enemy;
            if (_killedEnemies.Contains(validId))
            {
                enemy.gameObject.SetActive(false);
            }

            return validId;
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
            Entity[] allEnemies = FindObjectsByType<Entity>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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

            return _enemies.ContainsKey(enemyId) && IsEnemyValid(_enemies[enemyId]) && _enemies[enemyId].gameObject.activeInHierarchy;
        }

        public void ResetForSceneRestart()
        {
            _enemies.Clear();
            if (_save != null && _save.HasSave())
            {
                GameSaveData saveData = _save.CurrentSave;
                _killedEnemies = saveData?.DefeatedEnemies != null
                    ? new HashSet<string>(saveData.DefeatedEnemies)
                    : new HashSet<string>();
            }
            else
            {
                _killedEnemies.Clear();
            }
        }

        public void SetSave(IGameProgressSave save)
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
            GameSaveConnections.Disconnect(SetSave);
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
                GameSaveConnections.Connect(SetSave);
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

            _killedEnemies = saveData.DefeatedEnemies != null
                ? new HashSet<string>(saveData.DefeatedEnemies)
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

            EnemyRegistration registration = enemy.GetComponent<EnemyRegistration>();
            if (registration != null)
            {
                string registeredId = registration.GetEnemyId();
                if (IsInvalidEnemyId(registeredId) == false)
                {
                    return registeredId;
                }
            }

            string cleanObjectName = enemy.gameObject.name.Replace("(Clone)", string.Empty).Trim();
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                 DefaultIdFormat,
                 cleanObjectName,
                 enemy.transform.position.x,
                 enemy.transform.position.y);
        }

        private bool IsInvalidEnemyId(string enemyId)
        {
            return string.IsNullOrWhiteSpace(enemyId) ||
                   string.Equals(enemyId, InvalidIdentifier, System.StringComparison.Ordinal);
        }

        private bool IsEnemyValid(Entity enemy)
        {
            return enemy != null && enemy.gameObject != null;
        }
    }
}
