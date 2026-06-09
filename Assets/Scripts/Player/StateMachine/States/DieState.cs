using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace Player.StateMachine
{
    public sealed class DieState : IState
    {
        private const float RestartDelay = 1.5f;

        private readonly Hero _hero;
        private readonly Rigidbody2D _rigidbody;

        private bool _isRestarting;
        private bool _wasAdShown;

        public DieState(Hero hero)
        {
            _hero = hero;
            _rigidbody = hero.GetComponent<Rigidbody2D>();
        }

        public void Enter()
        {
            if (_isRestarting)
            {
                return;
            }

            _isRestarting = true;
            _wasAdShown = false;

            FreezeHero();
            _hero.AnimationService.SetState(States.Die);
            _hero.StartCoroutine(RestartGame());
        }

        public void Tick() { }

        public void FixedTick() { }

        public void Exit() { }

        private void FreezeHero()
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.gravityScale = 0f;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        private IEnumerator RestartGame()
        {
            ShowInterstitialAdOnce();

            yield return new WaitForSecondsRealtime(RestartDelay);

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ShowInterstitialAdOnce()
        {
            if (_wasAdShown)
            {
                return;
            }

            _wasAdShown = true;

            YG2.InterstitialAdvShow();
        }
    }
}