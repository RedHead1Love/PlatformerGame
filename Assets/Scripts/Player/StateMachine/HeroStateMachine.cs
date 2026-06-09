using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.StateMachine
{
    public sealed class HeroStateMachine
    {
        private const int MinimumAttackNumber = 0;
        private const int MaximumAttackNumber = 2;

        private readonly Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private readonly Hero _hero;

        private IState _currentState;

        public IState Current => _currentState;

        public HeroStateMachine(Hero hero)
        {
            _hero = hero;

            InitializeStates();
        }

        public void Change<TState>() where TState : IState
        {
            if (_states.TryGetValue(typeof(TState), out IState nextState) == false)
            {
                return;
            }

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();
        }

        public void PerformRandomAttack()
        {
            int randomAttackIndex = UnityEngine.Random.Range(MinimumAttackNumber, MaximumAttackNumber);

            if (randomAttackIndex == MinimumAttackNumber)
            {
                Change<Attack1State>();
            }
            else
            {
                Change<Attack2State>();
            }
        }

        public void Tick()
        {
            _currentState?.Tick();
        }

        public void FixedTick()
        {
            _currentState?.FixedTick();
        }

        private void InitializeStates()
        {
            _states[typeof(IdleState)] = new IdleState(_hero);
            _states[typeof(RunState)] = new RunState(_hero);
            _states[typeof(JumpState)] = new JumpState(_hero);
            _states[typeof(SlideState)] = new SlideState(_hero);
            _states[typeof(Attack1State)] = new Attack1State(_hero, _hero.AttackPoint);
            _states[typeof(Attack2State)] = new Attack2State(_hero, _hero.AttackPoint);
            _states[typeof(Attack3State)] = new Attack3State(_hero, _hero.AttackPoint);
            _states[typeof(AirAttackState)] = new AirAttackState(_hero, _hero.AttackPoint);
            _states[typeof(HurtState)] = new HurtState(_hero);
            _states[typeof(DieState)] = new DieState(_hero);
        }
    }
}