using DoorControl;

namespace GameLogic
{
    public interface IWorldProgress
    {
        bool IsChestOpened(string chestId);

        void SetChestOpened(string chestId, bool isOpened);

        bool IsKeySpawned(string chestId);

        void SetKeySpawned(string chestId, bool isSpawned);

        void ResetGameState();

        void MarkGameSaved();

        bool HasKey(KeyColor color);

        void AddKey(KeyColor color);

        void RemoveKey(KeyColor color);
    }
}
