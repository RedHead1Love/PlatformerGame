using System;
using UnityEngine;

namespace Foxberry
{
    internal sealed class GamePauseScope : IDisposable
    {
        private static int _owners;
        private static float _resumeTimeScale;
        private bool _active;

        public void Pause()
        {
            if (_active)
            {
                return;
            }

            if (_owners == 0)
            {
                _resumeTimeScale = Time.timeScale;
            }

            _owners++;
            _active = true;
            Time.timeScale = 0f;
        }

        public void Dispose()
        {
            if (!_active)
            {
                return;
            }

            _active = false;
            _owners = Mathf.Max(0, _owners - 1);
            if (_owners == 0 && Time.timeScale == 0f)
            {
                Time.timeScale = _resumeTimeScale;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            _owners = 0;
            _resumeTimeScale = 1f;
        }
    }
}
