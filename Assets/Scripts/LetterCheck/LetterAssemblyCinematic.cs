using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LetterAssemblyCinematic : MonoBehaviour
{
    [Header("References")]
    public EnvelopeView envelopeView;
    public Image fadePanel;

    [Header("Audio")]
    public AudioSource assemblyAudio; // plays during letters/coins collapse
    public AudioSource slamAudio;     // plays when envelope snaps closed

    [Header("Settings")]
    public string nextSceneName;
    public float moveDuration = 0.35f;
    public float fadeDuration = 0.6f;

    private bool assembled;

    void Update()
    {
        if (assembled || envelopeView == null)
            return;

        // Cinematic triggers when coins are revealed and stamp clicked
        if (envelopeView.AreCoinsRevealed() && envelopeView.StampVerified)
        {
            assembled = true;
            StartCoroutine(ClosingCinematic());
        }
    }

    private IEnumerator ClosingCinematic()
    {
        envelopeView.cinematicPlaying = true;

        RectTransform envelopeRect = envelopeView.GetComponent<RectTransform>();
        NormalizeUI(envelopeRect);
        Vector2 targetPos = envelopeRect.anchoredPosition;

        // --- Letters glow + collapse ---
        List<RectTransform> letterRects = new List<RectTransform>();
        List<CanvasGroup> letterGroups = new List<CanvasGroup>();

        if (envelopeView.letterHalf != null && envelopeView.letterHalf.gameObject.activeSelf)
        {
            NormalizeUI(envelopeView.letterHalf.rectTransform);
            letterRects.Add(envelopeView.letterHalf.rectTransform);
            letterGroups.Add(GetOrAddCanvasGroup(envelopeView.letterHalf));
        }

        if (envelopeView.letterFull != null && envelopeView.letterFull.gameObject.activeSelf)
        {
            NormalizeUI(envelopeView.letterFull.rectTransform);
            letterRects.Add(envelopeView.letterFull.rectTransform);
            letterGroups.Add(GetOrAddCanvasGroup(envelopeView.letterFull));
        }

        foreach (CanvasGroup cg in letterGroups)
            StartCoroutine(PulseAlpha(cg, 0.15f));

        // Play assembly sound during collapse
        if (assemblyAudio != null)
            assemblyAudio.Play();

        yield return new WaitForSeconds(0.15f);

        if (letterRects.Count > 0)
            yield return AnimateElements(letterRects, letterGroups, targetPos);

        if (envelopeView.letterHalf != null)
            envelopeView.letterHalf.gameObject.SetActive(false);

        if (envelopeView.letterFull != null)
            envelopeView.letterFull.gameObject.SetActive(false);

        // --- Coins flash + collapse ---
        if (envelopeView.coins != null && envelopeView.coins.gameObject.activeSelf)
        {
            NormalizeUI(envelopeView.coins.rectTransform);
            CanvasGroup coinGroup = GetOrAddCanvasGroup(envelopeView.coins);

            StartCoroutine(PulseColor(envelopeView.coins, Color.white, 0.12f));
            yield return new WaitForSeconds(0.12f);

            yield return AnimateElements(
                new List<RectTransform> { envelopeView.coins.rectTransform },
                new List<CanvasGroup> { coinGroup },
                targetPos
            );

            envelopeView.coins.gameObject.SetActive(false);
        }

        // --- Envelope snap + shake + SLAM SOUND ---
        Image envelopeImage = envelopeView.GetComponent<Image>();
        if (envelopeImage != null && envelopeView.closedFront != null)
        {
            envelopeImage.sprite = envelopeView.closedFront;

            // Play slam sound here — AFTER letters & coins disappear
            if (slamAudio != null && slamAudio.clip != null)
                slamAudio.PlayOneShot(slamAudio.clip);

            yield return SnapShake(envelopeRect, 0.15f, 3f);
        }

        yield return new WaitForSeconds(0.05f);

        // --- Fade to black ---
        yield return FadeToBlack();

        envelopeView.cinematicPlaying = false;

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // ---------------- Helpers ----------------
    private IEnumerator AnimateElements(
        List<RectTransform> rects,
        List<CanvasGroup> groups,
        Vector2 targetPos)
    {
        int count = rects.Count;
        Vector2[] startPos = new Vector2[count];
        Vector3[] startScale = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            startPos[i] = rects[i].anchoredPosition;
            startScale[i] = rects[i].localScale;
            groups[i].alpha = 1f;
        }

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float eased = 1f - Mathf.Pow(1f - (t / moveDuration), 3f);

            for (int i = 0; i < count; i++)
            {
                rects[i].anchoredPosition = Vector2.Lerp(startPos[i], targetPos, eased);
                rects[i].localScale = startScale[i];
                groups[i].alpha = 1f - eased;
            }

            yield return null;
        }
    }

    private IEnumerator PulseAlpha(CanvasGroup cg, float duration)
    {
        float half = duration / 2f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(cg.alpha, 1f, t / half);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0.8f, t / half);
            yield return null;
        }
    }

    private IEnumerator PulseColor(Image img, Color flashColor, float duration)
    {
        Color original = img.color;
        float half = duration / 2f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            img.color = Color.Lerp(original, flashColor, t / half);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            img.color = Color.Lerp(flashColor, original, t / half);
            yield return null;
        }
    }

    private IEnumerator SnapShake(RectTransform rect, float duration = 0.15f, float magnitude = 3f)
    {
        Vector2 originalPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-magnitude, magnitude);
            float y = Random.Range(-magnitude, magnitude);
            rect.anchoredPosition = originalPos + new Vector2(x, y);
            yield return null;
        }

        rect.anchoredPosition = originalPos;
    }

    private IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        fadePanel.gameObject.SetActive(true);
        fadePanel.transform.SetAsLastSibling();
        fadePanel.color = new Color(0, 0, 0, 0);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }

        fadePanel.color = Color.black;
    }

    private CanvasGroup GetOrAddCanvasGroup(Graphic g)
    {
        CanvasGroup cg = g.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = g.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    private void NormalizeUI(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
    }
}