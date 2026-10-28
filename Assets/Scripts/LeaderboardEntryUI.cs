using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _deathsText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void SetData(int rank, string playerName, int rawData)
    {
        int deaths = rawData % 100;                     
        int timeInSeconds = (rawData / 100) % 10000;     
        int score = rawData / 1000000;                   

        if (rawData < 100000)
        {
            score = rawData;
            timeInSeconds = 0;
            deaths = 0;
        }

        _rankText.text = "";
        _nameText.text = string.IsNullOrEmpty(playerName) ? "Аноним" : playerName;

        _timeText.text = $"{timeInSeconds / 60}:{(timeInSeconds % 60):00}";

        _deathsText.text = deaths.ToString();
        _scoreText.text = score.ToString();
    }
}
