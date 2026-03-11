using UnityEngine;

public class EnvelopeShakeAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public void StartShake()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopShake()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}