using System.IO;
using UnityEngine;

namespace GameLogic
{
    internal sealed class JsonGameSaveStore : IGameSaveStore
    {
        private readonly string _directory;
        private readonly string _path;

        public JsonGameSaveStore(string root)
        {
            _directory = Path.Combine(root, "Saves");
            _path = Path.Combine(_directory, "game_save.json");
        }

        public bool Exists() => File.Exists(_path);

        public GameSaveData Load() => Exists() ? JsonUtility.FromJson<GameSaveData>(File.ReadAllText(_path)) : null;

        public void Save(GameSaveData data)
        {
            Directory.CreateDirectory(_directory);
            File.WriteAllText(_path, JsonUtility.ToJson(data, true));
        }

        public void Delete() => File.Delete(_path);
    }
}
