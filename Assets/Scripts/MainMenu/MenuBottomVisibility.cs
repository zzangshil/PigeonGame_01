using UnityEngine;

public class MenuBottomVisibility : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public CanvasGroup canvasGroup;

    [Header("Visibility Settings")]
    public float showAtY = -4.5f;     
    public float fadeDistance = 2f;    
    public float fadeSpeed = 5f;     
    public float fadeBuffer = 0.2f;   

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

        float targetAlpha = 0f;
        if (distance > fadeBuffer)
        {
            targetAlpha = Mathf.Clamp01((distance - fadeBuffer) / fadeDistance);
        }

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        canvasGroup.interactable = canvasGroup.alpha > 0.9f;
        canvasGroup.blocksRaycasts = canvasGroup.alpha > 0.9f;
    }
}