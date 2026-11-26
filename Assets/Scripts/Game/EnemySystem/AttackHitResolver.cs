using Shared.Damage;
using UnityEngine;

namespace Foxberry
{
    internal static class AttackHitResolver
    {
        public static bool TryApplyBox(Vector2 center, Vector2 size, int layerMask, int damage)
        {
            Collider2D collider = Physics2D.OverlapBox(center, size, 0f, layerMask);
            if (collider == null)
            {
                return false;
            }

            IDamageable target = collider.GetComponent<IDamageable>() ?? collider.GetComponentInParent<IDamageable>();
            if (target == null)
            {
                return false;
            }

            target.TakeDamage(damage);
            return true;
        }
    }
}
