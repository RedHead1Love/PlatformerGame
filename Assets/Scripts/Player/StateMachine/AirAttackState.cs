using Shared.Sensors;
using UnityEngine;

namespace Player.StateMachine
{
    public sealed class AirAttackState : BaseAttackState
    {
        private readonly GroundSensor _groundCheck;

        public AirAttackState(Hero hero, Transform attackPoint)
            : base(hero, attackPoint, hero.Data.AirAttackDamage, HeroAnimationState.AirAttack)
        {
            _groundCheck = hero.GetComponentInChildren<GroundSensor>();
        }

        public override void Tick()
        {
            if (Hero.AnimationService.IsAnimationFinished(HeroAnimationState.AirAttack.ToString()))
            {
                Hero.StateMachine.Change<JumpState>();
            }
        }

        public override void OnAnimationEnd()
        {
            if (_groundCheck != null && _groundCheck.IsGrounded)
            {
                Hero.StateMachine.Change<IdleState>();
            }
            else
            {
                Hero.StateMachine.Change<JumpState>();
            }
        }
    }
}
