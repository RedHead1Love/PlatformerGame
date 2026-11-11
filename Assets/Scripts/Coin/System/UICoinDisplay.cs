using Foxberry.Shop;
using GameLogic;
using UnityEngine;

namespace Foxberry
{
    public sealed class UICoinDisplay : MonoBehaviour
    {
        [SerializeField]
        private CoinDisplay[] _coinDisplays;

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

        private void OnCoinsChanged(CoinType coinType, int newAmount)
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
}
