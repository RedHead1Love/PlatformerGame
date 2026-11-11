namespace GameLogic
{
    internal interface IGameSnapshotCapture
    {
        void Capture(GameSaveData save);
    }
}
