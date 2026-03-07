using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFadeIn : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup characterCanvas;      // Character UI CanvasGroup
    public DialogueBubble bubble;            // Dialogue system reference
    public Image blackScreen;                // Fullscreen black Image for fade effect

    [Header("Opening Sounds")]
    public AudioSource firstSound;
    public AudioSource secondSound;

    [Header("Closing Sound")]
    public AudioSource closingDoorSound;

    [Header("Timing Settings")]
    public float disappearDelay = 0.2f;      // Wait before black screen fade
    public float blackFadeDuration = 0.15f;  // Duration of black fade in/out
    public float doorSoundDelay = 0.1f;      // Delay after fade to play door sound

    private bool soundsFinished = false;
    private bool dialogueAllowed = false;
    private bool characterHasAppeared = false;
    private Coroutine soundRoutine;

    void Start()
    {
        if (characterCanvas != null)
            characterCanvas.alpha = 0f;

        if (blackScreen != null)
            blackScreen.color = new Color(0f, 0f, 0f, 0f); // Start transparent

        if (bubble != null)
            bubble.OnDialogueEnded += OnDialogueEnded;

        soundRoutine = StartCoroutine(PlaySounds());
    }

    void OnDestroy()
    {
        if (bubble != null)
            bubble.OnDialogueEnded -= OnDialogueEnded;
    }

    void Update()
    {
        // Skip opening sounds
        if (!soundsFinished && Input.GetKeyDown(KeyCode.Space))
        {
            SkipSounds();
            return;
        }

        if (!soundsFinished)
            return;

        // Auto appear character only once
        if (!characterHasAppeared)
        {
            characterHasAppeared = true;
            if (characterCanvas != null)
                characterCanvas.alpha = 1f;

            dialogueAllowed = true;
        }

        // Click → start dialogue
        if (dialogueAllowed && Input.GetMouseButtonDown(0))
        {
            bubble.AllowDialogueStart();
            dialogueAllowed = false;
        }
    }

    IEnumerator PlaySounds()
    {
        if (firstSound != null && firstSound.clip != null)
        {
            firstSound.Play();
            yield return new WaitForSeconds(firstSound.clip.length);
        }

        if (secondSound != null && secondSound.clip != null)
        {
            secondSound.Play();
            yield return new WaitForSeconds(secondSound.clip.length);
        }

        soundsFinished = true;
    }

    void SkipSounds()
    {
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);

        if (firstSound != null) firstSound.Stop();
        if (secondSound != null) secondSound.Stop();

        soundsFinished = true;
    }

    // Called automatically when dialogue ends
    void OnDialogueEnded()
    {
        StartCoroutine(DisappearWithBlackFade());
    }

    IEnumerator DisappearWithBlackFade()
    {
        // Wait a tiny delay before fade (optional for timing)
        yield return new WaitForSeconds(disappearDelay);

        // Fade black screen in
        if (blackScreen != null)
            yield return StartCoroutine(FadeImage(blackScreen, 0f, 1f, blackFadeDuration));

        // Hide character
        if (characterCanvas != null)
            characterCanvas.alpha = 0f;

        // Fade black screen out
        if (blackScreen != null)
            yield return StartCoroutine(FadeImage(blackScreen, 1f, 0f, blackFadeDuration));

        // Play door sound slightly after
        if (closingDoorSound != null && closingDoorSound.clip != null)
        {
            yield return new WaitForSeconds(doorSoundDelay);
            closingDoorSound.Play();
        }
    }

    IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = img.color;
        c.a = from;
        img.color = c;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            img.color = c;
            yield return null;
        }

        c.a = to;
        img.color = c;
    }
}