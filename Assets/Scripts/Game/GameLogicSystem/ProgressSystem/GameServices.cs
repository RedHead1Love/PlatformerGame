using UnityEngine;

namespace GameLogic
{
    internal static class GameServices
    {
        public static IGameSaveStore SaveStore { get; private set; }

        public static IGameSnapshotCapture Snapshot { get; private set; }

        public static IProgressPreferences Preferences { get; private set; }

        public static IWorldProgress Progress { get; private set; }

        public static void Configure(IGameSaveStore saveStore,
             IGameSnapshotCapture snapshot,
             IProgressPreferences preferences,
             IWorldProgress progress)
        {
            SaveStore = saveStore ?? throw new System.ArgumentNullException(nameof(saveStore));
            Snapshot = snapshot ?? throw new System.ArgumentNullException(nameof(snapshot));
            Preferences = preferences ?? throw new System.ArgumentNullException(nameof(preferences));
            Progress = progress ?? throw new System.ArgumentNullException(nameof(progress));
        }

        internal static void EnsureInitialized()
        {
            if (SaveStore != null && Snapshot != null && Preferences != null && Progress != null)
            {
                return;
            }

            var preferences = new UnityProgressPreferences();
            Configure(new JsonGameSaveStore(Application.persistentDataPath),
                 new UnityGameSnapshotCapture(),
                 preferences,
                 new WorldProgressStore(preferences));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            EnsureInitialized();
        }
    }
}
