using UnityEngine;

namespace Player.StateMachine
{
    public sealed class Attack2State : BaseAttackState
    {
        public Attack2State(Hero hero, Transform attackPoint)
            : base(hero, attackPoint, hero.Data.Attack2Damage, States.Attack2) { }

        protected override void PlayAttackSound(bool hitConnected)
        {
            if (_hero.AudioController == null)
            {
                return;
            }

            if (hitConnected)
            {
                _hero.AudioController.PlayAttack2HitSound();
            }
            else
            {
                _hero.AudioController.PlayAttack2MissSound();
            }
        }
    }
}