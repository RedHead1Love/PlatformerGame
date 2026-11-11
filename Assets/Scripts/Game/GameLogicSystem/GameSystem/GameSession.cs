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

        public static GameSession Instance { get; private set; }

        public void StartNewGame()
        {
            WorldProgress.ResetGameState();

            _shouldStartNewGame = false;

            PlayerPrefs.DeleteKey("LastChance_Active");
            PlayerPrefs.DeleteKey("ArmorPlates_Used");

            _save?.DeleteSaveData();

            ResetKeyCollectionUI();
        }

        public void SetNewGameFlag()
        {
            _shouldStartNewGame = true;
        }

        private IGameProgressSave _save;

        private void BindSave(IGameProgressSave save)
        {
            _save = save;
        }

        private void Awake()
        {
            InitializeSingleton();
        }

        private void OnDestroy()
        {
            GameSaveConnections.Disconnect(BindSave);
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
                GameSaveConnections.Connect(BindSave);

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
