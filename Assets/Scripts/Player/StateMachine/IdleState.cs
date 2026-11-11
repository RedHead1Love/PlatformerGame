using Player.Input;
using Shared.Sensors;
using UnityEngine;

namespace Player.StateMachine
{
    public sealed class IdleState : IState
    {
        private const float HorizontalDeadZone = 0.1f;

        private readonly Hero _hero;
        private readonly IInputProvider _inputProvider;
        private readonly GroundSensor _groundCheck;
        private readonly EnemyScanner _enemyScanner;
        private readonly Rigidbody2D _rigidbody;

        public IdleState(Hero hero)
        {
            _hero = hero;
            _inputProvider = hero.GetComponent<IInputProvider>();
            _groundCheck = hero.GetComponentInChildren<GroundSensor>();
            _enemyScanner = hero.GetComponentInChildren<EnemyScanner>();
            _rigidbody = hero.Rigidbody;
        }

        public void Enter()
        {
            UpdateAnimationState();
        }

        public void Tick()
        {
            UpdateAnimationState();

            if (_groundCheck != null && _groundCheck.IsGrounded == false)
            {
                _hero.StateMachine.Change<JumpState>();

                return;
            }

            HandleInput();
        }

        public void FixedTick()
        {
            StopHorizontalMovement();
        }

        public void Exit()
        {
        }

        private void UpdateAnimationState()
        {
            bool hasEnemyNearby = _enemyScanner != null && _enemyScanner.HasEnemyNearby;
            HeroAnimationState targetState = hasEnemyNearby ? HeroAnimationState.Idle2 : HeroAnimationState.Idle;

            _hero.AnimationService.SetState(targetState);
        }

        private void HandleInput()
        {
            switch (_inputProvider)
            {
                case null:
                    return;
                case var input when Mathf.Abs(input.HorizontalAxis) > HorizontalDeadZone:
                    _hero.StateMachine.Change<RunState>();
                    break;
                case var action when action.IsJumpPressed:
                    _hero.StateMachine.Change<JumpState>();
                    break;
                case var action when action.IsSlidePressed:
                    _hero.StateMachine.Change<SlideState>();
                    break;
                case var action when action.IsAttackPressed:
                    _hero.StateMachine.PerformRandomAttack();
                    break;
                case var action when action.IsSecondaryAttackPressed:
                    _hero.StateMachine.Change<Attack3State>();
                    break;
            }
        }

        private void StopHorizontalMovement()
        {
            if (_rigidbody == null)
            {
                return;
            }

            Vector2 velocity = _rigidbody.velocity;

            _rigidbody.velocity = new Vector2(0f, velocity.y);
        }
    }
}
