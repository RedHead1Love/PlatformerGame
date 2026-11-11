using AudioSystem;
using UnityEngine;

namespace Player
{
    public sealed class ArmorManager : MonoBehaviour, IArmorManager
    {
        private const int MinimumArmorValue = 0;

        [SerializeField]
        private int _maxArmor = 6;
        [SerializeField]
        private int _currentArmor = 6;
        [SerializeField]
        private bool _startEnabled = false;
        [SerializeField]
        private GameObject _armorPanelPrefab;
        [SerializeField]
        private bool _useAudioController = true;
        private AudioController _audioController;
        private readonly ArmorView _view = new ArmorView();
        private Hero _hero;
        private bool _isArmorUnlocked;

        public event System.Action<int, int> OnArmorChanged;

        public int CurrentArmor => _currentArmor;

        public int MaxArmor => _maxArmor;

        public bool HasArmor => IsArmorUnlocked() && _currentArmor > MinimumArmorValue;

        public bool IsArmorUnlocked()
        {
            return _isArmorUnlocked || (_hero != null && _hero.AbilityManager != null && _hero.AbilityManager.HasArmor);
        }

        public void SetArmor(int amount)
        {
            if (IsArmorUnlocked() == false)
            {
                return;
            }

            int validArmor = Mathf.Clamp(amount, MinimumArmorValue, _maxArmor);

            if (_currentArmor == validArmor)
            {
                UpdateArmorUI();

                return;
            }

            _currentArmor = validArmor;

            UpdateArmorUI();
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        public void AddArmor(int amount)
        {
            if (amount <= MinimumArmorValue || IsArmorUnlocked() == false)
            {
                return;
            }

            SetArmor(_currentArmor + amount);
        }

        public int TakeArmorDamage(int damageAmount)
        {
            if (damageAmount <= MinimumArmorValue)
            {
                return MinimumArmorValue;
            }

            if (HasArmor == false)
            {
                return damageAmount;
            }

            int oldArmor = _currentArmor;
            int absorbedDamage = Mathf.Min(damageAmount, _currentArmor);
            int remainingDamage = damageAmount - absorbedDamage;

            SetArmor(_currentArmor - absorbedDamage);
            PlayArmorDamageSound(oldArmor, _currentArmor);

            return remainingDamage;
        }

        public void FillArmor()
        {
            if (IsArmorUnlocked() == false)
            {
                return;
            }

            SetArmor(_maxArmor);
        }

        public void ResetArmor()
        {
            if (IsArmorUnlocked() == false)
            {
                return;
            }

            SetArmor(MinimumArmorValue);
        }

        public void UnlockArmor()
        {
            if (_isArmorUnlocked)
            {
                return;
            }

            _isArmorUnlocked = true;

            _view.Show(_armorPanelPrefab);
            SetArmor(_maxArmor);
        }

        public void UnlockArmorAbility()
        {
            UnlockArmor();
        }

        public void LoadArmorFromSave(int armorFromSave)
        {
            if (IsArmorUnlocked() == false)
            {
                return;
            }

            int validArmor = Mathf.Clamp(armorFromSave, MinimumArmorValue, _maxArmor);

            SetArmor(validArmor);
        }

        public bool CanPurchaseArmorPlates()
        {
            return IsArmorUnlocked() && _currentArmor < _maxArmor;
        }

        public bool NeedArmorPlates()
        {
            return CanPurchaseArmorPlates();
        }

        public void UpdateArmorUI() => _view.UpdateIcons(_currentArmor, _maxArmor);

        private void Start()
        {
            InitializeReferences();
            InitializeArmorState();
        }

        private void InitializeReferences()
        {
            _hero = GetComponent<Hero>();

            if (_hero == null)
            {
                _hero = FindFirstObjectByType<Hero>();
            }

            if (_useAudioController)
            {
                _audioController = GetComponentInParent<AudioController>();

                if (_audioController == null)
                {
                    _audioController = FindFirstObjectByType<AudioController>();
                }
            }
        }

        private void InitializeArmorState()
        {
            _isArmorUnlocked =
                _startEnabled || (_hero != null && _hero.AbilityManager != null && _hero.AbilityManager.HasArmor);

            if (IsArmorUnlocked())
            {
                _view.Show(_armorPanelPrefab);
                UpdateArmorUI();
            }
            else
            {
                _view.Hide();
            }
        }

        private void PlayArmorDamageSound(int oldArmor, int newArmor)
        {
            if (_useAudioController == false || _audioController == null)
            {
                return;
            }

            if (newArmor <= MinimumArmorValue && oldArmor > MinimumArmorValue)
            {
                _audioController.PlayArmorBreakSound();

                return;
            }

            _audioController.PlayArmorDamageSound();
        }
    }
}
