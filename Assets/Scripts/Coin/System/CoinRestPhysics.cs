using GameLogic;
using UnityEngine;

namespace Foxberry
{
    internal static class CoinRestPhysics
    {
        public static void Settle(Rigidbody2D body, Collider2D collider, ICoin coin)
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.bodyType = RigidbodyType2D.Kinematic;
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            coin?.EnableCollection();
        }
    }
}
