using UnityEngine;

namespace Player.StateMachine
{
    public sealed class Attack2State : BaseAttackState
    {
        public Attack2State(Hero hero, Transform attackPoint)
            : base(hero, attackPoint, hero.Data.Attack2Damage, HeroAnimationState.Attack2)
        {
        }
    }
}
