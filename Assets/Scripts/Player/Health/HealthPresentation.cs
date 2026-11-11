using AudioSystem;
using Foxberry;

namespace Player
{
    internal sealed class HealthPresentation
    {
        private readonly HealthBarUI _bar;
        private readonly LowHealthEffect _lowHealth;
        private readonly AudioController _audio;

        public HealthPresentation(HealthBarUI bar, LowHealthEffect lowHealth, AudioController audio)
        {
            _bar = bar;
            _lowHealth = lowHealth;
            _audio = audio;
        }

        public void Refresh(int current, int maximum)
        {
            if (_bar != null)
            {
                _bar.SetHealth(current);
            }

            if (_lowHealth != null)
            {
                _lowHealth.UpdateHealthState(current, maximum);
            }
        }

        public void PlayDamage()
        {
            if (_audio != null)
            {
                _audio.PlayTakeDamageSound();
            }
        }
    }
}
