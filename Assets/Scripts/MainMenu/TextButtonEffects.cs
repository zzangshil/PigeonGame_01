using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TextButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Text Settings")]
    public TextMeshProUGUI buttonText;
    public float hoverAlpha = 0.5f;
    public float alphaSpeed = 10f;
    private float normalAlpha = 1f;

    [Header("Pop Settings")]
    public float popScale = 1.2f;
    public float scaleSpeed = 10f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public float actionDelay = 0.15f; 

    private bool isHovering = false;

    private void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        originalScale = buttonText.transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        float targetAlpha = isHovering ? hoverAlpha : normalAlpha;
        Color c = buttonText.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * alphaSpeed);
        buttonText.color = c;

        buttonText.transform.localScale = Vector3.Lerp(buttonText.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * popScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale;

        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlayGame(string sceneName)
    {
        StartCoroutine(DoActionAfterSound(() => SceneManager.LoadScene(sceneName)));
    }

    public void QuitGame()
    {
        StartCoroutine(DoActionAfterSound(() => Application.Quit()));
    }

    private IEnumerator DoActionAfterSound(System.Action action)
    {
        yield return new WaitForSeconds(actionDelay);
        action?.Invoke();
    }
}
