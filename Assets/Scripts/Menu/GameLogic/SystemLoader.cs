using UnityEngine;

public sealed class SystemLoader : MonoBehaviour
{
    private const float NormalTimeScale = 1f;

    [SerializeField] private GameObject _systemManagersPrefab;

    private void Awake()
    {
        Time.timeScale = NormalTimeScale;

        if (SaveSystem.Instance == null && _systemManagersPrefab != null)
        {
            Instantiate(_systemManagersPrefab);
        }
    }
}