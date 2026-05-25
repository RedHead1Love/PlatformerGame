using Player;
using UnityEngine;
using UnityEngine.UI;

public sealed class ArmorManager : MonoBehaviour, IArmorManager
{
    private const string ArmorPanelName = "ArmorPanel_Runtime";
    private const int MinimumArmorValue = 0;

    [Header("Armor Settings")]
    [SerializeField] private int _maxArmor = 6;
    [SerializeField] private int _currentArmor = 6;
    [SerializeField] private bool _startEnabled = false;

    [Header("UI References")]
    [SerializeField] private GameObject _armorPanelPrefab;

    [Header("Sound Settings")]
    [SerializeField] private bool _useAudioController = true;

    private GameObject _armorPanelInstance;
    private Image[] _armorIcons;
    private AudioController _audioController;
    private Hero _hero;

    private bool _isArmorUnlocked = false;

    public int CurrentArmor => _currentArmor;
    public int MaxArmor => _maxArmor;

    public bool HasArmor => _currentArmor > 0 && IsArmorUnlocked();

    public event System.Action<int, int> OnArmorChanged;

    private void Start()
    {
        InitializeAudioController();
        FindHero();

        _isArmorUnlocked = _startEnabled || (_hero != null && _hero.AbilityManager != null && _hero.AbilityManager.HasArmor);

        if (IsArmorUnlocked())
        {
            InitializeArmorUI();
            UpdateArmorUI();
        }
        else
        {
            HideArmorUI();
        }
    }

    private void InitializeAudioController()
    {
        if (_useAudioController && _audioController == null)
        {
            _audioController = GetComponent<AudioController>();

            if (_audioController == null)
            {
                _audioController = FindFirstObjectByType<AudioController>();
            }
        }
    }

    private void FindHero()
    {
        _hero = GetComponent<Hero>();

        if (_hero == null)
        {
            _hero = FindFirstObjectByType<Hero>();
        }
    }

    public bool IsArmorUnlocked()
    {
        return _isArmorUnlocked;
    }

    public void SetArmor(int amount)
    {
        if (!IsArmorUnlocked())
        {
            return;
        }

        _currentArmor = Mathf.Clamp(amount, MinimumArmorValue, _maxArmor);

        UpdateArmorUI();
        OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
    }

    public int TakeArmorDamage(int damageAmount)
    {
        if (!IsArmorUnlocked() || _currentArmor <= 0)
        {
            return damageAmount;
        }

        int oldArmor = _currentArmor;
        int remainingDamage = 0;

        if (damageAmount >= _currentArmor)
        {
            remainingDamage = damageAmount - _currentArmor;
            SetArmor(MinimumArmorValue);
        }
        else
        {
            SetArmor(_currentArmor - damageAmount);
        }

        PlayArmorDamageSound(oldArmor, _currentArmor);

        return remainingDamage;
    }

    private void PlayArmorDamageSound(int oldArmor, int newArmor)
    {
        if (!_useAudioController || _audioController == null)
        {
            return;
        }
    }

    public void AddArmor(int amount)
    {
        if (!IsArmorUnlocked())
        {
            return;
        }

        SetArmor(_currentArmor + amount);
    }

    public void FillArmor()
    {
        if (IsArmorUnlocked())
        {
            SetArmor(_maxArmor);
        }
    }

    public void ResetArmor()
    {
        if (IsArmorUnlocked())
        {
            SetArmor(MinimumArmorValue);
        }
    }

    public void UnlockArmor()
    {
        _isArmorUnlocked = true;

        ShowArmorUI();
        SetArmor(_maxArmor);
    }

    public void UnlockArmorAbility()
    {
        UnlockArmor();
    }

    public void LoadArmorFromSave(int armorFromSave)
    {
        if (!IsArmorUnlocked())
        {
            return;
        }

        int validArmor = Mathf.Clamp(armorFromSave, MinimumArmorValue, _maxArmor);

        if (validArmor != _currentArmor)
        {
            SetArmor(validArmor);
        }
    }

    public bool CanPurchaseArmorPlates()
    {
        return IsArmorUnlocked() && CurrentArmor < MaxArmor;
    }

    public void UpdateArmorUI()
    {
    }

    private void InitializeArmorUI() { }
    private void HideArmorUI() { }
    private void ShowArmorUI() { }
}
