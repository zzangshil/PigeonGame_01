using UnityEngine;
using UnityEngine.EventSystems;

public class LetterHalfClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private EnvelopeView envelope;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        envelope.RevealFullLetter();
    }
}