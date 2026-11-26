using DoorControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public sealed class GameSession : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        [SerializeField]
        private bool _shouldStartNewGame = false;
        private IGameProgressSave _save;
        private IWorldProgress _worldProgress;
        private IProgressPreferences _preferences;

        public static GameSession Instance { get; private set; }

        public void StartNewGame()
        {
            _worldProgress.ResetGameState();
            _shouldStartNewGame = false;
            _preferences.DeleteKey("LastChance_Active");
            _preferences.DeleteKey("ArmorPlates_Used");
            _preferences.Save();
            _save?.DeleteSaveData();
            ResetKeyCollectionUI();
        }

        public void SetNewGameFlag()
        {
            _shouldStartNewGame = true;
        }

        public void SetSave(IGameProgressSave save)
        {
            _save = save;
        }

        public void SetWorldProgress(IWorldProgress worldProgress)
        {
            _worldProgress = worldProgress ?? throw new System.ArgumentNullException(nameof(worldProgress));
        }

        public void SetPreferences(IProgressPreferences preferences)
        {
            _preferences = preferences ?? throw new System.ArgumentNullException(nameof(preferences));
        }

        private void Awake()
        {
            _worldProgress ??= GameServices.Progress;
            _preferences ??= GameServices.Preferences;
            InitializeSingleton();
        }

        private void OnDestroy()
        {
            GameSaveConnections.Disconnect(SetSave);
            if (Instance == this)
            {
                Instance = null;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                GameSaveConnections.Connect(SetSave);
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }

            Destroy(gameObject);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == MainMenuSceneName || _shouldStartNewGame)
            {
                StartNewGame();
            }
        }

        private void ResetKeyCollectionUI()
        {
            KeyCollection keyCollection = FindFirstObjectByType<KeyCollection>();
            keyCollection?.ResetAllKeys();
        }
    }
}
