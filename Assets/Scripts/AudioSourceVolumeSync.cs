using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class AudioSourceVolumeSync : MonoBehaviour
{
    private const string MusicPrefsKey = "MusicVolume";
    private const string SFXPrefsKey = "SFXVolume";
    private const float DefaultVolume = 0.8f;

    [SerializeField] private bool _isMusic = false;

    private AudioSource _audioSource;
    private float _originalVolume = 1f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null)
        {
            _originalVolume = _audioSource.volume;
        }
    }

    private void OnEnable()
    {
        if (_isMusic)
        {
            VolumeSettings.OnMusicVolumeChanged += UpdateVolume;
            UpdateVolume(PlayerPrefs.GetFloat(MusicPrefsKey, DefaultVolume));
        }
        else
        {
            VolumeSettings.OnSFXVolumeChanged += UpdateVolume;
            UpdateVolume(PlayerPrefs.GetFloat(SFXPrefsKey, DefaultVolume));
        }
    }

    private void OnDisable()
    {
        if (_isMusic)
        {
            VolumeSettings.OnMusicVolumeChanged -= UpdateVolume;
        }
        else
        {
            VolumeSettings.OnSFXVolumeChanged -= UpdateVolume;
        }
    }

    private void UpdateVolume(float globalVolume)
    {
        if (_audioSource != null)
        {
            _audioSource.volume = _originalVolume * globalVolume;
        }
    }
}