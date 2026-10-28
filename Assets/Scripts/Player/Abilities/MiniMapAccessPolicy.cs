using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MiniMapAccessPolicy
{
    private const string MapUnlockedKey = "MapUnlocked";
    private const string MainMenuSceneName = "MainMenu";
    private ShopManager _shop;
    private Hero _hero;
    public bool IsLocked { get; private set; }

    public MiniMapAccessPolicy(bool initiallyLocked)
    {
        IsLocked = initiallyLocked;
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(MapUnlockedKey))
        {
            IsLocked = PlayerPrefs.GetInt(MapUnlockedKey, 0) == 0;
        }
        else if (HasMapAbility())
        {
            IsLocked = false;
        }
    }

    public void RefreshUnlock()
    {
        if (IsLocked && HasMapAbility())
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        if (!IsLocked)
        {
            return;
        }
        IsLocked = false;
        PlayerPrefs.SetInt(MapUnlockedKey, 1);
        PlayerPrefs.Save();
    }

    public bool MustClose(bool cutscenePlaying, bool hasPlayer)
    {
        if (cutscenePlaying || !hasPlayer ||
            SceneManager.GetActiveScene().name == MainMenuSceneName ||
            PauseMenuController.IsGamePaused || Mathf.Approximately(Time.timeScale, 0f))
        {
            return true;
        }
        if (_shop == null)
        {
            _shop = Object.FindFirstObjectByType<ShopManager>(FindObjectsInactive.Include);
        }
        return _shop != null && _shop.IsShopOpen;
    }

    public void ResetScene()
    {
        _hero = null;
        _shop = null;
    }

    private bool HasMapAbility()
    {
        if (_hero == null)
        {
            _hero = Object.FindFirstObjectByType<Hero>();
        }
        return _hero != null && _hero.AbilityManager != null && _hero.AbilityManager.HasMap;
    }
}
