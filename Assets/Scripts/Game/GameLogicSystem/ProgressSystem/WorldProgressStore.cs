using DoorControl;

namespace GameLogic
{
    internal sealed class WorldProgressStore : IWorldProgress
    {
        private const string ChestOpenedPrefix = "Chest_";
        private const string ChestOpenedSuffix = "_Opened";
        private const string KeySpawnedSuffix = "_KeySpawned";
        private const string KeyPrefix = "Key_";
        private const string GameSavedKey = "GameSaved";
        private const string ChestIdsKey = "ChestIds";
        private const char ChestIdsDelimiter = '\n';
        private const int TrueValue = 1;
        private const int FalseValue = 0;
        private const int MaxChestCount = 100;
        private readonly IProgressPreferences _preferences;

        public WorldProgressStore(IProgressPreferences preferences)
        {
            _preferences = preferences;
        }

        public bool IsChestOpened(string chestId)
        {
            TrackChestId(chestId);
            return _preferences.GetInt(GetChestOpenedKey(chestId), FalseValue) == TrueValue;
        }

        public void SetChestOpened(string chestId, bool isOpened)
        {
            TrackChestId(chestId);
            _preferences.SetInt(GetChestOpenedKey(chestId), isOpened
                ? TrueValue
                : FalseValue);
            _preferences.Save();
        }

        public bool IsKeySpawned(string chestId)
        {
            TrackChestId(chestId);
            return _preferences.GetInt(GetKeySpawnedKey(chestId), FalseValue) == TrueValue;
        }

        public void SetKeySpawned(string chestId, bool isSpawned)
        {
            TrackChestId(chestId);
            _preferences.SetInt(GetKeySpawnedKey(chestId), isSpawned
                ? TrueValue
                : FalseValue);
            _preferences.Save();
        }

        public void ResetGameState()
        {
            ClearAllKeyData();
            ClearAllChestData();
            _preferences.DeleteKey(GameSavedKey);
            _preferences.Save();
        }

        public void MarkGameSaved()
        {
            _preferences.SetInt(GameSavedKey, TrueValue);
            _preferences.Save();
        }

        public bool HasKey(KeyColor color)
        {
            return _preferences.GetInt(GetKeyColorKey(color), FalseValue) == TrueValue;
        }

        public void AddKey(KeyColor color)
        {
            _preferences.SetInt(GetKeyColorKey(color), TrueValue);
            _preferences.Save();
        }

        public void RemoveKey(KeyColor color)
        {
            _preferences.DeleteKey(GetKeyColorKey(color));
            _preferences.Save();
        }

        private void TrackChestId(string chestId)
        {
            if (string.IsNullOrWhiteSpace(chestId))
            {
                return;
            }

            string currentIds = _preferences.GetString(ChestIdsKey, string.Empty);
            string[] trackedIds = currentIds.Split(ChestIdsDelimiter);
            if (System.Array.IndexOf(trackedIds, chestId) >= 0)
            {
                return;
            }

            string separator = string.IsNullOrEmpty(currentIds) ? string.Empty : ChestIdsDelimiter.ToString();
            _preferences.SetString(ChestIdsKey, $"{currentIds}{separator}{chestId}");
            _preferences.Save();
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

        private void ClearAllKeyData()
        {
            foreach (KeyColor color in System.Enum.GetValues(typeof(KeyColor)))
            {
                _preferences.DeleteKey(GetKeyColorKey(color));
            }
        }

        private void ClearAllChestData()
        {
            string[] trackedChestIds = _preferences.GetString(ChestIdsKey, string.Empty).Split(ChestIdsDelimiter);
            foreach (string chestId in trackedChestIds)
            {
                if (string.IsNullOrEmpty(chestId))
                {
                    continue;
                }

                _preferences.DeleteKey(GetChestOpenedKey(chestId));
                _preferences.DeleteKey(GetKeySpawnedKey(chestId));
            }

            _preferences.DeleteKey(ChestIdsKey);

            for (int i = 0; i < MaxChestCount; i++)
            {
                _preferences.DeleteKey(GetChestOpenedKey(i.ToString()));
                _preferences.DeleteKey(GetKeySpawnedKey(i.ToString()));
            }

            _preferences.DeleteKey($"{ChestOpenedPrefix}{ChestOpenedSuffix}");
            _preferences.DeleteKey($"{ChestOpenedPrefix}{KeySpawnedSuffix}");
        }
    }
}
