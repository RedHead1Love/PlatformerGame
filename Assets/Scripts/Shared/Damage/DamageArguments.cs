using UnityEngine;

namespace Shared.Damage
{
    public readonly struct DamageArguments
    {
        public readonly int Amount;
        public readonly Vector2 Direction;

        public DamageArguments(int amount, Vector2 direction)
        {
            Amount = amount;
            Direction = direction;
        }
    }
}