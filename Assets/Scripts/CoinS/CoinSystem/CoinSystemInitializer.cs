using GameLogic;
using System.Collections;
using UnityEngine;

public sealed class CoinSystemInitializer : MonoBehaviour
{
    private WaitForSeconds _initializationDelay;

    private void Awake()
    {
        _initializationDelay = new WaitForSeconds(0.3f);
    }

    private void Start()
    {
        StartCoroutine(InitializeCoins());
    }

    private IEnumerator InitializeCoins()
    {
        yield return _initializationDelay;

        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            var saveData = SaveSystem.Instance.CurrentSave;

            if (saveData?.Coins != null)
            {
                InitializeFromSave(saveData.Coins);
                yield break;
            }
        }

        InitializeFromPlayerPrefs();
    }

    private void InitializeFromSave(CoinData savedCoins)
    {
        if (PersistentWallet.Instance != null)
        {
            PersistentWallet.Instance.LoadCoinsFromSave(savedCoins);
        }
    }

    private void InitializeFromPlayerPrefs()
    {
        if (PersistentWallet.Instance != null)
        {
            PersistentWallet.Instance.LoadCoins();
        }
    }
}
