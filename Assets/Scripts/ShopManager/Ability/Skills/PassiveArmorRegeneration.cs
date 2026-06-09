using Player.Abilities;
using UnityEngine;

public sealed class PassiveArmorRegeneration : MonoBehaviour, IPassiveArmorRegeneration
{
    private const float DefaultRegenInterval = 10f;
    private const int DefaultRegenAmount = 1;
    private const float AbilityCheckInterval = 1f;

    [SerializeField] private float _regenInterval = DefaultRegenInterval;
    [SerializeField] private int _regenAmount = DefaultRegenAmount;

    private Hero _hero;
    private IArmorManager _armorManager;
    private AbilityManager _abilityManager;
    private float _timer;
    private float _checkTimer;
    private bool _isActive;

    public float RegenInterval => _regenInterval;
    public int RegenAmount => _regenAmount;
    public bool IsActive => _isActive;

    private void Awake()
    {
        InitializeReferences();
    }

    private void Update()
    {
        if (_isActive == false)
        {
            CheckAbilityByInterval();

            return;
        }

        if (CanRegenerate() == false)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer < _regenInterval)
        {
            return;
        }

        _timer = 0f;

        _armorManager.AddArmor(_regenAmount);
    }

    public void Activate()
    {
        _isActive = true;
        _timer = 0f;
    }

    public void Deactivate()
    {
        _isActive = false;
        _timer = 0f;
    }

    private void InitializeReferences()
    {
        _hero = GetComponent<Hero>();

        if (_hero == null)
        {
            _hero = FindFirstObjectByType<Hero>();
        }

        _armorManager = GetComponent<IArmorManager>();

        if (_armorManager == null)
        {
            _armorManager = FindFirstObjectByType<ArmorManager>();
        }

        _abilityManager = _hero?.AbilityManager;
    }

    private void CheckAbilityByInterval()
    {
        _checkTimer += Time.deltaTime;

        if (_checkTimer < AbilityCheckInterval)
        {
            return;
        }

        _checkTimer = 0f;

        RefreshAbilityManagerReference();

        if (_abilityManager != null && _abilityManager.HasRobocopRegeneration)
        {
            Activate();
        }
    }

    private void RefreshAbilityManagerReference()
    {
        if (_abilityManager == null)
        {
            _abilityManager = _hero?.AbilityManager;
        }
    }

    private bool CanRegenerate()
    {
        return _armorManager != null &&
               _hero != null &&
               _hero.IsAlive() &&
               _armorManager.IsArmorUnlocked() &&
               _armorManager.CurrentArmor < _armorManager.MaxArmor;
    }
}