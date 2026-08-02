using UnityEngine;
using UnityEngine.UI;

public sealed class LowHealthEffect : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image _overlayImage;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float _healthThreshold = 0.3f;
    [SerializeField] private float _pulseSpeed = 5f;
    [SerializeField] private float _maxAlpha = 0.4f;

    private bool _isLowHealth;

    private void Start()
    {
        if (_overlayImage == null)
        {
            _overlayImage = GetComponent<Image>();
        }

        DisableEffect();
    }

    private void Update()
    {
        float sineShift = 1f;
        float sineRange = 2f;

        if (_isLowHealth && _overlayImage != null)
        {
            float alpha = (Mathf.Sin(Time.time * _pulseSpeed) + sineShift) / sineRange * _maxAlpha;

            SetAlpha(alpha);
        }
    }

    public void UpdateHealthState(int currentHealth, int maxHealth)
    {
        float healthPercent = (float)currentHealth / maxHealth;

        _isLowHealth = healthPercent <= _healthThreshold;

        if (_isLowHealth == false)
        {
            DisableEffect();
        }
    }

    private void DisableEffect()
    {
        float transparentAlpha = 0f;

        if (_overlayImage != null)
        {
            SetAlpha(transparentAlpha);
        }
    }

    private void SetAlpha(float alphaValue)
    {
        Color color = _overlayImage.color;
        color.a = alphaValue;
        _overlayImage.color = color;
    }
}