using UnityEngine;

public class MenuTitleVisibility : MonoBehaviour
{
    public Transform cameraTransform;
    public CanvasGroup canvasGroup;

    public float hideAtY = -4.5f;
    public float fadeDistance = 2f;
    public float fadeSpeed = 5f;
    public float fadeBuffer = 0.2f;

    void Start()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 1f;         
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void Update()
    {
        if (!cameraTransform || !canvasGroup)
            return;

        float distance = hideAtY - cameraTransform.position.y;

        float targetAlpha = 1f;
        if (distance > fadeBuffer)
        {
            targetAlpha = Mathf.Clamp01(1f - (distance - fadeBuffer) / fadeDistance);
        }

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        canvasGroup.interactable = canvasGroup.alpha > 0.9f;
        canvasGroup.blocksRaycasts = canvasGroup.alpha > 0.9f;
    }
}