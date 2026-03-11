using UnityEngine;
using UnityEngine.EventSystems;

public class StampClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private EnvelopeView envelope;
    [SerializeField] private AudioSource stampAudio;
    [SerializeField] private Color verifiedColor = Color.green;
    [SerializeField] private Color fakeColor = Color.red;
    private bool used;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (used || eventData.button != PointerEventData.InputButton.Left) return;

        used = true;

        // play sound instantly
        if (stampAudio != null && stampAudio.clip != null)
            stampAudio.PlayOneShot(stampAudio.clip);

        // verify stamp and mark clicked
        envelope.VerifyStamp();

        // immediate color feedback
        if (envelope.stamp != null)
            envelope.stamp.color = envelope.stampAuthentic ? verifiedColor : fakeColor;

        eventData.Use();
    }
}