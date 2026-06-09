using Player.Input;
using UnityEngine;
using YG;

namespace NPC
{
    public sealed class Merchant : MonoBehaviour, IMerchant
    {
        private const int StateIdle = 0;
        private const int StateIdle2 = 1;
        private const int StateTalk = 2;
        private const float DefaultInteractionRadius = 2f;
        private const string PlayerTag = "Player";

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

        private readonly int _stateHash = Animator.StringToHash("state");

        private Transform _playerTransform;
        private bool _hasInteracted;
        private bool _isPlayerInRange;
        private bool _isShopOpen;

        public bool IsShopOpen => _isShopOpen;

        private void Start()
        {
            InitializeReferences();
            FindInputProvider();
            FindPlayer();
        }

        private void Update()
        {
            FindPlayerIfNeeded();
            UpdatePlayerRange();
            HandlePlayerInput();
        }

        public void OpenShop()
        {
            _hasInteracted = true;
            _isShopOpen = true;

            SetAnimation(StateTalk);

            if (_shopPanel != null)
            {
                _shopPanel.SetActive(true);
            }

            _shopManager?.OpenShop();
        }

        public void CloseShop()
        {
            _shopManager?.CloseShop();

            if (_shopPanel != null)
            {
                _shopPanel.SetActive(false);
            }

            _isShopOpen = false;

            if (_isPlayerInRange)
            {
                SetAnimation(StateIdle);
            }
        }

        public void CloseShopExternal()
        {
            CloseShop();
        }

        private void InitializeReferences()
        {
            if (_interactionPoint == null)
            {
                _interactionPoint = transform;
            }

            if (_interactionHint != null)
            {
                _interactionHint.SetActive(false);
            }

            if (_shopPanel != null)
            {
                _shopPanel.SetActive(false);
            }

            if (_shopManager == null && _shopPanel != null)
            {
                _shopManager = _shopPanel.GetComponent<ShopManager>();
            }

            SetAnimation(StateIdle2);
        }

        private void FindInputProvider()
        {
            if (_inputProvider != null)
            {
                return;
            }

            _inputProvider = FindFirstObjectByType<AggregatedInputProvider>();

            if (_inputProvider == null && YG2.envir.isDesktop)
            {
                _inputProvider = FindFirstObjectByType<OldInputProvider>();
            }

            if (_inputProvider == null && YG2.envir.isMobile)
            {
                _inputProvider = FindFirstObjectByType<JoystickInput>();
            }

            if (_inputProvider == null)
            {
                Debug.LogWarning("IInputProvider не найден");
            }
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);

            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        private void FindPlayerIfNeeded()
        {
            if (_playerTransform == null)
            {
                FindPlayer();
            }
        }

        private void UpdatePlayerRange()
        {
            if (_playerTransform == null)
            {
                if (_isPlayerInRange)
                {
                    OnPlayerExitRange();
                }

                return;
            }

            float distance = Vector2.Distance(_interactionPoint.position, _playerTransform.position);
            bool playerInRange = distance <= _interactionRadius;

            if (playerInRange && _isPlayerInRange == false)
            {
                OnPlayerEnterRange();
            }
            else if (playerInRange == false && _isPlayerInRange)
            {
                OnPlayerExitRange();
            }
        }

        private void HandlePlayerInput()
        {
            if (_inputProvider == null)
            {
                return;
            }

            if (_isPlayerInRange && _inputProvider.IsOpenShopOrChestPressed)
            {
                ToggleShop();
            }

            if (_isShopOpen && _inputProvider.IsMenuPressed)
            {
                CloseShop();
            }
        }

        private void ToggleShop()
        {
            if (_isShopOpen)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }

        private void OnPlayerEnterRange()
        {
            _isPlayerInRange = true;

            if (_interactionHint != null)
            {
                _interactionHint.SetActive(true);
            }
        }

        private void OnPlayerExitRange()
        {
            _isPlayerInRange = false;

            if (_interactionHint != null)
            {
                _interactionHint.SetActive(false);
            }

            if (_closeShopOnExit && _isShopOpen)
            {
                CloseShop();
            }

            if (_hasInteracted && _isShopOpen == false)
            {
                SetAnimation(StateIdle);
            }
        }

        private void SetAnimation(int state)
        {
            if (_animator != null)
            {
                _animator.SetInteger(_stateHash, state);
            }
        }
    }
}