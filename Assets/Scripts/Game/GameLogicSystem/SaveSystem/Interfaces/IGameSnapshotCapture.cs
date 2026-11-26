namespace GameLogic
{
    public interface IGameSnapshotCapture
    {
        void Capture(GameSaveData save);
    }
}
