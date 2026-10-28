using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BossDoorMessageView : IDisposable
{
    private readonly MonoBehaviour _host;
    private readonly Func<bool> _isBlocked;
    private GameObject _interactionMessagePrefab;
    private GameObject _temporaryMessagePrefab;
    private readonly int _messageSortingOrder;
    private GameObject _currentInteractionMessage;
    private Canvas _interactionCanvas;
    private TextMeshProUGUI _interactionMessageText;
    private GameObject _currentTemporaryMessage;
    private Canvas _temporaryCanvas;
    private Coroutine _temporaryMessageCoroutine;
    private bool _ownsInteractionPrefab;
    private bool _ownsTemporaryPrefab;

    public BossDoorMessageView(MonoBehaviour host, GameObject interactionPrefab,
        GameObject temporaryPrefab, int sortingOrder, Func<bool> isBlocked)
    {
        _host = host;
        _interactionMessagePrefab = interactionPrefab;
        _temporaryMessagePrefab = temporaryPrefab;
        _messageSortingOrder = sortingOrder;
        _isBlocked = isBlocked;
    }

    public void Initialize()
    {
        if (_interactionMessagePrefab == null)
        {
            _interactionMessagePrefab = CreateDefaultUIPrefab("InteractionMessage");
            _ownsInteractionPrefab = true;
        }

        if (_temporaryMessagePrefab == null)
        {
            _temporaryMessagePrefab = CreateDefaultUIPrefab("TemporaryMessage");
            _ownsTemporaryPrefab = true;
        }
    }

    private GameObject CreateDefaultUIPrefab(string objectName)
    {
        const string textObjectName = "MessageText";
        const string defaultMessage = "Message";
        const int fontSize = 24;
        const float width = 600f;
        const float height = 80f;
        const float anchorPosition = 0.5f;

        GameObject uiObject = new GameObject(objectName);

        Canvas canvas = uiObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = _messageSortingOrder;

        uiObject.AddComponent<CanvasScaler>();

        GameObject textObject = new GameObject(textObjectName);
        textObject.transform.SetParent(uiObject.transform, false);

        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();

        textComponent.text = defaultMessage;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.raycastTarget = false;

        RectTransform rectTransform = textComponent.GetComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(anchorPosition, anchorPosition);
        rectTransform.anchorMax = new Vector2(anchorPosition, anchorPosition);
        rectTransform.pivot = new Vector2(anchorPosition, anchorPosition);
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = Vector2.zero;

        uiObject.SetActive(false);

        return uiObject;
    }

    public void RefreshVisibility()
    {
        bool shouldHide = _isBlocked();

        if (_interactionCanvas != null)
        {
            _interactionCanvas.enabled = !shouldHide;
        }

        if (_temporaryCanvas != null)
        {
            _temporaryCanvas.enabled = !shouldHide;
        }
    }

    public void ShowInteractionMessage(string message)
    {
        HideInteractionMessage();

        if (_interactionMessagePrefab == null) return;

        _currentInteractionMessage = UnityEngine.Object.Instantiate(_interactionMessagePrefab);
        _currentInteractionMessage.SetActive(true);

        _interactionCanvas = _currentInteractionMessage.GetComponent<Canvas>();
        if (_interactionCanvas != null)
        {
            _interactionCanvas.sortingOrder = _messageSortingOrder;
            _interactionCanvas.enabled = !_isBlocked();
        }

        _interactionMessageText = _currentInteractionMessage.GetComponentInChildren<TextMeshProUGUI>();

        if (_interactionMessageText != null)
        {
            _interactionMessageText.text = message;
            _interactionMessageText.raycastTarget = false;
        }
    }

    public void HideInteractionMessage()
    {
        if (_currentInteractionMessage == null) return;

        UnityEngine.Object.Destroy(_currentInteractionMessage);

        _currentInteractionMessage = null;
        _interactionCanvas = null;
        _interactionMessageText = null;
    }

    public void ShowTemporaryMessage(string message, float duration)
    {
        if (_temporaryMessagePrefab == null) return;

        if (_temporaryMessageCoroutine != null) _host.StopCoroutine(_temporaryMessageCoroutine);
        if (_currentTemporaryMessage != null) UnityEngine.Object.Destroy(_currentTemporaryMessage);

        _currentTemporaryMessage = UnityEngine.Object.Instantiate(_temporaryMessagePrefab);
        _currentTemporaryMessage.SetActive(true);

        _temporaryCanvas = _currentTemporaryMessage.GetComponent<Canvas>();
        if (_temporaryCanvas != null)
        {
            _temporaryCanvas.sortingOrder = _messageSortingOrder;
            _temporaryCanvas.enabled = !_isBlocked();
        }

        TextMeshProUGUI textComponent = _currentTemporaryMessage.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.raycastTarget = false;
        }

        _temporaryMessageCoroutine = _host.StartCoroutine(TemporaryMessageLifetimeRoutine(duration));
    }

    private IEnumerator TemporaryMessageLifetimeRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (_isBlocked() == false)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        if (_currentTemporaryMessage != null)
        {
            UnityEngine.Object.Destroy(_currentTemporaryMessage);
            _currentTemporaryMessage = null;
            _temporaryCanvas = null;
        }

        _temporaryMessageCoroutine = null;
    }

    public void Dispose()
    {
        if (_temporaryMessageCoroutine != null)
        {
            _host.StopCoroutine(_temporaryMessageCoroutine);
            _temporaryMessageCoroutine = null;
        }
        HideInteractionMessage();

        if (_currentTemporaryMessage != null)
        {
            UnityEngine.Object.Destroy(_currentTemporaryMessage);
        }
        if (_ownsInteractionPrefab && _interactionMessagePrefab != null)
        {
            UnityEngine.Object.Destroy(_interactionMessagePrefab);
            _interactionMessagePrefab = null;
        }
        if (_ownsTemporaryPrefab && _temporaryMessagePrefab != null)
        {
            UnityEngine.Object.Destroy(_temporaryMessagePrefab);
            _temporaryMessagePrefab = null;
        }

    }
}
