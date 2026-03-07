using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LetterDrag : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Canvas canvas;
    public int sceneIndexToLoad = 3;
    public float doubleClickTime = 0.3f;

    [HideInInspector] public bool isDragging;

    private RectTransform rect;
    private CanvasGroup group;
    private float lastClick;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        EnsureCanvasGroup();
    }

    void EnsureCanvasGroup()
    {
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        EnsureCanvasGroup();

        isDragging = true;
        e.Use();

        if (Time.time - lastClick < doubleClickTime)
            SceneManager.LoadScene(sceneIndexToLoad);

        lastClick = Time.time;
        group.blocksRaycasts = false;
        group.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!canvas) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            e.position,
            canvas.worldCamera,
            out Vector2 pos
        );

        rect.anchoredPosition = pos;
    }

    public void OnPointerUp(PointerEventData e)
    {
        EnsureCanvasGroup();

        isDragging = false;
        group.blocksRaycasts = true;
        group.alpha = 1f;
    }
}