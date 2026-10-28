using DoorControl;
using NPC;
using System.Collections.Generic;
using UnityEngine;

public sealed class MiniMapMarkerController
{
    private readonly GameObject _miniMapPanel;
    private readonly RectTransform _playerMarker;
    private readonly List<Merchant> _shops;
    private readonly List<RectTransform> _keyMarkers;
    private readonly List<RectTransform> _keyCollectedMarkers;
    private readonly List<Transform> _keyAnchors;
    private readonly List<Checkpoint> _checkpoints;
    private List<RectTransform> _checkpointMarkers;
    private readonly GameObject _checkpointMarkerPrefab;
    private readonly MiniMapProjection _projection;
    private Transform _playerTransform;

    public MiniMapMarkerController(GameObject panel, RectTransform playerMarker, Transform player,
        List<Merchant> shops, List<RectTransform> keyMarkers, List<RectTransform> collectedMarkers,
        List<Transform> keyAnchors, List<Checkpoint> checkpoints,
        List<RectTransform> checkpointMarkers, GameObject checkpointPrefab, MiniMapProjection projection)
    {
        _miniMapPanel = panel;
        _playerMarker = playerMarker;
        _playerTransform = player;
        _shops = shops;
        _keyMarkers = keyMarkers;
        _keyCollectedMarkers = collectedMarkers;
        _keyAnchors = keyAnchors;
        _checkpoints = checkpoints;
        _checkpointMarkers = checkpointMarkers;
        _checkpointMarkerPrefab = checkpointPrefab;
        _projection = projection;
    }

    public bool HasPlayer
    {
        get
        {
            FindPlayerTransform();
            return _playerTransform != null;
        }
    }

    public void ResetPlayer()
    {
        _playerTransform = null;
    }

    public void Hide()
    {
        if (_playerMarker != null)
        {
            _playerMarker.gameObject.SetActive(false);
        }
        HideAllMapMarkers();
    }

    public void Refresh()
    {
        if (HasPlayer && _playerMarker != null)
        {
            _playerMarker.gameObject.SetActive(true);
            _playerMarker.anchoredPosition = _projection.ToUIPosition(_playerTransform.position);
            UpdatePlayerMarkerRotation();
        }
        SetShopsPositions();
        UpdateKeyMarkers();
        UpdateCheckpointMarkers();
    }

    private void FindPlayerTransform()
    {
        if (_playerTransform != null)
        {
            return;
        }

        Hero hero = Object.FindFirstObjectByType<Hero>();

        if (hero != null)
        {
            _playerTransform = hero.transform;
        }
    }

    private void HideAllMapMarkers()
    {
        if (_shops != null)
        {
            foreach (var merchant in _shops)
            {
                if (merchant != null)
                {
                    merchant.SetMarkerVisible(false);
                }
            }
        }

        if (_keyMarkers != null)
        {
            foreach (var marker in _keyMarkers)
            {
                if (marker != null)
                {
                    marker.gameObject.SetActive(false);
                }
            }
        }

        if (_keyCollectedMarkers != null)
        {
            foreach (var marker in _keyCollectedMarkers)
            {
                if (marker != null)
                {
                    marker.gameObject.SetActive(false);
                }
            }
        }

        if (_checkpointMarkers != null)
        {
            foreach (var marker in _checkpointMarkers)
            {
                if (marker != null)
                {
                    marker.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetShopsPositions()
    {
        if (_shops == null)
        {
            return;
        }

        foreach (var merchant in _shops)
        {
            if (merchant == null)
            {
                continue;
            }

            bool isInsideBounds = _projection.Contains(merchant.transform.position);

            merchant.SetMarkerVisible(isInsideBounds);

            if (isInsideBounds)
            {
                Vector2 uiPosition = _projection.ToUIPosition(merchant.transform.position);
                merchant.UpdateMarkerPosition(uiPosition);
            }
        }
    }

    private void UpdateCheckpointMarkers()
    {
        if (_checkpoints == null || _checkpointMarkers == null)
        {
            return;
        }

        int markerIndex = 0;

        for (int i = 0; i < _checkpoints.Count; i++)
        {
            if (_checkpoints[i] == null)
            {
                continue;
            }

            if (markerIndex >= _checkpointMarkers.Count)
            {
                break;
            }

            RectTransform marker = _checkpointMarkers[markerIndex];

            if (marker == null || marker.gameObject == null)
            {
                markerIndex++;
                continue;
            }

            bool isInsideBounds = _projection.Contains(_checkpoints[i].transform.position);

            marker.gameObject.SetActive(isInsideBounds);

            if (isInsideBounds)
            {
                Vector2 uiPosition = _projection.ToUIPosition(_checkpoints[i].transform.position);
                marker.anchoredPosition = uiPosition;
            }

            markerIndex++;
        }
    }

    public void CreateCheckpointMarkers()
    {
        if (_checkpoints == null || _checkpointMarkerPrefab == null || _miniMapPanel == null)
        {
            return;
        }

        if (_checkpointMarkers != null)
        {
            foreach (var marker in _checkpointMarkers)
            {
                if (marker != null)
                {
                    Object.Destroy(marker.gameObject);
                }
            }

            _checkpointMarkers.Clear();
        }
        else
        {
            _checkpointMarkers = new List<RectTransform>();
        }

        foreach (var checkpoint in _checkpoints)
        {
            if (checkpoint == null)
            {
                continue;
            }

            GameObject marker = Object.Instantiate(_checkpointMarkerPrefab, _miniMapPanel.transform, false);

            RectTransform rect = marker.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.localScale = Vector3.one;
                rect.gameObject.SetActive(false);
                _checkpointMarkers.Add(rect);
            }
        }
    }

    private void UpdateKeyMarkers()
    {
        if (_keyAnchors == null || _keyMarkers == null || _keyCollectedMarkers == null)
        {
            return;
        }

        if (_keyAnchors.Count != _keyMarkers.Count || _keyAnchors.Count != _keyCollectedMarkers.Count)
        {
            return;
        }

        KeyCollection keyCollection = Object.FindFirstObjectByType<KeyCollection>();

        if (keyCollection == null)
        {
            return;
        }

        for (int i = 0; i < _keyAnchors.Count; i++)
        {
            if (_keyAnchors[i] == null || _keyMarkers[i] == null || _keyCollectedMarkers[i] == null)
            {
                continue;
            }

            bool isInsideBounds = _projection.Contains(_keyAnchors[i].position);

            KeyColor color = (KeyColor)i;

            bool isCollected = keyCollection.HasKey(color);

            _keyMarkers[i].gameObject.SetActive(isInsideBounds && !isCollected);
            _keyCollectedMarkers[i].gameObject.SetActive(isInsideBounds && isCollected);

            if (isInsideBounds)
            {
                Vector2 uiPosition = _projection.ToUIPosition(_keyAnchors[i].position);

                if (isCollected == false)
                {
                    _keyMarkers[i].anchoredPosition = uiPosition;
                }

                if (isCollected)
                {
                    _keyCollectedMarkers[i].anchoredPosition = uiPosition;
                }
            }
        }
    }

    private void UpdatePlayerMarkerRotation()
    {
        if (_playerTransform == null || _playerMarker == null)
        {
            return;
        }

        float playerRotation = _playerTransform.eulerAngles.z;

        _playerMarker.localEulerAngles = new Vector3(0f, 0f, playerRotation);
    }
}
