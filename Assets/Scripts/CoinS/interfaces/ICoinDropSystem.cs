using UnityEngine;

namespace GeneralLogicEnemies
{
    public interface ICoinDropSystem
    {
        void DropCoins();
        void InitializeDropSettings(GameObject coinPrefab, int minCoins, int maxCoins);
    }
}