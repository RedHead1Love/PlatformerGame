using GameLogic;
using System.Collections.Generic;
using UnityEngine;

public sealed class ShopNavigationController
{
    private const float DefaultMoveDelay = 0.2f;
    private const float InputThreshold = 0.1f;
    private const int NoItemSelectedIndex = -1;
    private const int FirstItemIndex = 0;

    private readonly float _moveDelay;
    private readonly Dictionary<WalletManager.CoinType, List<IShopItem>> _itemsByCurrency;

    private WalletManager.CoinType _currentCurrency;
    private int _currentItemIndex;
    private float _lastMoveTime;

    public WalletManager.CoinType CurrentCurrency => _currentCurrency;
    public int CurrentItemIndex => _currentItemIndex;

    public ShopNavigationController(float moveDelay = DefaultMoveDelay)
    {
        _moveDelay = moveDelay;
        _itemsByCurrency = new Dictionary<WalletManager.CoinType, List<IShopItem>>();

        foreach (WalletManager.CoinType currency in System.Enum.GetValues(typeof(WalletManager.CoinType)))
        {
            _itemsByCurrency[currency] = new List<IShopItem>();
        }

        Reset();
    }

    public void AddItems(WalletManager.CoinType currency, List<IShopItem> items)
    {
        if (items == null)
        {
            _itemsByCurrency[currency] = new List<IShopItem>();

            return;
        }

        _itemsByCurrency[currency] = items;
    }

    public bool TryMove(Vector2 inputDirection)
    {
        if (Time.time - _lastMoveTime < _moveDelay)
        {
            return false;
        }

        if (Mathf.Abs(inputDirection.x) <= InputThreshold &&
            Mathf.Abs(inputDirection.y) <= InputThreshold)
        {
            return false;
        }

        bool moved = HandleNavigation(inputDirection);

        if (moved)
        {
            _lastMoveTime = Time.time;
        }

        return moved;
    }

    public IShopItem GetCurrentItem()
    {
        if (_currentItemIndex < FirstItemIndex ||
            _currentItemIndex >= GetCurrentItemCount())
        {
            return null;
        }

        return _itemsByCurrency[_currentCurrency][_currentItemIndex];
    }

    public void Reset()
    {
        _currentCurrency = WalletManager.CoinType.Bronze;
        _currentItemIndex = NoItemSelectedIndex;
        _lastMoveTime = 0f;
    }

    private bool HandleNavigation(Vector2 inputDirection)
    {
        if (_currentItemIndex < FirstItemIndex)
        {
            return HandleCurrencyNavigation(inputDirection);
        }

        return HandleItemNavigation(inputDirection);
    }

    private bool HandleCurrencyNavigation(Vector2 inputDirection)
    {
        if (inputDirection.x < -InputThreshold)
        {
            return SwitchCurrency(-1);
        }

        if (inputDirection.x > InputThreshold)
        {
            return SwitchCurrency(1);
        }

        if (inputDirection.y < -InputThreshold && GetCurrentItemCount() > 0)
        {
            _currentItemIndex = FirstItemIndex;

            return true;
        }

        return false;
    }

    private bool HandleItemNavigation(Vector2 inputDirection)
    {
        if (inputDirection.y > InputThreshold)
        {
            _currentItemIndex = NoItemSelectedIndex;

            return true;
        }

        if (inputDirection.x < -InputThreshold && _currentItemIndex > FirstItemIndex)
        {
            _currentItemIndex--;

            return true;
        }

        if ((inputDirection.x > InputThreshold || inputDirection.y < -InputThreshold) &&
            _currentItemIndex < GetCurrentItemCount() - 1)
        {
            _currentItemIndex++;

            return true;
        }

        return false;
    }

    private bool SwitchCurrency(int direction)
    {
        int currencyCount = System.Enum.GetValues(typeof(WalletManager.CoinType)).Length;
        int newCurrencyIndex = (int)_currentCurrency + direction;

        if (newCurrencyIndex < 0 || newCurrencyIndex >= currencyCount)
        {
            return false;
        }

        _currentCurrency = (WalletManager.CoinType)newCurrencyIndex;
        _currentItemIndex = NoItemSelectedIndex;

        return true;
    }

    private int GetCurrentItemCount()
    {
        return _itemsByCurrency.TryGetValue(_currentCurrency, out List<IShopItem> items)
            ? items.Count
            : 0;
    }
}