using UnityEngine;
using UnityEngine.UI;

public sealed class MiniMapView
{
    private readonly GameObject _panel;
    private readonly GameObject _lockOverlay;
    private readonly Image _image;
    public bool IsVisible { get; private set; }
    public bool CanShow => _panel != null;

    public MiniMapView(GameObject panel, GameObject lockOverlay, Image image)
    {
        _panel = panel;
        _lockOverlay = lockOverlay;
        _image = image;
        if (_panel != null)
        {
            if (_image == null)
            {
                _image = _panel.GetComponentInChildren<Image>(true);
            }
            if (_lockOverlay == null)
            {
                Transform overlay = _panel.transform.Find("LockOverlay");
                _lockOverlay = overlay != null ? overlay.gameObject : null;
            }
        }
        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        IsVisible = visible && CanShow;
        if (_panel != null)
        {
            _panel.SetActive(IsVisible);
        }
    }

    public void SetLocked(bool locked)
    {
        if (_lockOverlay != null)
        {
            _lockOverlay.SetActive(locked);
        }
    }

    public void ShowMap(MiniMapData map)
    {
        if (_image != null && map != null)
        {
            _image.sprite = map.MapTexture;
        }
    }
}
