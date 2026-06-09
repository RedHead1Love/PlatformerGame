using UnityEngine;

namespace Player.StateMachine
{
    public sealed class HurtState : IState
    {
        private const string HurtAnimationName = "Hurt";

        private readonly Hero _hero;
        private float _timer;

        public HurtState(Hero hero)
        {
            _hero = hero;
        }

        public void Enter()
        {
            _hero.AnimationService.SetState(States.Hurt);
            _timer = 0f;
        }

        public void Tick()
        {
            _timer += Time.deltaTime;

            float animationLength = _hero.AnimationService.GetAnimationLength(HurtAnimationName);

            if (animationLength <= 0f || _timer >= animationLength)
            {
                _hero.StateMachine.Change<IdleState>();
            }
        }

        public void FixedTick() { }

        public void Exit() { }
    }
}