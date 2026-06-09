using UnityEngine;

public sealed class HealthManager : MonoBehaviour
{
    private const int MinimumHealth = 0;

    [SerializeField] private int _maxHealth = 6;
    [SerializeField] private HealthBarUI _healthBarUI;
    [SerializeField] private ArmorManager _armorManager;
    [SerializeField] private AudioController _audioController;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => _maxHealth;
    public bool IsFullHealth => CurrentHealth >= _maxHealth;

    public event System.Action<int> OnHealthChanged;
    public event System.Action OnDeath;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        if (CurrentHealth <= MinimumHealth)
        {
            CurrentHealth = _maxHealth;
        }

        UpdateHealthUI();
    }

    public void SetHealth(int health)
    {
        int newHealth = Mathf.Clamp(health, MinimumHealth, _maxHealth);

        if (CurrentHealth == newHealth)
        {
            UpdateHealthUI();

            return;
        }

        CurrentHealth = newHealth;

        OnHealthChanged?.Invoke(CurrentHealth);
        UpdateHealthUI();

        if (CurrentHealth <= MinimumHealth)
        {
            OnDeath?.Invoke();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (damageAmount <= MinimumHealth || CurrentHealth <= MinimumHealth)
        {
            return;
        }

        int remainingDamage = damageAmount;

        if (_armorManager != null && _armorManager.HasArmor)
        {
            remainingDamage = _armorManager.TakeArmorDamage(damageAmount);
        }

        if (remainingDamage <= MinimumHealth)
        {
            return;
        }

        SetHealth(CurrentHealth - remainingDamage);

        _audioController?.PlayTakeDamageSound();
    }

    public void Heal(int healAmount)
    {
        if (healAmount <= MinimumHealth)
        {
            return;
        }

        SetHealth(CurrentHealth + healAmount);
    }

    public void FullHeal()
    {
        SetHealth(_maxHealth);
    }

    public void AddArmor(int armorAmount)
    {
        _armorManager?.AddArmor(armorAmount);
    }

    public void FillArmor()
    {
        _armorManager?.FillArmor();
    }

    public void ResetArmor()
    {
        _armorManager?.ResetArmor();
    }

    public void RestoreOneArmor()
    {
        _armorManager?.AddArmor(1);
    }

    private void InitializeComponents()
    {
        if (_healthBarUI == null)
        {
            _healthBarUI = FindFirstObjectByType<HealthBarUI>();
        }

        if (_armorManager == null)
        {
            _armorManager = GetComponent<ArmorManager>();

            if (_armorManager == null)
            {
                _armorManager = FindFirstObjectByType<ArmorManager>();
            }
        }

        if (_audioController == null)
        {
            _audioController = GetComponent<AudioController>();

            if (_audioController == null)
            {
                _audioController = FindFirstObjectByType<AudioController>();
            }
        }
    }

    private void UpdateHealthUI()
    {
        _healthBarUI?.SetHealth(CurrentHealth);
    }
}