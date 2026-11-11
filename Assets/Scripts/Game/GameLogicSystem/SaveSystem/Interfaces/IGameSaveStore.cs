namespace GameLogic
{
    internal interface IGameSaveStore
    {
        bool Exists();

        GameSaveData Load();

        void Save(GameSaveData data);

        void Delete();
    }
}
