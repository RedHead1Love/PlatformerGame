using Player.Input;
using UnityEngine;

namespace ShopLogic
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Merchant : MonoBehaviour, IMerchant
    {
        private const KeyCode InteractKey = KeyCode.E;
        private const string PlayerTag = "Player";

        [Header("UI & Interaction")]
        [SerializeField] private GameObject _interactionHint;
        [SerializeField] private ShopUIManager _shopUIManager;

        private bool _isPlayerInRange;
        private OldInputProvider _inputProvider;

        private void Start()
        {
            InitializeShopManager();
            HideInteractionHint();
        }

        private void Update()
        {
            if (_isPlayerInRange && Input.GetKeyDown(InteractKey))
            {
                if (_shopUIManager != null && !_shopUIManager.IsShopOpen)
                {
                    OpenShop();
                }
            }
        }

        private void InitializeShopManager()
        {
            if (_shopUIManager == null)
            {
                _shopUIManager = FindFirstObjectByType<ShopUIManager>();
            }
        }

        private void HideInteractionHint()
        {
            if (_interactionHint != null)
            {
                _interactionHint.SetActive(false);
            }
        }

        private void ShowInteractionHint()
        {
            if (_interactionHint != null)
            {
                _interactionHint.SetActive(true);
            }
        }

        public void OpenShop()
        {
            _shopUIManager?.OpenShop();

            if (_inputProvider != null)
            {
                _inputProvider.SetShopMode(true);
            }
        }

        public void CloseShop()
        {
            _shopUIManager?.CloseShop();

            if (_inputProvider != null)
            {
                _inputProvider.SetShopMode(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(PlayerTag))
            {
                _isPlayerInRange = true;

                ShowInteractionHint();

                _inputProvider = other.GetComponent<OldInputProvider>();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(PlayerTag))
            {
                _isPlayerInRange = false;

                HideInteractionHint();

                if (_inputProvider != null)
                {
                    _inputProvider.SetShopMode(false);
                    _inputProvider = null;
                }
            }
        }
    }
}
