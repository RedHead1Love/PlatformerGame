using Player.Abilities;
using UnityEngine;

public sealed class DamageCalculator
{
    private const float NeutralMultiplier = 1f;
    private const float TextVerticalOffset = 2f;
    private const float TextDestroyDelay = 2f;
    private const int TextFontSize = 36;

    private readonly AbilityManager _abilityManager;

    public DamageCalculator(AbilityManager abilityManager)
    {
        _abilityManager = abilityManager;
    }

    public int CalculateDamageWithBonuses(int baseDamage, GameObject target = null)
    {
        if (_abilityManager == null || target == null)
        {
            return baseDamage;
        }

        float multiplier = _abilityManager.GetDamageMultiplierForEnemy(target);

        if (multiplier <= NeutralMultiplier)
        {
            return baseDamage;
        }

        int finalDamage = Mathf.CeilToInt(baseDamage * multiplier);

        ShowBonusDamageEffect(target, multiplier);

        return finalDamage;
    }

    private void ShowBonusDamageEffect(GameObject target, float multiplier)
    {
        EnemyTypeComponent enemyTypeComponent = target.GetComponent<EnemyTypeComponent>();

        if (enemyTypeComponent == null)
        {
            enemyTypeComponent = target.GetComponentInParent<EnemyTypeComponent>();
        }

        string enemyTypeName = enemyTypeComponent != null
            ? GetEnemyTypeName(enemyTypeComponent.EnemyType)
            : "враг";

        CreateBonusDamageText(target, $"{enemyTypeName} x{multiplier}!");
    }

    private string GetEnemyTypeName(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Swamp => "Болото",
            EnemyType.Skeleton => "Скелет",
            EnemyType.Demon => "Демон",
            EnemyType.Spider => "Паук",
            EnemyType.Zombie => "Зомби",
            EnemyType.Boss => "Босс",
            _ => "Враг"
        };
    }

    private void CreateBonusDamageText(GameObject target, string text)
    {
        GameObject textObject = new GameObject("BonusDamageText");

        textObject.transform.position = target.transform.position + Vector3.up * TextVerticalOffset;

        TextMesh textMesh = textObject.AddComponent<TextMesh>();

        textMesh.text = text;
        textMesh.color = Color.red;
        textMesh.fontSize = TextFontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontStyle = FontStyle.Bold;

        Object.Destroy(textObject, TextDestroyDelay);
    }
}