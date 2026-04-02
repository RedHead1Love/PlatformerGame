using UnityEngine;

public sealed class SystemLoader : MonoBehaviour
{
    [SerializeField] private GameObject _systemManagersPrefab;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (SaveSystem.Instance == null && _systemManagersPrefab != null)
        {
            Instantiate(_systemManagersPrefab);
        }
    }
}