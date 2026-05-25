using Player;
using System.Collections;
using UnityEngine;
using YG;
using UnityEngine.SceneManagement;

namespace Player.StateMachine
{
    public sealed class DieState : IState
    {
        private const string DieAnimationName = "Die";
        private const string StateParameterName = "state";
        private const float RestartAnimationThreshold = 0.55f;
        private const float RestartDelay = 1f;
        private const int BaseLayerIndex = 0;

        private readonly Hero _hero;
        private readonly Animator _animator;
        private bool _isRestarting;

        public DieState(Hero hero)
        {
            _hero = hero;
            _animator = hero.GetComponent<Animator>();
        }

        public void Enter()
        {
            _animator.SetInteger(StateParameterName, (int)States.Hurt);
            _isRestarting = false;
        }

        public void Tick()
        {
            if (!_isRestarting && IsDieAnimationFinished())
            {
                _isRestarting = true;

                _hero.StartCoroutine(RestartGameCoroutine());
            }
        }

        public void FixedTick() { }

        public void Exit() { }

    private bool IsDieAnimationFinished()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        YG2.InterstitialAdvShow();
        private bool IsDieAnimationFinished()
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);

            return stateInfo.IsName(DieAnimationName) && stateInfo.normalizedTime >= RestartAnimationThreshold;
        }

        private IEnumerator RestartGameCoroutine()
        {
            yield return new WaitForSeconds(RestartDelay);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
