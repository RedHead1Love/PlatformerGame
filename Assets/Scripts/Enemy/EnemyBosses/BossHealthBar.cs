using GeneralLogicEnemies;
using System.Reflection;
using UnityEngine;

public sealed class BossHealthBar : MonoBehaviour
{
    private const float HealthBarWidth = 300f;
    private const float HealthBarHeight = 30f;
    private const float TextOffsetY = 25f;
    private const float FontSize = 16f;

    [SerializeField] private Vector2 _healthBarOffset = new Vector2(0f, 3f);

    private Camera _mainCamera;
    private Entity _bossEntity;
    private FieldInfo _livesFieldInfo;
    private int _maxHealth;

    private void Start()
    {
        _bossEntity = GetComponent<Entity>();
        _mainCamera = Camera.main ?? FindFirstObjectByType<Camera>();

        _livesFieldInfo = typeof(Entity).GetField("lives", BindingFlags.NonPublic | BindingFlags.Instance);
        if (_bossEntity != null && _livesFieldInfo != null)
        {
            _maxHealth = (int)_livesFieldInfo.GetValue(_bossEntity);
        }
    }

    private void Update()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
    }

    private void OnGUI()
    {
        if (_bossEntity == null || !_bossEntity.IsAlive) return;

    }
}
