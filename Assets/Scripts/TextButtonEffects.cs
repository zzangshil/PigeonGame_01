using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Text Settings")]
    public TextMeshProUGUI buttonText; // assign your button's TextMeshPro
    public float hoverAlpha = 0.5f;    // alpha when hovering
    private float normalAlpha = 1f;

    [Header("Pop Settings")]
    public float popScale = 1.2f;      // scale when pressed
    private Vector3 originalScale;

    private bool clicked = false;

    private void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        originalScale = buttonText.transform.localScale;
    }

    // Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!clicked)
        {
            SetAlpha(hoverAlpha);
        }
    }

    // Exit hover
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!clicked)
        {
            SetAlpha(normalAlpha);
        }

        buttonText.transform.localScale = originalScale; // reset scale
    }

    // Press
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonText.transform.localScale = originalScale * popScale;
    }

    // Release / click
    public void OnPointerUp(PointerEventData eventData)
    {
        buttonText.transform.localScale = originalScale;
        clicked = true;       // mark as clicked
        SetAlpha(normalAlpha); // return alpha to full
    }

    private void SetAlpha(float alpha)
    {
        Color c = buttonText.color;
        c.a = alpha;
        buttonText.color = c;
    }
}