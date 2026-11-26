using UnityEngine;
using UnityEngine.UI;

namespace Foxberry
{
    public sealed class MiniMapView
    {
        private readonly GameObject _panel;
        private readonly GameObject _lockOverlay;
        private readonly Image _image;
        private bool _isVisible;

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
                    _lockOverlay = overlay != null
                        ? overlay.gameObject
                        : null;
                }
            }

            SetVisible(false);
        }

        public bool IsVisible => _isVisible && CanShow;

        public bool CanShow => _panel != null;

        public void SetVisible(bool visible)
        {
            _isVisible = visible && CanShow;
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
}
