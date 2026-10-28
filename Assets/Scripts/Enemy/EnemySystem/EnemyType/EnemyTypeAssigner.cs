#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class EnemyTypeAssigner : EditorWindow
{
    [MenuItem("Tools/ EnemyType ")]
    private static void ShowWindow()
    {
        GetWindow<EnemyTypeAssigner>(" EnemyType");
    }

    private void OnGUI()
    {
        GUILayout.Label(" EnemyType ", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("  :", EditorStyles.boldLabel);

        DrawNameButtons();

        EditorGUILayout.Space();

        GUILayout.Label(" :", EditorStyles.boldLabel);

        DrawManualAssignmentButton();
    }

    private void DrawNameButtons()
    {
        DrawButton("Swamp ()", "swamp", EnemyType.Swamp);
        DrawButton("Skeleton ()", "skeleton", EnemyType.Skeleton);
        DrawButton("Demon ()", "demon", EnemyType.Demon);
        DrawButton("Spider ()", "spider", EnemyType.Spider);
        DrawButton("Zombie ()", "zombie", EnemyType.Zombie);
        DrawButton("Boss ()", "boss", EnemyType.Boss);
    }

    private void DrawButton(string buttonName, string namePart, EnemyType enemyType)
    {
        if (GUILayout.Button(buttonName))
        {
            AssignEnemyTypeByName(namePart, enemyType);
        }
    }

    private void DrawManualAssignmentButton()
    {
        if (GUILayout.Button("  "))
        {
            ShowEnemyTypeSelector();
        }
    }

    private static void AssignEnemyTypeByName(string namePart, EnemyType enemyType)
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int changedCount = 0;

        foreach (GameObject gameObject in allObjects)
        {
            if (gameObject.name.ToLower().Contains(namePart) == false)
            {
                continue;
            }

            AddOrUpdateEnemyTypeComponent(gameObject, enemyType);
            changedCount++;
        }

        Debug.Log($"EnemyType  : {changedCount}");
    }

    private static void ShowEnemyTypeSelector()
    {
        GenericMenu menu = new GenericMenu();

        foreach (EnemyType enemyType in System.Enum.GetValues(typeof(EnemyType)))
        {
            EnemyType capturedType = enemyType;

            menu.AddItem(
                new GUIContent(capturedType.ToString()),
                false,
                () => AssignEnemyTypeToSelected(capturedType));
        }

        menu.ShowAsContext();
    }

    private static void AssignEnemyTypeToSelected(EnemyType enemyType)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            AddOrUpdateEnemyTypeComponent(selectedObject, enemyType);
        }
    }

    private static void AddOrUpdateEnemyTypeComponent(GameObject targetObject, EnemyType enemyType)
    {
        EnemyTypeComponent component = targetObject.GetComponent<EnemyTypeComponent>();

        if (component == null)
        {
            component = targetObject.AddComponent<EnemyTypeComponent>();
        }

        component.SetEnemyType(enemyType);

        EditorUtility.SetDirty(targetObject);
    }
}
#endif