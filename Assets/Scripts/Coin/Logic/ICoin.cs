namespace GameLogic
{
    public interface ICoin
    {
        CoinType CoinType { get; }

        int CoinValue { get; }

        bool IsCollectable { get; }

        void Collect();

        void EnableCollection();
    }
}
