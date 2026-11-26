using System;
using AudioSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Foxberry
{
    public sealed class VolumeSettings : MonoBehaviour
    {
        private const string MusicPrefsKey = "MusicVolume";
        private const string SFXPrefsKey = "SFXVolume";
        private const float DefaultVolume = 0.8f;
        [SerializeField]
        private AudioController _audioController;
        [SerializeField]
        private Slider _musicSlider;
        [SerializeField]
        private Slider _sfxSlider;

        public static event Action<float> OnMusicVolumeChanged;

        public static event Action<float> OnSFXVolumeChanged;

        private void Start()
        {
            if (_audioController == null)
            {
                _audioController = FindObjectOfType<AudioController>();
            }

            InitializeSliders();
            LoadSavedVolumes();
        }

        private void OnDestroy()
        {
            if (_musicSlider != null)
            {
                _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeSliderChanged);
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeSliderChanged);
            }
        }

        private void InitializeSliders()
        {
            if (_musicSlider != null)
            {
                _musicSlider.minValue = 0f;
                _musicSlider.maxValue = 1f;
                _musicSlider.onValueChanged.AddListener(OnMusicVolumeSliderChanged);
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.minValue = 0f;
                _sfxSlider.maxValue = 1f;
                _sfxSlider.onValueChanged.AddListener(OnSfxVolumeSliderChanged);
            }
        }

        private void LoadSavedVolumes()
        {
            float musicVol = PlayerPrefs.GetFloat(MusicPrefsKey, DefaultVolume);
            float sfxVol = PlayerPrefs.GetFloat(SFXPrefsKey, DefaultVolume);
            if (_musicSlider != null)
            {
                _musicSlider.value = musicVol;
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.value = sfxVol;
            }

            OnMusicVolumeSliderChanged(musicVol);
            OnSfxVolumeSliderChanged(sfxVol);
        }

        private void OnMusicVolumeSliderChanged(float volume)
        {
            PlayerPrefs.SetFloat(MusicPrefsKey, volume);
            PlayerPrefs.Save();
            if (_audioController != null)
            {
                _audioController.SetMusicVolume(volume);
            }

            OnMusicVolumeChanged?.Invoke(volume);
        }

        private void OnSfxVolumeSliderChanged(float volume)
        {
            PlayerPrefs.SetFloat(SFXPrefsKey, volume);
            PlayerPrefs.Save();
            if (_audioController != null)
            {
                _audioController.SetSoundEffectsVolume(volume);
            }

            OnSFXVolumeChanged?.Invoke(volume);
        }
    }
}
