using System;
using Player.Abilities;

namespace GameLogic
{
    public interface IGameProgressSave
    {
        event Action<GameSaveData> OnGameLoaded;
        GameSaveData CurrentSave { get; }
        bool HasSave();
        void DeleteSaveData();
        void MarkEnemyKilled(string id);
        void UpdateAbilityData(AbilityManager abilities);
    }
}
