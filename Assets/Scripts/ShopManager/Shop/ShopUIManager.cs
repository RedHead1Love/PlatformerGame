using UnityEngine;
using UnityEngine.UI;

namespace ShopLogic
{
    public sealed class ShopUIManager : MonoBehaviour
    {
        private const float PausedTimeScale = 0f;
        private const float NormalTimeScale = 1f;
        private const string OpenTrigger = "Open";
        private const string CloseTrigger = "Close";

        [Header("UI Panels")]
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private Button _closeButton;

        [Header("Animation")]
        [SerializeField] private Animator _shopAnimator;

        public bool IsShopOpen { get; private set; }

        private void Start()
        {
            _closeButton?.onClick.AddListener(CloseShop);

            if (_shopPanel != null)
            {
                _shopPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveListener(CloseShop);
        }

        public void OpenShop()
        {
            if (IsShopOpen || _shopPanel == null)
            {
                return;
            }

            IsShopOpen = true;
            _shopPanel.SetActive(true);

            if (_shopAnimator != null)
            {
                _shopAnimator.SetTrigger(OpenTrigger);
            }

            Time.timeScale = PausedTimeScale;
        }

        public void CloseShop()
        {
            if (!IsShopOpen || _shopPanel == null)
            {
                return;
            }

            IsShopOpen = false;

            if (_shopAnimator != null)
            {
                _shopAnimator.SetTrigger(CloseTrigger);
            }
            else
            {
                _shopPanel.SetActive(false);
            }

            Time.timeScale = NormalTimeScale;
        }
    }
}
