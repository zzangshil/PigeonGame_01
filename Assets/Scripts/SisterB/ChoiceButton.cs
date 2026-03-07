using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private DialogueBubble dialogue;
    private DialogueChoice choice;

    [Header("Hover & Pop")]
    public float hoverAlpha = 0.6f;
    public float popScale = 1.2f;
    public float popDuration = 0.1f;

    [Header("Audio")]
    public AudioClip clickSound;

    private TextMeshProUGUI text;
    private Color originalColor;
    private Vector3 originalScale;

    public void Setup(DialogueBubble bubble, DialogueChoice newChoice)
    {
        dialogue = bubble;
        choice = newChoice;

        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = choice.text;
            originalColor = text.color;
            originalScale = transform.localScale;
        }
        else
        {
            Debug.LogWarning($"{name}: No TextMeshProUGUI found!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = hoverAlpha;
        text.color = c;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text == null) return;
        text.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (text == null) return;

        // Play click sound
        if (clickSound != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);

        // Pop animation
        StopAllCoroutines();
        StartCoroutine(PopEffect());

        // Trigger dialogue choice
        if (dialogue != null && choice != null)
        {
            dialogue.OnChoiceSelected(choice);
        }
        else
        {
            Debug.LogWarning($"{name}: Dialogue or choice is null!");
        }
    }

    private System.Collections.IEnumerator PopEffect()
    {
        transform.localScale = originalScale * popScale;
        yield return new WaitForSeconds(popDuration);
        transform.localScale = originalScale;
    }
}