using GameLogic;
using Player.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopItemView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private GameObject _purchasedOverlay;

    private readonly Color _soldSelectedColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
    private readonly Color _soldDefaultColor = new Color(0f, 0.6f, 0f, 0.3f);
    private readonly Color _overlayColor = new Color(0f, 1f, 0f, 0.2f);

    private Color _originalIconColor;
    private Color _originalNameColor;
    private Color _originalPriceColor;
    private Color _originalBackgroundColor;
    private Vector3 _originalScale;

    private IShopItem _itemData;

    public IShopItem ItemData => _itemData;
    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();

        CacheOriginalVisuals();
        EnsurePurchasedOverlayExists();
    }

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

        UpdateName();
        UpdatePrice();
        UpdatePurchasedOverlay();
    }

    public void SetSelected(bool isSelected, bool isAvailable)
    {
        UpdateBackgroundSelection(isSelected, isAvailable);
        UpdateScaleSelection(isSelected);
    }

    public void ResetVisuals()
    {
        if (_iconImage != null)
        {
            _iconImage.color = _originalIconColor;
        }

        if (_nameText != null)
        {
            _nameText.color = _originalNameColor;
        }

        if (_priceText != null)
        {
            _priceText.color = _originalPriceColor;
        }

        if (_backgroundImage != null)
        {
            _backgroundImage.color = _originalBackgroundColor;
        }

        if (RectTransform != null)
        {
            RectTransform.localScale = _originalScale;
        }
    }

    private void CacheOriginalVisuals()
    {
        if (_iconImage != null)
        {
            _originalIconColor = _iconImage.color;
        }

        if (_nameText != null)
        {
            _originalNameColor = _nameText.color;
        }

        if (_priceText != null)
        {
            _originalPriceColor = _priceText.color;
        }

        if (_backgroundImage != null)
        {
            _originalBackgroundColor = _backgroundImage.color;
        }

        _originalScale = RectTransform != null ? RectTransform.localScale : Vector3.one;
    }

    private void EnsurePurchasedOverlayExists()
    {
        if (_purchasedOverlay != null)
        {
            return;
        }

        _purchasedOverlay = new GameObject("PurchasedOverlay");
        _purchasedOverlay.transform.SetParent(transform, false);

        Image overlayImage = _purchasedOverlay.AddComponent<Image>();
        overlayImage.color = _overlayColor;

        RectTransform rectTransform = _purchasedOverlay.GetComponent<RectTransform>();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        _purchasedOverlay.SetActive(false);
    }

    private void UpdateName()
    {
        if (_nameText != null)
        {
            _nameText.text = string.IsNullOrEmpty(_itemData.DisplayName)
                ? "нет имени"
                : _itemData.DisplayName;
        }
    }

    private void UpdatePrice()
    {
        if (_priceText == null)
        {
            return;
        }

        if (_itemData.ItemId == ShopItemIds.ActivateLastChance && IsLastChanceActive())
        {
            _priceText.text = "<color=yellow>Активно</color>";

            return;
        }

        if (_itemData.IsSold && _itemData.ItemId != ShopItemIds.RestoreArmor)
        {
            _priceText.text = "<color=green>Куплено</color>";

            return;
        }

        _priceText.text = $"{_itemData.Price} {GetCurrencySymbol(_itemData.CurrencyType)}";
    }

    private void UpdatePurchasedOverlay()
    {
        if (_purchasedOverlay == null)
        {
            return;
        }

        bool shouldShowOverlay = _itemData.ItemId == ShopItemIds.ActivateLastChance
            ? IsLastChanceActive()
            : _itemData.ItemId != ShopItemIds.RestoreArmor && _itemData.IsSold;

        _purchasedOverlay.SetActive(shouldShowOverlay);
    }

    private void UpdateBackgroundSelection(bool isSelected, bool isAvailable)
    {
        if (_backgroundImage == null)
        {
            return;
        }

        if (_itemData != null && _itemData.IsSold && _itemData.ItemId != ShopItemIds.RestoreArmor)
        {
            _backgroundImage.color = isSelected ? _soldSelectedColor : _soldDefaultColor;

            return;
        }

        _backgroundImage.color = isSelected
            ? isAvailable ? Color.green : Color.red
            : _originalBackgroundColor;
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

    private string GetCurrencySymbol(WalletManager.CoinType coinType)
    {
        return coinType switch
        {
            WalletManager.CoinType.Gold => "G",
            WalletManager.CoinType.Silver => "S",
            WalletManager.CoinType.Bronze => "B",
            _ => string.Empty
        };
    }
}