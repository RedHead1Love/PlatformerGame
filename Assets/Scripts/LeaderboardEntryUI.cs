using TMPro;
using UnityEngine;

public sealed class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Setup(int rank, string playerName, int score, string boardId)
    {
        _rankText.text = rank.ToString();
        _nameText.text = string.IsNullOrEmpty(playerName) ? "Аноним" : playerName;

        if (boardId == "TimeBoard")
        {
            int minutes = Mathf.FloorToInt(score / 60f);
            int seconds = score % 60;
            _scoreText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            _scoreText.text = score.ToString();
        }
    }
}