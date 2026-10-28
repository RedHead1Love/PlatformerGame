using Player.StateMachine;
using Player;
using Shared.Sensors;
using UnityEngine;

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
        if (_hero.AnimationService.IsAnimationFinished(HeroAnimationState.AirAttack.ToString()))
        {
            _hero.StateMachine.Change<JumpState>();
        }
    }

    public override void OnAnimationEnd()
    {
        if (_groundCheck != null && _groundCheck.IsGrounded)
        {
            _hero.StateMachine.Change<IdleState>();
        }
        else
        {
            _hero.StateMachine.Change<JumpState>();
        }
    }

    protected override void PlayAttackSound(bool hitConnected)
    {
        if (_hero.AudioController == null)
        {
            return;
        }

        if (hitConnected)
        {
            _hero.AudioController.PlayAirAttackHitSound();
        }
        else
        {
            _hero.AudioController.PlayAirAttackMissSound();
        }
    }
}
