using Player.Input;
using UnityEngine;
using YG;

namespace ShopLogic
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Merchant : MonoBehaviour, IMerchant
    {
        private const int StateIdle = 0;
        private const int StateIdle2 = 1;
        private const int StateTalk = 2;
        private const float DefaultInteractionRadius = 2f;

        [Header("Input")]
        [SerializeField] private IInputProvider _inputProvider;

        [Header("Animations")]
        [SerializeField] private Animator _animator;

        [Header("Shop Settings")]
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private ShopManager _shopManager;
        [SerializeField] private bool _closeShopOnExit = true;

        [Header("Interaction Points")]
        [SerializeField] private float _interactionRadius = DefaultInteractionRadius;
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private GameObject _interactionHint;
        private const KeyCode InteractKey = KeyCode.E;
        private const string PlayerTag = "Player";

        [Header("UI & Interaction")]
        [SerializeField] private GameObject _interactionHint;
        [SerializeField] private ShopUIManager _shopUIManager;

        private bool _isPlayerInRange;
        private OldInputProvider _inputProvider;

        private void Start()
        {
            InitializeReferences();
            FindInputProvider();
        }

        private void InitializeReferences()
        {
            _interactionPoint ??= transform;

            if (_interactionHint != null)
            {
                _interactionHint.SetActive(false);
            }

            if (_shopPanel != null)
            {
                _shopPanel.SetActive(false);
            }

            _shopManager ??= _shopPanel?.GetComponent<ShopManager>();

            SetAnimation(StateIdle2);
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

        private void FindInputProvider()
        {
            if (_inputProvider == null)
            {
                if (YG2.envir.isDesktop)
                {
                    _inputProvider = FindObjectOfType<OldInputProvider>();
                }
                else if (YG2.envir.isMobile)
                {
                    _inputProvider = FindObjectOfType<JoystickInput>();
                }
            }
            
            if (_inputProvider == null)
                Debug.LogWarning("IInputProvider not found! Map toggle won't work with key.");
        }

        private void HandlePlayerInput()
        {
            if (_isPlayerInRange && _inputProvider.IsOpenShopOrChestPressed)
            {
                if (!_isShopOpen)
                {
                    OpenShop();
                }
                else
                {
                    CloseShop();
                }
            }

            if (_isShopOpen && _inputProvider.IsMenuPressed)
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
