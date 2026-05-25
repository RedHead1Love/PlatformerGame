using TMPro;
using UnityEngine;

public sealed class BossDoor : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const float MessageDisplayDuration = 2f;
    private const float MessageDisplayDistance = 3f;

    [Header("Boss Settings")]
    [SerializeField] private string[] _bossIds = { "226", "227", "228", "229" };

    [Header("UI Prefabs")]
    [SerializeField] private GameObject _interactionMessagePrefab;
    [SerializeField] private GameObject _temporaryMessagePrefab;

    private Transform _playerTransform;
    private AudioController _audioController;

    private GameObject _currentMessage;
    private TextMeshProUGUI _messageText;

    private void Start()
    {
        InitializeReferences();
        CreateUIPrefabsIfNeeded();
    }

    private void Update()
    {
        if (_playerTransform == null)
        {
            return;
        }

        UpdatePlayerProximity();
        HandlePlayerInteraction();
    }

    private void OnDestroy()
    {
        CleanupUI();
    }

    private void InitializeReferences()
    {
        _playerTransform = GameObject.FindGameObjectWithTag(PlayerTag)?.transform;
        _audioController = FindFirstObjectByType<AudioController>();
    }

    private void CreateUIPrefabsIfNeeded()
    {
        if (_interactionMessagePrefab == null)
        {
            Debug.LogWarning("Interaction Message Prefab is missing on BossDoor.");
        }
    }

    private void UpdatePlayerProximity()
    {
        // Логика проверки расстояния до игрока
    }

    private void HandlePlayerInteraction()
    {
        // Логика взаимодействия и проверки ключей/боссов
    }

    private void ShowInteractionMessage(string message)
    {
        HideInteractionMessage();

        if (_interactionMessagePrefab != null)
        {
            _currentMessage = Instantiate(_interactionMessagePrefab);
            _currentMessage.SetActive(true);

            _messageText = _currentMessage.GetComponentInChildren<TextMeshProUGUI>();

            if (_messageText != null)
            {
                _messageText.text = message;
            }
        }
    }

    private void HideInteractionMessage()
    {
        if (_currentMessage != null)
        {
            Destroy(_currentMessage);

            _currentMessage = null;
            _messageText = null;
        }
    }

    private void ShowTemporaryMessage(string message, float duration)
    {
        if (_temporaryMessagePrefab != null)
        {
            GameObject tempMessage = Instantiate(_temporaryMessagePrefab);
            tempMessage.SetActive(true);

            TextMeshProUGUI textComponent = tempMessage.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = message;
            }

            Destroy(tempMessage, duration);
        }
    }

    private void CleanupUI()
    {
        HideInteractionMessage();
    }
}
