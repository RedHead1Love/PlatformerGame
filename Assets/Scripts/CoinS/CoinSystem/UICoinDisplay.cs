using GameLogic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public sealed class UICoinDisplay : MonoBehaviour
{
    [System.Serializable]
    public sealed class CoinDisplay
    {
        [SerializeField] private WalletManager.CoinType _coinType;
        [SerializeField] private Image _coinIcon;
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private string _displayFormat = "{0}";

        public WalletManager.CoinType CoinType => _coinType;
        public Image CoinIcon => _coinIcon;
        public TMP_Text CoinText => _coinText;
        public string DisplayFormat => _displayFormat;
    }

    [SerializeField] private CoinDisplay[] _coinDisplays;

    private void Start()
    {
        if (WalletManager.Instance == null)
        {
            return;
        }

        WalletManager.Instance.OnCoinsChanged += OnCoinsChanged;

        InitializeDisplay();
    }

    private void OnDestroy()
    {
        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.OnCoinsChanged -= OnCoinsChanged;
        }
    }

    private void InitializeDisplay()
    {
        if (_coinDisplays == null || WalletManager.Instance == null)
        {
            return;
        }

        foreach (CoinDisplay display in _coinDisplays)
        {
            if (display == null)
            {
                continue;
            }

            int currentCoins = WalletManager.Instance.GetCoins(display.CoinType);

            UpdateCoinDisplay(display, currentCoins);
        }
    }

    private void OnCoinsChanged(WalletManager.CoinType coinType, int newAmount)
    {
        if (_coinDisplays == null)
        {
            return;
        }

        foreach (CoinDisplay display in _coinDisplays)
        {
            if (display == null || display.CoinType != coinType)
            {
                continue;
            }

            UpdateCoinDisplay(display, newAmount);

            break;
        }
    }

    private void UpdateCoinDisplay(CoinDisplay display, int amount)
    {
        if (display.CoinText == null)
        {
            return;
        }

        display.CoinText.text = FormatCoinAmount(display.DisplayFormat, amount);

    }

    private string FormatCoinAmount(string displayFormat, int amount)
    {
        if (string.IsNullOrEmpty(displayFormat))
        {
            return amount.ToString();
        }

        try
        {
            return string.Format(displayFormat, amount);
        }
        catch (System.FormatException)
        {
            return amount.ToString();
        }
    }
}
