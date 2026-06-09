using Shared.Damage;
using UnityEngine;

namespace Player
{
    public sealed class EnhancedDamageService
    {
        private const float TextVerticalOffset = 1.5f;
        private const float TextLifetime = 1f;
        private const int TextFontSize = 20;

        private readonly DamageCalculator _damageCalculator;

        public EnhancedDamageService(Hero hero)
        {
            _damageCalculator = new DamageCalculator(hero.AbilityManager);
        }

        public void ApplyDamageToTarget(int baseDamage, GameObject target)
        {
            if (target == null)
            {
                return;
            }

            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
            {
                damageable = target.GetComponentInParent<IDamageable>();
            }

            if (damageable == null)
            {
                return;
            }

            int finalDamage = _damageCalculator.CalculateDamageWithBonuses(baseDamage, target);

            damageable.TakeDamage(finalDamage);

            if (finalDamage > baseDamage)
            {
                ShowBonusDamageEffect(target);
            }
        }

        private void ShowBonusDamageEffect(GameObject target)
        {
            GameObject textObject = new GameObject("BonusDamageText");

            textObject.transform.position = target.transform.position + Vector3.up * TextVerticalOffset;

            TextMesh textMesh = textObject.AddComponent<TextMesh>();

            textMesh.text = "Бонус!";
            textMesh.color = Color.green;
            textMesh.fontSize = TextFontSize;
            textMesh.anchor = TextAnchor.MiddleCenter;

            Object.Destroy(textObject, TextLifetime);
        }
    }
}