using System;
using Player;
using Player.Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Foxberry.Shop
{
    public sealed class ShopItemView : MonoBehaviour, IPointerClickHandler
    {
        private readonly Color _overlayColor = new Color(0f, 1f, 0f, 0.2f);

        [SerializeField]
        private Image _iconImage;
        private Color _originalIconColor;
        private Vector3 _originalScale;
        private IShopItem _itemData;

        public event Action<IShopItem> OnPointerSelected;

        public IShopItem ItemData => _itemData;

        public RectTransform RectTransform { get; private set; }

        public Sprite ItemIcon => _iconImage != null ? _iconImage.sprite : null;

        public void Initialize(IShopItem itemData)
        {
            _itemData = itemData;
            UpdateView();
        }

        public void UpdateView()
        {
            if (_itemData == null)
            {
                return;
            }
        }

        public void SetSelected(bool isSelected, bool isAvailable)
        {
            UpdateScaleSelection(isSelected);
        }

        public void ResetVisuals()
        {
            if (_iconImage != null)
            {
                _iconImage.color = _originalIconColor;
            }

            if (RectTransform != null)
            {
                RectTransform.localScale = _originalScale;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_itemData != null)
                {
                    OnPointerSelected?.Invoke(_itemData);
                }
            }
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            CacheOriginalVisuals();
        }

        private void CacheOriginalVisuals()
        {
            if (_iconImage != null)
            {
                _originalIconColor = _iconImage.color;
            }

            _originalScale = RectTransform != null ? RectTransform.localScale : Vector3.one;
        }

        private void UpdateScaleSelection(bool isSelected)
        {
            if (RectTransform != null)
            {
                RectTransform.localScale = isSelected ? _originalScale * 1.05f : _originalScale;
            }
        }

        private bool IsLastChanceActive()
        {
            AbilityManager abilityManager = FindAbilityManager();
            return abilityManager != null && abilityManager.IsLastChanceActive;
        }

        private AbilityManager FindAbilityManager()
        {
            Hero hero = FindFirstObjectByType<Hero>();
            return hero != null ? hero.AbilityManager : null;
        }
    }
}
