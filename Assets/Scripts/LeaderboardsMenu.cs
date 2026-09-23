using UnityEngine;
using YG;
using YG.Utils.LB;

public sealed class LeaderboardsMenu : MonoBehaviour
{
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private LeaderboardEntryUI _entryPrefab;

    private string _currentBoardId;

    private void OnEnable() => YG2.onGetLeaderboard += OnDataReceived;
    private void OnDisable() => YG2.onGetLeaderboard -= OnDataReceived;

    public void LoadBoard(string boardId)
    {
        _currentBoardId = boardId;
        ClearBoard();

        YG2.GetLeaderboard(boardId);
    }

    private void OnDataReceived(LBData lbData)
    {
        if (lbData.technoName != _currentBoardId) return;

        ClearBoard();
        foreach (var player in lbData.players)
        {
            var entry = Instantiate(_entryPrefab, _contentContainer);

            entry.Setup(player.rank, player.name, player.score, _currentBoardId);
        }
    }

    private void ClearBoard()
    {
        foreach (Transform child in _contentContainer)
        {
            Destroy(child.gameObject);
        }
    }
}