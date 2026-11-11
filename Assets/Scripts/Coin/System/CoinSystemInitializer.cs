using System.Collections;
using GameLogic;
using UnityEngine;

namespace Foxberry
{
    public sealed class CoinSystemInitializer : MonoBehaviour
    {
        private const float InitializationDelay = 0.3f;

        private void Start()
        {
            StartCoroutine(InitializeCoins());
        }

        private IEnumerator InitializeCoins()
        {
            yield return new WaitForSeconds(InitializationDelay);

            if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
            {
                GameSaveData saveData = SaveSystem.Instance.CurrentSave;

                if (saveData != null && saveData.WalletBalance.IsInitialized)
                {
                    InitializeFromSave(saveData.WalletBalance);

                    yield break;
                }
            }

            InitializeFromPlayerPrefs();
        }

        private void InitializeFromSave(CoinData savedCoins)
        {
            PersistentWallet.Instance?.LoadCoinsFromSave(savedCoins);
            WalletManager.Instance?.LoadFromSaveData(savedCoins);
        }

        private void InitializeFromPlayerPrefs()
        {
            PersistentWallet.Instance?.LoadCoins();

            if (PersistentWallet.Instance != null && WalletManager.Instance != null)
            {
                WalletManager.Instance.LoadFromSaveData(PersistentWallet.Instance.CurrentCoins);
            }
        }
    }
}
