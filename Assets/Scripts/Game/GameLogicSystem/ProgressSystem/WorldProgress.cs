using DoorControl;
using UnityEngine;

namespace GameLogic
{
    public sealed class WorldProgress : MonoBehaviour
    {
        private static readonly WorldProgressStore Store = new WorldProgressStore(new UnityProgressPreferences());

        public static WorldProgress Instance { get; private set; }

        public static bool IsChestOpened(string chestId) => Store.IsChestOpened(chestId);

        public static void SetChestOpened(string chestId, bool isOpened) => Store.SetChestOpened(chestId, isOpened);

        public static bool IsKeySpawned(string chestId) => Store.IsKeySpawned(chestId);

        public static void SetKeySpawned(string chestId, bool isSpawned) => Store.SetKeySpawned(chestId, isSpawned);

        public static void ResetGameState() => Store.ResetGameState();

        public static void MarkGameSaved() => Store.MarkGameSaved();

        public static bool HasKey(KeyColor color) => Store.HasKey(color);

        public static void AddKey(KeyColor color) => Store.AddKey(color);

        public static void RemoveKey(KeyColor color) => Store.RemoveKey(color);

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

                return;
            }

            Destroy(gameObject);
        }
    }
}
