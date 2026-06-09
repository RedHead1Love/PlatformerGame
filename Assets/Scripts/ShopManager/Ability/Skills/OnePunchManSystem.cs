using GeneralLogicEnemies;
using Player.Abilities;
using UnityEngine;

public sealed class OnePunchManSystem : MonoBehaviour, IOnePunchManSystem
{
    private const float DefaultInstakillChance = 0.5f;
    private const float DefaultInstakillTextHeight = 1f;
    private const float DefaultTextLifetime = 2f;
    private const int InstakillFontSize = 16;

    [SerializeField] private float _instakillChance = DefaultInstakillChance;
    [SerializeField] private bool _showInstakillEffect = true;
    [SerializeField] private AudioClip _instakillSound;

    private Hero _hero;
    private AbilityManager _abilityManager;
    private AudioController _audioController;
    private bool _isActive;

    public float InstakillChance => _instakillChance;
    public bool IsActive => _isActive;

    private void Awake()
    {
        InitializeReferences();
    }

    private void Start()
    {
        CheckIfAbilityPurchased();
    }

    public void Activate()
    {
        _isActive = true;

        RefreshAbilityManagerReference();

        if (_abilityManager != null)
        {
            _abilityManager.HasOnePunchManAbility = true;
        }
    }

    public void Deactivate()
    {
        _isActive = false;
    }

    public bool CheckForInstakill(Entity enemy)
    {
        if (_isActive == false || enemy == null)
        {
            return false;
        }

        if (Random.value > _instakillChance)
        {
            return false;
        }

        PerformInstakill(enemy);

        return true;
    }

    private void InitializeReferences()
    {
        _hero = GetComponent<Hero>();

        if (_hero == null)
        {
            _hero = FindFirstObjectByType<Hero>();
        }

        _audioController = GetComponent<AudioController>();

        if (_audioController == null)
        {
            _audioController = GetComponentInChildren<AudioController>();
        }

        _abilityManager = _hero?.AbilityManager;
    }

    private void CheckIfAbilityPurchased()
    {
        RefreshAbilityManagerReference();

        _isActive = _abilityManager?.HasOnePunchManAbility ?? false;
    }

    private void RefreshAbilityManagerReference()
    {
        if (_abilityManager == null && _hero != null)
        {
            _abilityManager = _hero.AbilityManager;
        }
    }

    private void PerformInstakill(Entity enemy)
    {
        if (_instakillSound != null && _audioController != null)
        {
            _audioController.PlayOneShot(_instakillSound);
        }

        if (_showInstakillEffect)
        {
            ShowInstakillEffect(enemy.transform.position);
        }
    }

    private void ShowInstakillEffect(Vector3 position)
    {
        GameObject textObject = new GameObject("InstakillEffect");

        textObject.transform.position = position + Vector3.up * DefaultInstakillTextHeight;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();

        textMesh.text = "OneShot";
        textMesh.color = Color.magenta;
        textMesh.fontSize = InstakillFontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontStyle = FontStyle.Bold;

        Destroy(textObject, DefaultTextLifetime);
    }
}