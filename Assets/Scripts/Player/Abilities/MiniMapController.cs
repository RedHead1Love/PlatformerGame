using System.Collections.Generic;
using Player.Input;
using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public sealed class MiniMapController : MonoBehaviour
{
    [System.Serializable]
    public sealed class MiniMapData
    {
        private const float DefaultMapWidth = 50f;
        private const float DefaultMapHeight = 50f;

        public string LocationName;
        public Sprite MapTexture;
        public Vector2 MapWorldSize = new Vector2(DefaultMapWidth, DefaultMapHeight);
        public Vector2 MapWorldCenter = Vector2.zero;
    }

    private const KeyCode DefaultToggleKey = KeyCode.M;
    private const string MapUnlockedKey = "MapUnlocked";

    [Header("MiniMap Settings")]
    [SerializeField] private GameObject _miniMapPanel;
    [SerializeField] private KeyCode _toggleKey = DefaultToggleKey;

    [Header("Input")]
    [SerializeField] private IInputProvider _inputProvider;

    [Header("Lock State")]
    [SerializeField] private bool _isMapLocked = true;
    [SerializeField] private GameObject _lockOverlay;

    [Header("Player Marker")]
    [SerializeField] private RectTransform _playerMarker;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector2 _mapUISize = new Vector2(400f, 250f);

    [Header("MiniMap Textures")]
    [SerializeField] private List<MiniMapData> _miniMapDataList;
    [SerializeField] private Image _miniMapImage;

    private bool _isMiniMapVisible;
    private RectTransform _miniMapRectTransform;
    private int _currentMapIndex = 0;

    private const string MapUnlockedKey = "MapUnlocked";

    [System.Serializable]
    public class MiniMapData
    {
        private const float DefaultMapWidth = 50f;
        private const float DefaultMapHeight = 50f;

        public string locationName;
        public Sprite mapTexture;
        public Vector2 mapWorldSize = new Vector2(DefaultMapWidth, DefaultMapHeight);
        public Vector2 mapWorldCenter = Vector2.zero;
    }

    private void Awake()
    {
        InitializeMiniMap();
        FindInputProvider();
    }
    private int _currentMapIndex;

    private void Start()
    {
        _miniMapRectTransform = _miniMapImage?.GetComponent<RectTransform>();

        LoadMapState();
        InitializePlayerTransform();

        if (_miniMapPanel != null)
        {
            _miniMapPanel.SetActive(false);
        }

        _isMiniMapVisible = false;

        UpdateLockOverlayVisibility();
    }

    private void FindInputProvider()
    {
        if (_inputProvider == null)
        {
            if (YG2.envir.isDesktop)
            {
                _inputProvider = FindObjectOfType<OldInputProvider>();
            }
            else if (YG2.envir.isMobile)
            {
                _inputProvider = FindObjectOfType<JoystickInput>();
            }
        }

        if (_inputProvider == null)
            Debug.LogWarning("IInputProvider not found! Map toggle won't work with key.");
    }

    private void InitializePlayerMarker()
    private void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            ToggleMiniMap();
        }

        if (_isMiniMapVisible && !_isMapLocked)
        {
            UpdatePlayerMarker();
        }
    }

    private void InitializePlayerTransform()
    {
        if (_playerTransform == null)
        {
            _playerTransform = FindFirstObjectByType<Hero>()?.transform;
        }
    }

    public void ToggleMiniMap()
    {
        if (_inputProvider != null && _inputProvider.IsOpenMapPressed)
        _isMiniMapVisible = !_isMiniMapVisible;

        if (_miniMapPanel != null)
        {
            _miniMapPanel.SetActive(_isMiniMapVisible);
        }

        UpdateLockOverlayVisibility();

        if (_isMiniMapVisible && !_isMapLocked)
        {
            UpdatePlayerMarker();
        }
    }

    private void UpdateLockOverlayVisibility()
    {
        if (_lockOverlay != null)
        {
            _lockOverlay.SetActive(_isMiniMapVisible && _isMapLocked);
        }
    }

    private void UpdatePlayerMarker()
    {
        if (_playerTransform == null || _miniMapRectTransform == null || _playerMarker == null)
        {
            return;
        }

        if (_currentMapIndex < 0 || _currentMapIndex >= _miniMapDataList.Count)
        {
            return;
        }

        MiniMapData currentMap = _miniMapDataList[_currentMapIndex];

        Vector3 playerPos = _playerTransform.position;
        Vector2 normalizedPos = new Vector2(
            (playerPos.x - currentMap.MapWorldCenter.x) / currentMap.MapWorldSize.x,
            (playerPos.y - currentMap.MapWorldCenter.y) / currentMap.MapWorldSize.y
        );

        normalizedPos.x = Mathf.Clamp(normalizedPos.x, -0.5f, 0.5f);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, -0.5f, 0.5f);

        _playerMarker.anchoredPosition = new Vector2(
            normalizedPos.x * _mapUISize.x,
            normalizedPos.y * _mapUISize.y
        );

        _playerMarker.localRotation = Quaternion.Euler(0, 0, _playerTransform.rotation.eulerAngles.z);
    }

    public void UnlockMap()
    {
        _isMapLocked = false;

        PlayerPrefs.SetInt(MapUnlockedKey, 1);
        PlayerPrefs.Save();

        UpdateLockOverlayVisibility();
    }

    private void LoadMapState()
    {
        if (PlayerPrefs.HasKey(MapUnlockedKey))
        {
            _isMapLocked = PlayerPrefs.GetInt(MapUnlockedKey, 0) == 0;
        }
        else
        {
            var hero = FindFirstObjectByType<Hero>();
            if (hero != null && hero.AbilityManager != null && hero.AbilityManager.HasMap)
            {
                _isMapLocked = false;
            }
        }
    }

    public void SetMiniMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= _miniMapDataList.Count)
        {
            return;
        }

        MiniMapData newMap = _miniMapDataList[mapIndex];

        if (_miniMapImage == null || newMap.MapTexture == null)
        {
            return;
        }

        _currentMapIndex = mapIndex;
        _miniMapImage.sprite = newMap.MapTexture;

        if (_isMiniMapVisible)
        {
            UpdatePlayerMarker();
        }
    }

    public void SetMiniMap(string locationName)
    {
        for (int i = 0; i < _miniMapDataList.Count; i++)
        {
            if (_miniMapDataList[i].LocationName == locationName)
            {
                SetMiniMap(i);
                return;
            }
        }
    }
}
