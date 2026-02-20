using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public float fadeDuration = 1.5f;

    [Header("Scene Music Clips")]
    public AudioClip scene1Music;
    public AudioClip scene2Music;
    public AudioClip scene3Music;
    // Add more clips per scene here

    private Coroutine currentFade;

    private void Awake()
    {
        // Singleton check: merge with existing instance if one exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Merge music source if new AudioManager has a source but instance doesn't
            if (instance.musicSource == null && musicSource != null)
                instance.musicSource = musicSource;

            Destroy(gameObject);
            return;
        }

        // Subscribe to scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipToPlay = null;

        switch (scene.name)
        {
            case "Scene1":
                clipToPlay = scene1Music;
                break;
            case "Scene2":
                clipToPlay = scene2Music;
                break;
            case "Scene3":
                clipToPlay = scene3Music;
                break;
            default:
                clipToPlay = null;
                break;
        }

        if (clipToPlay != null)
            PlayMusic(clipToPlay);
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (musicSource == null || musicSource.clip == newClip) return;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeMusic(newClip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        // Fade out
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        // Switch clip
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
        currentFade = null;
    }

    public void StopMusic()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        if (musicSource == null) yield break;

        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
        currentFade = null;
    }
}