using NPC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MiniMapController : MonoBehaviour
{
    private const KeyCode DefaultToggleKey = KeyCode.M;
    public static bool IsCutscenePlaying { get; set; }

    [SerializeField] private GameObject _miniMapPanel;
    [SerializeField] private KeyCode _toggleKey = DefaultToggleKey;

    [SerializeField] private bool _isMapLocked = true;
    [SerializeField] private GameObject _lockOverlay;

    [SerializeField] private RectTransform _playerMarker;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector2 _mapUISize = new Vector2(400f, 250f);

    [SerializeField] private List<Merchant> _shops;
    [SerializeField] private List<RectTransform> _keyMarkers;
    [SerializeField] private List<RectTransform> _keyCollectedMarkers;
    [SerializeField] private List<Transform> _keyAnchors;

    [SerializeField] private List<Checkpoint> _checkpoints;
    [SerializeField] private List<RectTransform> _checkpointMarkers;
    [SerializeField] private GameObject _checkpointMarkerPrefab;

    [SerializeField] private List<MiniMapData> _miniMapDataList;
    [SerializeField] private Image _miniMapImage;

    private MiniMapView _view;
    private MiniMapLocationSelector _locations;
    private MiniMapMarkerController _markers;
    private MiniMapInputHandler _input;
    private MiniMapAccessPolicy _access;

    private void Awake()
    {
        IsCutscenePlaying = false;
        EnsureInitialized();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        if (_view != null)
        {
            CloseMiniMap();
        }
    }

    private void Start()
    {
        EnsureInitialized();
        _markers.CreateCheckpointMarkers();
        _markers.Hide();
        _view.SetLocked(_access.IsLocked);
    }

    private void Update()
    {
        if (_access.MustClose(IsCutscenePlaying, _markers.HasPlayer))
        {
            CloseMiniMap();
            _view.SetLocked(false);
            return;
        }
        _access.RefreshUnlock();
        _view.SetLocked(_access.IsLocked);
        if (_access.IsLocked)
        {
            CloseMiniMap();
            return;
        }
        if (_input.IsTogglePressed())
        {
            ToggleMiniMap();
        }
        if (_view.IsVisible)
        {
            _markers.Refresh();
        }
    }

    public void ToggleMiniMap()
    {
        EnsureInitialized();
        if (_access.IsLocked || !_view.CanShow ||
            _access.MustClose(IsCutscenePlaying, _markers.HasPlayer))
        {
            return;
        }
        if (_view.IsVisible)
        {
            CloseMiniMap();
            return;
        }
        _view.SetVisible(true);
        _markers.Refresh();
    }

    public void CloseMiniMap()
    {
        EnsureInitialized();
        _view.SetVisible(false);
        _markers.Hide();
    }

    public void UnlockMap()
    {
        EnsureInitialized();
        _access.Unlock();
        _view.SetLocked(false);
    }

    public void SetMiniMap(int mapIndex)
    {
        EnsureInitialized();
        if (_locations.TrySelect(mapIndex))
        {
            RefreshMap();
        }
    }

    public void SetMiniMap(string locationName)
    {
        EnsureInitialized();
        if (_locations.TrySelect(locationName))
        {
            RefreshMap();
        }
    }

    private void RefreshMap()
    {
        _view.ShowMap(_locations.CurrentMap);
        if (_view.IsVisible)
        {
            _markers.Refresh();
        }
    }

    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        IsCutscenePlaying = false;
        CloseMiniMap();
        _markers.ResetPlayer();
        _input.Reset();
        _access.ResetScene();
    }

    private void EnsureInitialized()
    {
        if (_view != null)
        {
            return;
        }
        _locations = new MiniMapLocationSelector(_miniMapDataList);
        _input = new MiniMapInputHandler(_toggleKey);
        _access = new MiniMapAccessPolicy(_isMapLocked);
        _access.Load();
        _markers = new MiniMapMarkerController(_miniMapPanel, _playerMarker, _playerTransform,
            _shops, _keyMarkers, _keyCollectedMarkers, _keyAnchors, _checkpoints,
            _checkpointMarkers, _checkpointMarkerPrefab, new MiniMapProjection(_locations, _mapUISize));
        _view = new MiniMapView(_miniMapPanel, _lockOverlay, _miniMapImage);
        _markers.Hide();
    }
    private void OnDrawGizmosSelected()
    {
        if (_miniMapDataList == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        foreach (var map in _miniMapDataList)
        {
            if (map != null)
            {
                Gizmos.DrawWireCube(map.MapWorldCenter, map.MapWorldSize);
            }
        }
    }
}
