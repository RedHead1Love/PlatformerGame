using UnityEngine;

public sealed class SaveSystemInitializer : MonoBehaviour
{
    private void Awake()
    {
        if (SaveSystem.Instance == null)
        {
            CreateSystemManagers();
        }
    }

    private void CreateSystemManagers()
    {
        GameObject systemManagers = new GameObject("SystemManagers");

        systemManagers.AddComponent<SaveSystem>();

        DontDestroyOnLoad(systemManagers);
    }
}
