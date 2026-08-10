using UnityEngine;
using TMPro;

public sealed class LevelStatsUI : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject _statsPanel;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _deathsText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("Input Settings")]
    [SerializeField] private KeyCode _toggleKey = KeyCode.Tab;

    private void Start()
    {
        if (_statsPanel != null)
        {
            _statsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            ToggleStatsPanel();
        }

        if (_statsPanel != null && _statsPanel.activeInHierarchy)
        {
            UpdateStatsText();
        }
    }

    private void ToggleStatsPanel()
    {
        if (_statsPanel == null)
        {
            return;
        }

        bool isActive = !_statsPanel.activeInHierarchy;
        _statsPanel.SetActive(isActive);

        if (isActive)
        {
            UpdateStatsText();
        }
    }

    private void UpdateStatsText()
    {
        if (LevelStatsTracker.Instance == null)
        {
            return;
        }

        float time = LevelStatsTracker.Instance.TimeInSeconds;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        if (_timeText != null)
        {
            _timeText.text = $" {minutes:00}:{seconds:00}";
        }

        if (_damageText != null)
        {
            _damageText.text = $" {LevelStatsTracker.Instance.TotalDamageTaken}";
        }

        if (_deathsText != null)
        {
            _deathsText.text = $" {LevelStatsTracker.Instance.TotalDeaths}";
        }

        if (_scoreText != null)
        {
            _scoreText.text = $" {LevelStatsTracker.Instance.TotalScore}";
        }
    }
}