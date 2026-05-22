using GameLogic;
using System.Collections;
using UnityEngine;

public sealed class CoinSaveHandler : MonoBehaviour
{
    private const float InitializationDelaySeconds = 0.3f;

    private WaitForSeconds _initializationDelay;

    private void Awake()
    {
        _initializationDelay = new WaitForSeconds(InitializationDelaySeconds);
    }

    private void Start()
    {
        StartCoroutine(InitializeCoinsCoroutine());
    }

    private IEnumerator InitializeCoinsCoroutine()
    {
        yield return _initializationDelay;

        if (SaveSystem.Instance == null || !SaveSystem.Instance.HasSave())
        {
            yield break;
        }

        var saveData = SaveSystem.Instance.CurrentSave;

        if (saveData == null || !saveData.Coins.IsInitialized)
        {
            yield break;
        }

        InitializeFromSave(saveData.Coins);
    }

    private void InitializeFromSave(CoinData savedCoins)
    {
        if (PersistentWallet.Instance != null)
        {
            PersistentWallet.Instance.LoadFromSaveData(savedCoins);
        }
        else if (WalletManager.Instance != null)
        {
            WalletManager.Instance.LoadFromSaveData(savedCoins);
        }
    }
}
