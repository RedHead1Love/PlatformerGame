using UnityEngine;

namespace Player.StateMachine
{
    public sealed class Attack3State : BaseAttackState
    {
        public Attack3State(Hero hero, Transform attackPoint)
            : base(hero, attackPoint, hero.Data.SuperAttackDamage, States.Attack3) { }

        protected override void PlayAttackSound(bool hitConnected)
        {
            if (_hero.AudioController == null)
            {
                return;
            }

            if (hitConnected)
            {
                _hero.AudioController.PlayAttack3HitSound();
            }
            else
            {
                _hero.AudioController.PlayAttack3MissSound();
            }
        }
    }
}