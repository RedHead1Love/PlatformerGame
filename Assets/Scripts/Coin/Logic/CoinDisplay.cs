using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foxberry.Shop
{
    [System.Serializable]
    public sealed class CoinDisplay
    {
        [SerializeField]
        private CoinType _coinType;
        [SerializeField]
        private Image _coinIcon;
        [SerializeField]
        private TMP_Text _coinText;
        [SerializeField]
        private string _displayFormat = "{0}";

        public CoinType CoinType => _coinType;

        public Image CoinIcon => _coinIcon;

        public TMP_Text CoinText => _coinText;

        public string DisplayFormat => _displayFormat;
    }
}
