using UnityEngine;

[CreateAssetMenu(fileName = "SoundConfiguration", menuName = "Game/SoundConfiguration")]
public sealed class SoundConfiguration : ScriptableObject
{
    [Header("Attack Sound Delays")]
    [SerializeField] private float _attack1SoundDelay;
    [SerializeField] private float _attack2SoundDelay;
    [SerializeField] private float _attack3SoundDelay;
    [SerializeField] private float _airAttackSoundDelay;

    [Header("Attack Hit Sounds")]
    [SerializeField] private AudioClip _attack1HitSound;
    [SerializeField] private AudioClip _attack2HitSound;
    [SerializeField] private AudioClip _attack3HitSound;
    [SerializeField] private AudioClip _airAttackHitSound;

    [Header("Attack Miss Sounds")]
    [SerializeField] private AudioClip _attack1MissSound;
    [SerializeField] private AudioClip _attack2MissSound;
    [SerializeField] private AudioClip _attack3MissSound;
    [SerializeField] private AudioClip _airAttackMissSound;

    [Header("Door Sounds")]
    [SerializeField] private AudioClip _bossDoorOpenSound;

    public float Attack1SoundDelay => _attack1SoundDelay;
    public float Attack2SoundDelay => _attack2SoundDelay;
    public float Attack3SoundDelay => _attack3SoundDelay;
    public float AirAttackSoundDelay => _airAttackSoundDelay;

    public AudioClip Attack1HitSound => _attack1HitSound;
    public AudioClip Attack2HitSound => _attack2HitSound;
    public AudioClip Attack3HitSound => _attack3HitSound;
    public AudioClip AirAttackHitSound => _airAttackHitSound;

    public AudioClip Attack1MissSound => _attack1MissSound;
    public AudioClip Attack2MissSound => _attack2MissSound;
    public AudioClip Attack3MissSound => _attack3MissSound;
    public AudioClip AirAttackMissSound => _airAttackMissSound;

    public AudioClip BossDoorOpenSound => _bossDoorOpenSound;
}