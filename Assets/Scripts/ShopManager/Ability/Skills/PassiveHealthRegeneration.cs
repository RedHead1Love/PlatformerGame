using Player.Abilities;
using UnityEngine;

public sealed class PassiveHealthRegeneration : MonoBehaviour, IPassiveHealthRegeneration
{
    private const float DefaultRegenerationInterval = 5f;
    private const int DefaultHealAmount = 1;

    [SerializeField] private float _regenerationInterval = DefaultRegenerationInterval;
    [SerializeField] private int _healAmount = DefaultHealAmount;

    private Hero _hero;
    private HealthManager _healthManager;
    private AbilityManager _abilityManager;
    private float _timer;

    public float RegenerationInterval => _regenerationInterval;
    public int HealAmount => _healAmount;
    public bool IsActive => _abilityManager?.HasPassiveHealthRegeneration ?? false;

    private void Awake()
    {
        InitializeReferences();
    }

    private void Update()
    {
        RefreshAbilityManagerReference();

        if (CanRegenerate() == false)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer < _regenerationInterval)
        {
            return;
        }

        _timer = 0f;

        _healthManager.Heal(_healAmount);
    }

    public void EnableRegeneration()
    {
        enabled = true;
        _timer = 0f;
    }

    public void DisableRegeneration()
    {
        enabled = false;
        _timer = 0f;
    }

    private void InitializeReferences()
    {
        _hero = GetComponent<Hero>();

        if (_hero == null)
        {
            _hero = FindFirstObjectByType<Hero>();
        }

        _healthManager = GetComponent<HealthManager>();

        if (_healthManager == null)
        {
            _healthManager = FindFirstObjectByType<HealthManager>();
        }

        _abilityManager = _hero?.AbilityManager;
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
        return IsActive &&
               _hero != null &&
               _hero.IsAlive() &&
               _healthManager != null &&
               _healthManager.IsFullHealth == false;
    }
}