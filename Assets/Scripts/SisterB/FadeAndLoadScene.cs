using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeAfterDialogueEvent : MonoBehaviour
{
    [Header("References")]
    public DialogueBubble dialogueBubble;
    public CanvasGroup fadePanel;

    [Header("Fade")]
    public float fadeDuration = 1f;
    public string sceneToLoad;

    void Awake()
    {
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
    }

    void OnEnable()
    {
        dialogueBubble.OnDialogueEnded += HandleDialogueEnded;
    }

    void OnDisable()
    {
        dialogueBubble.OnDialogueEnded -= HandleDialogueEnded;
    }

    void HandleDialogueEnded()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        fadePanel.blocksRaycasts = true;
        yield return Fade(0f, 1f);
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator Fade(float from, float to)
    {
        float time = 0f;
        fadePanel.alpha = from;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = to;
    }
}