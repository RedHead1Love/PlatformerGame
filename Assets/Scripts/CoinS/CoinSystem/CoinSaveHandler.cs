using GameLogic;
using System.Collections;
using UnityEngine;

public sealed class CoinSaveHandler : MonoBehaviour
{
    private const float InitializationDelay = 0.3f;

    private void Start()
    {
        StartCoroutine(InitializeCoins());
    }

    private IEnumerator InitializeCoins()
    {
        yield return new WaitForSeconds(InitializationDelay);

        if (SaveSystem.Instance == null || SaveSystem.Instance.HasSave() == false)
        {
            yield break;
        }

        GameSaveData saveData = SaveSystem.Instance.CurrentSave;

        if (saveData == null || saveData.coins.isInitialized == false)
        {
            yield break;
        }

        InitializeFromSave(saveData.coins);
    }

    private void InitializeFromSave(CoinData savedCoins)
    {
        PersistentWallet.Instance?.LoadFromSaveData(savedCoins);
        WalletManager.Instance?.LoadFromSaveData(savedCoins);
    }
}