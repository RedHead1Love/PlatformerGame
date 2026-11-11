using UnityEngine;

namespace Player.StateMachine
{
    public sealed class Attack1State : BaseAttackState
    {
        public Attack1State(Hero hero, Transform attackPoint)
            : base(hero, attackPoint, hero.Data.Attack1Damage, HeroAnimationState.Attack)
        {
        }
    }
}
