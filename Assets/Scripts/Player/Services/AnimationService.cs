using System.Linq;
using UnityEngine;

namespace Player
{
    public sealed class AnimationService
    {
        private const int BaseLayerIndex = 0;
        private const float AnimationEndThreshold = 1f;
        private const string StateParameterName = "state";

        private readonly Animator _animator;
        private readonly int _stateParameterHash;

        public AnimationService(Animator animator)
        {
            _animator = animator;
            _stateParameterHash = Animator.StringToHash(StateParameterName);
        }

        public void SetState(States state)
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetInteger(_stateParameterHash, (int)state);
        }

        public void SetFloat(string parameterName, float value)
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetFloat(parameterName, value);
        }

        public bool IsAnimationFinished(string stateName)
        {
            if (_animator == null)
            {
                return true;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);

            return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= AnimationEndThreshold;
        }

        public float GetAnimationLength(string stateName)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
            {
                return 0f;
            }

            AnimationClip clip = _animator.runtimeAnimatorController.animationClips
                .FirstOrDefault(animationClip => animationClip.name == stateName);

            return clip != null ? clip.length : 0f;
        }
    }
}