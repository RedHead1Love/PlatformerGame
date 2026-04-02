using UnityEngine;

public sealed class GameSceneSystemLoader : MonoBehaviour
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