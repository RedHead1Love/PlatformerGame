using UnityEngine;

[CreateAssetMenu(fileName = "SoundConfiguration", menuName = "Game/SoundConfiguration")]
public sealed class SoundConfiguration : ScriptableObject
{
    [Header("Attack Sound Delays")]
    public float Attack1SoundDelay = 0f;
    public float Attack2SoundDelay = 0f;
    public float Attack3SoundDelay = 0f;
    public float AirAttackSoundDelay = 0f;

    [Header("Attack Hit Sounds")]
    public AudioClip Attack1HitSound;
    public AudioClip Attack2HitSound;
    public AudioClip Attack3HitSound;
    public AudioClip AirAttackHitSound;

    [Header("Attack Miss Sounds")]
    public AudioClip Attack1MissSound;
    public AudioClip Attack2MissSound;
    public AudioClip Attack3MissSound;
    public AudioClip AirAttackMissSound;

    [Header("Door Sounds")]
    public AudioClip BossDoorOpenSound;
}