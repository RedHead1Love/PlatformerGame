using UnityEngine;
using UnityEngine.UI;

public sealed class HealthBarUI : MonoBehaviour
{
    private const float DefaultSpacing = 35f;
    private const int MinimumHeartIndex = 0;

    [SerializeField] private Image _heartPrefab;
    [SerializeField] private Image _emptyHeartImage;
    [SerializeField] private int _maxHeartCount = 6;
    [SerializeField] private float _heartSpacing = DefaultSpacing;

    private Image[] _hearts;
    private bool _isInitialized;

    private void Start()
    {
        InitializeHearts();
    }

    public void SetHealth(int currentHealth)
    {
        InitializeHearts();

        if (_hearts == null)
        {
            return;
        }

        int validHealth = Mathf.Clamp(currentHealth, MinimumHeartIndex, _maxHeartCount);

        for (int heartIndex = MinimumHeartIndex; heartIndex < _maxHeartCount; heartIndex++)
        {
            Image heart = _hearts[heartIndex];

            if (heart == null)
            {
                continue;
            }

            bool isHeartFull = heartIndex < validHealth;

            heart.sprite = isHeartFull ? _heartPrefab.sprite : _emptyHeartImage.sprite;
            heart.color = Color.white;
        }
    }

    public void ForceRefresh()
    {
        ClearHearts();

        _isInitialized = false;

        InitializeHearts();
    }

    private void InitializeHearts()
    {
        if (_isInitialized)
        {
            return;
        }

        if (_heartPrefab == null || _emptyHeartImage == null)
        {
            return;
        }

        ClearHearts();

        _hearts = new Image[_maxHeartCount];

        for (int heartIndex = MinimumHeartIndex; heartIndex < _maxHeartCount; heartIndex++)
        {
            Image heart = Instantiate(_heartPrefab, transform);

            heart.rectTransform.anchoredPosition = new Vector2(heartIndex * _heartSpacing, 0f);
            heart.color = Color.white;

            _hearts[heartIndex] = heart;
        }

        _isInitialized = true;
    }

    private void ClearHearts()
    {
        if (_hearts == null)
        {
            return;
        }

        foreach (Image heart in _hearts)
        {
            if (heart != null)
            {
                Destroy(heart.gameObject);
            }
        }

        _hearts = null;
    }
}