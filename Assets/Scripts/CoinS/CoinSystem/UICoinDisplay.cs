using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameLogic;

public sealed class UICoinDisplay : MonoBehaviour
{
    [System.Serializable]
    public sealed class CoinDisplay
    {
        public WalletManager.CoinType CoinType;
        public Image CoinIcon;
        public TMP_Text CoinText;
        public string DisplayFormat = "{0}";
    }

    private const int HighThreshold = 100;
    private const int MediumThreshold = 50;

    [Header("UI Elements")]
    [SerializeField] private CoinDisplay[] _coinDisplays;

    private void Start()
    {
        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.OnCoinsChanged += HandleCoinsChanged;
            InitializeDisplay();
        }
    }

    private void OnDestroy()
    {
        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.OnCoinsChanged -= HandleCoinsChanged;
        }
    }

    private void InitializeDisplay()
    {
        foreach (var display in _coinDisplays)
        {
            int currentCoins = WalletManager.Instance.GetCoins(display.CoinType);
            UpdateCoinDisplay(display, currentCoins);
        }
    }

    private void HandleCoinsChanged(WalletManager.CoinType coinType, int newAmount)
    {
        foreach (var display in _coinDisplays)
        {
            if (display.CoinType == coinType)
            {
                UpdateCoinDisplay(display, newAmount);
                break;
            }
        }
    }

    private void UpdateCoinDisplay(CoinDisplay display, int amount)
    {
        if (display.CoinText != null)
        {
            if (!string.IsNullOrEmpty(display.DisplayFormat))
            {
                try
                {
                    display.CoinText.text = string.Format(display.DisplayFormat, amount);
                }
                catch
                {
                    display.CoinText.text = amount.ToString();
                }
            }
            else
            {
                display.CoinText.text = amount.ToString();
            }
        }

        UpdateDisplayColor(display, amount);
    }

    private void UpdateDisplayColor(CoinDisplay display, int amount)
    {
        if (display.CoinText == null)
        {
            return;
        }

        if (amount >= HighThreshold)
        {
            display.CoinText.color = Color.yellow;
        }
        else if (amount >= MediumThreshold)
        {
            display.CoinText.color = Color.white;
        }
        else
        {
            display.CoinText.color = Color.gray;
        }
    }
}
