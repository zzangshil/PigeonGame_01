using UnityEngine;

public class MenuBottomVisibility : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public CanvasGroup canvasGroup;

    [Header("Visibility Settings")]
    public float showAtY = -4.5f;      // camera Y where buttons are fully visible
    public float fadeDistance = 2f;    // distance over which fade happens
    public float fadeSpeed = 5f;       // how fast alpha interpolates
    public float fadeBuffer = 0.2f;    // small buffer to prevent jitter

    void Start()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;         
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (!cameraTransform || !canvasGroup)
            return;

        float distance = showAtY - cameraTransform.position.y;

        // Only start fading if distance is more than the buffer
        float targetAlpha = 0f;
        if (distance > fadeBuffer)
        {
            targetAlpha = Mathf.Clamp01((distance - fadeBuffer) / fadeDistance);
        }

        // Smooth interpolation
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        canvasGroup.interactable = canvasGroup.alpha > 0.9f;
        canvasGroup.blocksRaycasts = canvasGroup.alpha > 0.9f;
    }
}