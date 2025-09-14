using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Hit")]
    [SerializeField] AudioClip hitClip;
    [SerializeField][Range(0f, 1f)] float hitVolume;

    [Header("Moving")]
    [SerializeField] AudioClip moveClip;
    [SerializeField][Range(0f, 1f)] float moveVolume;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    public void PlayHitSound()
    {
        StopAllSounds();
        PlayClip(hitClip, hitVolume);
    }

    public void PlayMoveSound()
    {
        PlayClip(moveClip, moveVolume);
    }

    public void StopAllSounds()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    void PlayClip(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}