using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnvelopeView : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IEndDragHandler
{
    public enum EnvelopeState { ClosedFront, ClosedBack, Open }

    [Header("Sprites")]
    public Sprite closedFront;
    public Sprite closedBack;
    public Sprite openSprite;

    [Header("Contents")]
    public Image letterHalf;
    public Image letterFull;
    public Image coins;
    public Image stamp;

    [Header("Stamp Settings")]
    public bool stampAuthentic = true;

    [Header("Audio")]
    public EnvelopeShakeAudio coinAudio;

    [Header("Shake Settings")]
    public float shakeThreshold = 200f;
    public float shakeCooldown = 0.1f;

    private Image envelopeImage;
    private EnvelopeState state = EnvelopeState.ClosedFront;

    private int clickCount;
    private float clickTimer;
    private const float doubleClickTime = 0.3f;

    private bool inspectingStamp;
    private Vector2 lastPosition;
    private float shakeTimer;

    [HideInInspector]
    public bool cinematicPlaying = false;
    [HideInInspector]
    public bool stampClicked = false; 

    void Awake()
    {
        envelopeImage = GetComponent<Image>();
        envelopeImage.sprite = closedFront;

        HideAllContents();

        if (coins != null)
            coins.gameObject.SetActive(false);

        lastPosition = ((RectTransform)transform).anchoredPosition;
    }

    void Update()
    {
        HandleDoubleClickTimer();
        HandleShakeDetection();
    }

    private void HandleDoubleClickTimer()
    {
        if (clickCount <= 0) return;

        clickTimer += Time.unscaledDeltaTime;
        if (clickTimer > doubleClickTime)
        {
            clickCount = 0;
            clickTimer = 0f;
        }
    }

    private void HandleShakeDetection()
    {
        if (state != EnvelopeState.Open || AreCoinsRevealed() || coinAudio == null) return;

        Vector2 currentPos = ((RectTransform)transform).anchoredPosition;
        Vector2 delta = currentPos - lastPosition;
        float speed = delta.magnitude / Time.unscaledDeltaTime;

        if (speed > shakeThreshold)
        {
            coinAudio.StartShake();
            shakeTimer = 0f;
        }
        else
        {
            shakeTimer += Time.unscaledDeltaTime;
            if (shakeTimer > shakeCooldown)
                coinAudio.StopShake();
        }

        lastPosition = currentPos;
    }

    // ---------------- Pointer Click ----------------
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cinematicPlaying) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            clickCount++;
            if (clickCount == 2)
            {
                HandleDoubleClick();
                clickCount = 0;
                clickTimer = 0f;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (state != EnvelopeState.Open)
                FlipEnvelope();
        }
    }

    private void HandleDoubleClick()
    {
        if (state != EnvelopeState.Open)
        {
            OpenEnvelope();
        }
        else
        {
            RevealCoins();
        }
    }

    // ---------------- Dragging ----------------
    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) => coinAudio?.StopShake();

    // ---------------- Envelope flipping ----------------
    public void FlipEnvelope()
    {
        if (state == EnvelopeState.Open) return;

        EndStampInspection();

        if (state == EnvelopeState.ClosedFront)
        {
            state = EnvelopeState.ClosedBack;
            envelopeImage.sprite = closedBack;

            if (!stampClicked) StartStampInspection();
        }
        else
        {
            state = EnvelopeState.ClosedFront;
            envelopeImage.sprite = closedFront;
        }

        UpdateStampVisibility();
        HideLetterHalf();
    }

    public void OpenEnvelope()
    {
        if (state == EnvelopeState.Open) return;

        state = EnvelopeState.Open;
        envelopeImage.sprite = openSprite;

        if (letterHalf != null)
            letterHalf.gameObject.SetActive(true);

        EndStampInspection();
        HideStamp();
    }

    // ---------------- Letter ----------------
    public void RevealFullLetter()
    {
        if (letterHalf != null) letterHalf.gameObject.SetActive(false);
        if (letterFull != null) letterFull.gameObject.SetActive(true);
    }

    // ---------------- Coins ----------------
    public void RevealCoins()
    {
        if (coins == null) return;
        if (AreCoinsRevealed()) return;

        coins.gameObject.SetActive(true);
        coins.transform.SetParent(envelopeImage.transform.parent, false);
        coins.transform.SetAsLastSibling();

        coinAudio?.StopShake();
    }

    public bool AreCoinsRevealed()
    {
        return coins != null && coins.gameObject.activeSelf;
    }

    // ---------------- Stamp ----------------
    public void VerifyStamp()
    {
        if (stamp == null) return;

        stamp.color = stampAuthentic ? Color.green : Color.red;
        stampClicked = true; // mark clicked
        EndStampInspection();
    }

    public bool StampVerified => stampClicked;

    // ---------------- Helpers ----------------
    private void StartStampInspection()
    {
        inspectingStamp = true;
        UpdateStampVisibility();
    }

    private void EndStampInspection()
    {
        inspectingStamp = false;
        UpdateStampVisibility();
    }

    private void UpdateStampVisibility()
    {
        if (stamp == null) return;

        bool show = state == EnvelopeState.ClosedBack;
        stamp.gameObject.SetActive(show);
        stamp.raycastTarget = show && inspectingStamp && !stampClicked;
    }

    private void HideStamp()
    {
        if (stamp == null) return;

        stamp.gameObject.SetActive(false);
        stamp.raycastTarget = false;
    }

    private void HideLetterHalf()
    {
        if (letterHalf != null)
            letterHalf.gameObject.SetActive(false);
    }

    private void HideAllContents()
    {
        if (letterHalf != null) letterHalf.gameObject.SetActive(false);
        if (letterFull != null) letterFull.gameObject.SetActive(false);
        if (coins != null) coins.gameObject.SetActive(false);
        HideStamp();
    }
}