using System;
using Player.Abilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace GameLogic
{
    public sealed class SaveSystem : MonoBehaviour, IGameProgressSave
    {
        private IGameSaveStore _store;
        private IGameSnapshotCapture _snapshot;

        public event Action<GameSaveData> OnGameLoaded;

        public static SaveSystem Instance { get; private set; }

        public GameSaveData CurrentSave { get; private set; }

        public void SaveGame(string checkpointId, Vector3 playerPosition)
        {
            InitializeSaveDataIfNeeded();

            CurrentSave.Id = Guid.NewGuid().ToString();
            CurrentSave.Timestamp = DateTime.Now;
            CurrentSave.Checkpoint = checkpointId;
            CurrentSave.Position = playerPosition;
            CurrentSave.Scene = SceneManager.GetActiveScene().name;

            _snapshot.Capture(CurrentSave);

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

            if (CurrentSave.DefeatedEnemies.Contains(enemyId))
            {
                return;
            }

            CurrentSave.DefeatedEnemies.Add(enemyId);
        }

        public void MarkHealthPickupCollected(string pickupId)
        {
            if (string.IsNullOrEmpty(pickupId))
            {
                return;
            }

            InitializeSaveDataIfNeeded();

            if (CurrentSave.HealthPickups.Contains(pickupId))
            {
                return;
            }

            CurrentSave.HealthPickups.Add(pickupId);
        }

        public void MarkItemPurchased(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            InitializeSaveDataIfNeeded();

            if (CurrentSave.Purchases.Contains(itemId))
            {
                return;
            }

            CurrentSave.Purchases.Add(itemId);

            SaveToFile();
        }

        public void UpdateAbilityData(AbilityManager abilityManager)
        {
            if (abilityManager == null)
            {
                return;
            }

            InitializeSaveDataIfNeeded();

            CurrentSave.Abilities.Capture(abilityManager);

            SaveToFile();
        }

        public void LoadGameData()
        {
            CurrentSave = _store.Load();
            CurrentSave?.EnsureValidCollections();

            OnGameLoaded?.Invoke(CurrentSave);
        }

        public void DeleteSaveData()
        {
            _store.Delete();

            CurrentSave = null;
        }

        public bool HasSave()
        {
            return _store.Exists();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                GameSaveConnections.SetActive(null);
            }
        }

        private void Awake()
        {
            _store = new JsonGameSaveStore(Application.persistentDataPath);
            _snapshot = new UnityGameSnapshotCapture();
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                GameSaveConnections.SetActive(this);

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

        private void SaveToFile()
        {
            if (CurrentSave == null)
            {
                return;
            }

            _store.Save(CurrentSave);
        }
    }
}
