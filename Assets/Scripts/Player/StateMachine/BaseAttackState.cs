using Player.Abilities;
using UnityEngine;

namespace Player.StateMachine
{
    public abstract class BaseAttackState : IState, IAnimationEndHandler, IDamageDealer
    {
        private const string EnemyLayerName = "Enemy";
        private const float AttackRange = 1.4f;
        private const float AttackAngle = 140f;
        private const float FacingThreshold = 0f;

        private readonly MeleeAttack _meleeAttack;
        private readonly int _baseDamage;
        private readonly HeroAnimationState _animationState;

        private bool _hasDealtDamage;

        protected BaseAttackState(Hero hero, Transform attackPoint, int baseDamage, HeroAnimationState animationState)
        {
            Hero = hero;
            _baseDamage = baseDamage;
            _animationState = animationState;

            _meleeAttack =
                new MeleeAttack(attackPoint, AttackRange, AttackAngle, LayerMask.GetMask(EnemyLayerName), hero);
        }

        protected Hero Hero { get; }

        public virtual void Enter()
        {
            Hero.AnimationService.SetState(_animationState);
            _hasDealtDamage = false;
        }

        public virtual void Tick()
        {
        }

        public virtual void FixedTick()
        {
        }

        public virtual void Exit()
        {
            _hasDealtDamage = false;
        }

        public virtual void DealDamage()
        {
            if (_hasDealtDamage)
            {
                return;
            }

            _hasDealtDamage = true;

            Vector2 attackDirection = new Vector2(Hero.FacingDirection, FacingThreshold);
            bool hitConnected = _meleeAttack.Perform(_baseDamage, attackDirection);

            PlayAttackSound(hitConnected);
        }

        public virtual void OnAnimationEnd()
        {
            Hero.StateMachine.Change<IdleState>();
        }

        private void PlayAttackSound(bool hitConnected)
        {
            Hero.AudioController?.PlayAttackSound(_animationState, hitConnected);
        }
    }
}
