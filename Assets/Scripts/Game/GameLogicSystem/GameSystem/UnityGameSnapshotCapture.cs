using System.Collections.Generic;
using DoorControl;
using Foxberry;
using GeneralLogicEnemies;
using Player;
using UnityEngine;

namespace GameLogic
{
    internal sealed class UnityGameSnapshotCapture : IGameSnapshotCapture
    {
        public void Capture(GameSaveData save)
        {
            UpdatePlayerData(save);
            UpdateKeyCollectionData(save);
            UpdateEnemyData(save);
            UpdateCoinData(save);
        }

        private void UpdatePlayerData(GameSaveData save)
        {
            Hero player = UnityEngine.Object.FindFirstObjectByType<Hero>();

            if (player == null)
            {
                return;
            }

            save.Health = player.Lives;

            ArmorManager armorManager = player.GetComponent<ArmorManager>();
            save.Armor = armorManager != null ? armorManager.CurrentArmor : 0;

            if (player.AbilityManager != null)
            {
                save.Abilities.Capture(player.AbilityManager);
            }
        }

        private void UpdateKeyCollectionData(GameSaveData save)
        {
            KeyCollection keyCollection = UnityEngine.Object.FindFirstObjectByType<KeyCollection>();

            if (keyCollection != null)
            {
                save.Keys = keyCollection.GetCollectedKeys();
            }
        }

        private void UpdateEnemyData(GameSaveData save)
        {
            if (EnemyRegistry.Instance == null)
            {
                save.DefeatedEnemies = new List<string>();

                return;
            }

            save.DefeatedEnemies = new List<string>(EnemyRegistry.Instance.GetKilledEnemies());
        }

        private void UpdateCoinData(GameSaveData save)
        {
            if (PersistentWallet.Instance != null)
            {
                save.WalletBalance = PersistentWallet.Instance.CurrentCoins;

                return;
            }

            if (WalletManager.Instance == null)
            {
                return;
            }

            save.WalletBalance = new CoinData(
                WalletManager.Instance.GetCoins(CoinType.Bronze),
                WalletManager.Instance.GetCoins(CoinType.Silver),
                WalletManager.Instance.GetCoins(CoinType.Gold));
        }
    }
}
