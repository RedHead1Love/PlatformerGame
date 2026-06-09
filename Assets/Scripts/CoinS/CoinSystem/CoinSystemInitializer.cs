using System.Collections;
using GameLogic;
using UnityEngine;

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

            if (saveData != null && saveData.coins.isInitialized)
            {
                InitializeFromSave(saveData.coins);

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