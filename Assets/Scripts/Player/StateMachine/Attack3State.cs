using UnityEngine;

namespace Player.StateMachine
{
    public sealed class Attack3State : BaseAttackState
    {
        public Attack3State(Hero hero, Transform attackPoint)
            : base(hero, attackPoint, hero.Data.SuperAttackDamage, HeroAnimationState.Attack3)
        {
        }
    }
}
