using UnityEngine;
using UnityEngine.EventSystems;

public class CoinClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private EnvelopeView envelope;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!envelope.AreCoinsRevealed())
            envelope.RevealCoins();
    }
}